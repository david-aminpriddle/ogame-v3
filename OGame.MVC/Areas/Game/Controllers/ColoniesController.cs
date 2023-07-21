using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OGame.MVC.Areas.Game.Controllers;

[Area("Game")]
[Authorize]
public class ColoniesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}