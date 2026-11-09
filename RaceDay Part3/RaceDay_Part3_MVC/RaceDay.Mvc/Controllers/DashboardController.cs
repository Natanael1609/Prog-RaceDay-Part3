using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
namespace RaceDay.Mvc.Controllers;
public class DashboardController(RaceDayApiClient api,SessionAuth auth):Controller
{
    public async Task<IActionResult> Index()
    {
        if(!auth.SignedIn)return RedirectToAction("Login","Account");
        if(auth.Role?.Equals("Organiser",StringComparison.OrdinalIgnoreCase)==true)
        { try{ViewBag.Events=await api.Get("api/events/mine");}catch{ViewBag.Events=default(System.Text.Json.JsonElement);} return View("Organiser"); }
        try{ViewBag.Enrolments=await api.Get("api/enrolments/mine");}catch{ViewBag.Enrolments=default(System.Text.Json.JsonElement);}
        return View("Participant");
    }
}
