using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Services;
using System.Security.Claims;

namespace SchoolApp.Controllers;

public class UserController : Controller
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<UserController> _logger;

    public UserController(IApplicationService applicationService, ILogger<UserController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    [HttpGet]
    //[Authorize]   -- Δεν χρειάζεται λόγω του fallback auth policy
    public IActionResult Index()
    {
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);
        _logger.LogInformation("Role claim={Role}", roleClaim);

        if (User.IsInRole("ADMIN"))
        {
            return RedirectToAction("Index", "Admin");
        }
        else if (User.IsInRole("TEACHER")) 
        {
            return RedirectToAction("Index", "Teacher");
        }
        //else if (User.HasClaim("Capability", "VIEW_STUDENT_DASHBOARD")) -- πάντα με Capability
        else if (User.IsInRole("STUDENT"))
        {
            return RedirectToAction("Index", "Student");
        }
        else
        {
            return RedirectToAction("AccessDenied", "Home");
        }
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?IsAuthenticated != true)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        return RedirectToAction("Index", "User");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginDTO userLoginDTO, string? returnUrl = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewData["RetutnUrl"] = returnUrl;
                return View(userLoginDTO);
            }

            var user = await _applicationService.UserService.VerifyAndGetUserAsync(userLoginDTO);

            if (user is null)
            {
                ViewData["RetutnUrl"] = returnUrl;
                ViewData["ValidateMessage"] = "Bad Credentials. Username or password is invalid.";
                return View(userLoginDTO);
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            claims.AddRange(user.Role.Capabilities.Select(c => new Claim("Capability", c.Name)));

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new()
            {
                AllowRefresh = true;
                IsPersistent = userLoginDTO.KeepLoggedIn;
            }

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties
                );

            _logger.LogInformation("User {Username} logged in.", principal.Identity?.Name);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "User");


        }
        catch (Exception)
        {
            _logger.LogWarning("Error in login");
            ViewData["ValidateMessage"] = "An error occured.";
            ViewData["ReturnUrl"] = returnUrl;
            return View(userLoginDTO);
        }
    }

    [HttpPost]      // logout should be post for being secured through CSRF Token
    //[Authorize]  -- default λόγω του fallback policy
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User {Username} logged out.", username);
        return RedirectToAction("Login", "User");
    }



}