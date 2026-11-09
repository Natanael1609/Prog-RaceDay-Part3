using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
namespace RaceDay.Mvc.Controllers;
public class ParticipantController(RaceDayApiClient api,SessionAuth auth):Controller
{
    private bool Allowed()=>auth.SignedIn&&auth.Role?.Equals("Participant",StringComparison.OrdinalIgnoreCase)==true;
    public async Task<IActionResult> Details(int id){try{ViewBag.Event=await api.Get($"api/events/{id}");ViewBag.Categories=await api.Get($"api/events/{id}/categories");}catch{return NotFound();}return View();}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Enrol(EnrolVm vm){if(!Allowed())return RedirectToAction("Login","Account");try{await api.Post("api/enrolments",new{vm.EventId,vm.CategoryId});TempData["Message"]="Your enrolment has been submitted."; }catch{TempData["Error"]="Enrolment could not be completed."; }return RedirectToAction(nameof(Details),new{id=vm.EventId});}
    public async Task<IActionResult> MyEvents(){if(!Allowed())return RedirectToAction("Login","Account");try{ViewBag.Data=await api.Get("api/enrolments/mine");}catch{ViewBag.Data=default(System.Text.Json.JsonElement);}return View();}
    public async Task<IActionResult> MyResults(){if(!Allowed())return RedirectToAction("Login","Account");try{ViewBag.Data=await api.Get("api/results/mine");}catch{ViewBag.Data=default(System.Text.Json.JsonElement);}return View();}
    [HttpGet] public async Task<IActionResult> Profile(){if(!Allowed())return RedirectToAction("Login","Account");try{var p=await api.Get("api/users/me");return View(new ProfileVm{Name=p.TryGetProperty("name",out var n)?n.ToString():"",Email=p.TryGetProperty("email",out var e)?e.ToString():"",Phone=p.TryGetProperty("phone",out var ph)?ph.ToString():""});}catch{return View(new ProfileVm());}}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Profile(ProfileVm vm){if(!Allowed())return Forbid();if(!ModelState.IsValid)return View(vm);try{await api.Put("api/users/me",new{vm.Name,vm.Email,vm.Phone});if(vm.ProfilePicture!=null)await api.Upload("api/users/me/profile-picture",vm.ProfilePicture);TempData["Message"]="Profile updated.";return RedirectToAction(nameof(Profile));}catch{ModelState.AddModelError("","Profile could not be updated.");return View(vm);}}
}
