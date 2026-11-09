using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
namespace RaceDay.Mvc.Controllers;
public class HomeController(RaceDayApiClient api):Controller
{
    public async Task<IActionResult> Index(string? search,string? eventType)
    {
        try { var data=await api.Get("api/events"); ViewBag.Events=data; ViewBag.Search=search; ViewBag.EventType=eventType; }
        catch { ViewBag.Events=System.Text.Json.JsonDocument.Parse("[]").RootElement; ViewBag.ApiUnavailable=true; }
        return View();
    }
    public IActionResult Error()=>View();
}
