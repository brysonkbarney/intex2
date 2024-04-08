using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
 
namespace intex2.Controllers
{
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleMgr)
        {
            _roleManager = roleMgr;
        }
 
        public ViewResult Index() => View(_roleManager.Roles);
 
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
        public IActionResult Create() => View();
 
        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            return View(name);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "No role found");
            return View("Index", _roleManager.Roles);
        }
    }
}