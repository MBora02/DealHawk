using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using DealHawk.WebMVC.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.Controllers
{

    public class AuthController : Controller
    {
        private readonly DealHawkApiClient _apiClient;

        public AuthController(DealHawkApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            var result = await _apiClient.LoginAsync(username, password);
            if (result == null || !result.IsSuccess)
            {
                ViewBag.Error = result?.Message ?? "Invalid credentials.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Username),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim("Token", result.Token)
            };

            foreach (var role in result.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string username, string password, string fullName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            var result = await _apiClient.RegisterAsync(email, username, password, fullName);
            if (result == null || !result.IsSuccess)
            {
                ViewBag.Error = result?.Message ?? "Registration failed.";
                return View();
            }

            return await Login(username, password);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
