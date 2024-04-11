using Humanizer;
using intex2.Models;
using intex2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
 
namespace intex2.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private readonly EmailHelper _emailHelper;
        private ILegoRepository _repo;
 
        public AdminController(UserManager<AppUser> usrMgr, IPasswordHasher<AppUser> passwordHash, EmailHelper emailHelper, ILegoRepository temp)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
            _emailHelper = emailHelper;
            _repo = temp;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(userManager.Users);
        }
        [AllowAnonymous]
        public ViewResult Create() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = user.Name,
                    Email = user.Email, 
                    TwoFactorEnabled = user.TwoFactor
                   
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);
                    await _emailHelper.InitializeAsync();
                    bool emailResponse = _emailHelper.SendEmail(user.Email, confirmationLink);
                    
                    Customer customer = new Customer()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CountryOfResidence = user.CountryOfResidence,
                        BirthDate = user.BirthDate,
                        Age = user.Age,
                        Gender = user.Gender,
                        NetUserId = appUser.Id
                    };
                    _repo.CreateCustomer(customer);
                    _repo.Save();
                    
                    if (emailResponse)
                        return View("../Email/GoConfirm");
                    else
                    {
                        // log email failed 
                    }
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddGoogleDetails(User user)
        {
            Customer customer = new Customer()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CountryOfResidence = user.CountryOfResidence,
                BirthDate = user.BirthDate,
                Age = user.Age,
                Gender = user.Gender,
                NetUserId = user.NetUserId
            };
                _repo.CreateCustomer(customer);
                _repo.Save();

            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<IActionResult> Update(string id)
        {
            AppUser appUser = await userManager.FindByIdAsync(id);
            Customer customer = _repo.GetCustomerByNetUserId(appUser.Id);
            User user = new User()
            {
                Name = appUser.UserName,
                Email = appUser.Email,
                TwoFactor = appUser.TwoFactorEnabled,
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                BirthDate = customer.BirthDate,
                CountryOfResidence = customer.CountryOfResidence,
                Gender = customer.Gender,
                Age = customer.Age,
                NetUserId = customer.NetUserId,
            };
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(string id, string email, string password, 
            bool twoFactor, string gender, int age, string firstName, string lastName, DateOnly birthdate,
            string countryOfResidence, string name)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            Customer customer = _repo.GetCustomerByNetUserId(user.Id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");
 
                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");
                
                if (!string.IsNullOrEmpty(name))
                    user.UserName = name;
                else
                    ModelState.AddModelError("", "Name cannot be empty");
                
                user.TwoFactorEnabled = twoFactor;
                if (customer != null)
                {
                    if (!string.IsNullOrEmpty(gender))
                        customer.Gender = gender;
                    else
                        ModelState.AddModelError("", "Gender cannot be empty");
                    if (age != 0)
                        customer.Age = age;
                    else
                        ModelState.AddModelError("", "Age cannot be 0");
                    if (!string.IsNullOrEmpty(firstName))
                        customer.FirstName = firstName;
                    else
                        ModelState.AddModelError("", "First Name cannot be empty");
                    if (!string.IsNullOrEmpty(lastName))
                        customer.LastName = lastName;
                    else
                        ModelState.AddModelError("", "Last Name cannot be empty");
                    if (birthdate != DateOnly.MinValue)
                        customer.BirthDate = birthdate;
                    else
                        ModelState.AddModelError("", "Invalid Birthdate");
                    if (!string.IsNullOrEmpty(countryOfResidence))
                        customer.CountryOfResidence = countryOfResidence;
                    else
                        ModelState.AddModelError("", "Invalid Birthdate");
                }
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    bool cresult = _repo.UpdateCustomer(customer);
                    _repo.Save();
                    if (result.Succeeded && cresult)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }
        [Authorize(Roles = "Admin")]
        
        public IActionResult ReviewOrders(int pageNum = 1, int pageSize = 1000)
        {
            IQueryable<Order> orders = _repo.Orders.Where(x => x.Fraud == 1)
                .OrderByDescending(x => x.Date)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize);

            var paginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = _repo.Orders.Count(x => x.Fraud == 1)
            };

            var model = new ReviewOrdersViewModel
            {
                Orders = orders,
                PaginationInfo = paginationInfo
            };

            return View("ReviewOrders", model);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ReviewOrdersAll()
        {
            IQueryable<Order> orders = _repo.Orders.OrderByDescending(x => x.Date);

            var model = new ReviewOrdersViewModel
            {
                Orders = orders,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = orders.Count(),
                    TotalItems = orders.Count()
                }
            };

            return View("ReviewOrders", model);
        }
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ManageProducts()
        {
            IQueryable<Product> ps = _repo.Products;
            return View(ps);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(int id)
        {
            Product p = _repo.Products.Where(x => x.ProductId == id).SingleOrDefault();
            return View(p);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(Product p)
        {
            _repo.UpdateProduct(p);
            _repo.Save();
            return RedirectToAction("ManageProducts");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct(Product p)
        {
            _repo.CreateProduct(p);
            _repo.Save();
            return RedirectToAction("ManageProducts");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(int id)
        {
            Product p = _repo.Products.Where(x => x.ProductId == id).SingleOrDefault();
            _repo.DeleteProduct(p);
            _repo.Save();
            return RedirectToAction("ManageProducts");
        }
    }
}