using System.Security.Claims;
using BinusZoom.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom.Controllers;

public class LoginController : Controller
{
    private readonly BinusZoomContext _context;

    public LoginController(BinusZoomContext context)
    {
        _context = context;
    }
    
    // GET: Login
    [HttpGet("Login/")]
    public async Task<IActionResult> Login()
    {
        return View();
    }
    
    // GET: Login
    [HttpPost("Login/")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _context.UserLogin
            .FirstOrDefaultAsync(m => m.Username == username && m.Password == password);
        if (user == null) return NotFound();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            IsPersistent = true
        };
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        
        return View();
    }
    
    [HttpGet("Logout/")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}