using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BinusZoom.Data;
using BinusZoom.Models;
using BinusZoom.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BinusZoom.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BinusZoomContext _context;
    public HomeController(ILogger<HomeController> logger, BinusZoomContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Meeting.Where(meeting => meeting.MeetingDate > DateTime.Now).ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}