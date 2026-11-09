using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
namespace RaceDay.Mvc.Controllers;
public class AccountController(RaceDayApiClient api,SessionAuth auth):Controller
{
    [HttpGet] public IActionResult Login()=>View();
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginVm vm)
    {
        if(!ModelState.IsValid)return View(vm);
        try { var result=await api.Post("api/auth/login",new{vm.Email,vm.Password}); auth.SignIn(result,vm.Email); return RedirectToAction("Index","Dashboard"); }
        catch { ModelState.AddModelError("","Login failed. Check your details and confirm the API is running."); return View(vm); }
    }
    [HttpGet] public IActionResult Register()=>View();
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterVm vm)
    {
        if(!ModelState.IsValid)return View(vm);
        try { await api.Post("api/auth/register",new{vm.Name,vm.Email,vm.Password,vm.Role}); TempData["Message"]="Registration completed. Please log in."; return RedirectToAction(nameof(Login)); }
        catch { ModelState.AddModelError("","Registration could not be completed. Check your details."); return View(vm); }
    }
    public IActionResult Logout(){auth.SignOut();return RedirectToAction("Index","Home");}
}
