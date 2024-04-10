using Humanizer;
using intex2.Models;
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
 
        public IActionResult Index()
        {
            return View(userManager.Users);
        }
        public ViewResult Create() => View();

        [HttpPost]
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
 
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}