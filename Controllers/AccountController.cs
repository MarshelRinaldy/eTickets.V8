using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eTickets.V8.Data;
using eTickets.V8.Data.Static;
using eTickets.V8.Data.ViewModels;
using eTickets.V8.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace eTickets.V8.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signManager, AppDbContext context, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signManager = signManager;
            _context = context;
            _logger = logger;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            var response = new LoginVM();
            return View(response);
        }

        //Login menggunakan GoogleAcccount
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LoginGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
                Items =
                {
                    { ".xsrf", Guid.NewGuid().ToString() }                    
                }

            };

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
            return new EmptyResult();
        }

        //Login menggunakan GoogleAcccount
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                // Coba authenticate dengan Google scheme terlebih dahulu
                var googleResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (googleResult.Succeeded)
                {
                    // Sign in user dengan cookie authentication
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        googleResult.Principal,
                        googleResult.Properties);

                    var claims = googleResult.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
                    {
                        claim.Issuer,
                        claim.OriginalIssuer,
                        claim.Type,
                        claim.Value
                    });

                    // Tampilkan hasil JSON dalam view yang lebih readable
                    ViewBag.Claims = claims;
                    ViewBag.JsonResult = System.Text.Json.JsonSerializer.Serialize(claims, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    return View("GoogleResponse");
                }
                else
                {
                    // Coba authenticate dengan cookie scheme sebagai fallback
                    var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    if (cookieResult.Succeeded)
                    {
                        var claims = cookieResult.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
                        {
                            claim.Issuer,
                            claim.OriginalIssuer,
                            claim.Type,
                            claim.Value
                        });

                        ViewBag.Claims = claims;
                        ViewBag.JsonResult = System.Text.Json.JsonSerializer.Serialize(claims, new System.Text.Json.JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        return View("GoogleResponse");
                    }
                    else
                    {
                        _logger.LogWarning("Google authentication failed: {Error}", googleResult.Failure?.Message);
                        TempData["Error"] = "Google authentication failed";
                        return RedirectToAction("Login");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GoogleResponse");
                TempData["Error"] = "An error occurred during Google authentication";
                return RedirectToAction("Login");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            //model state ini berpatokan pada VM yang kita buat sebagai contoh required, sehingga ketika tidak diisi  maka tidak valid model statenya 
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            //cari dulu email ada atau nga di db
            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                //conditional jika password benar
                if (passwordCheck)
                {
                    var result = await _signManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        //Index merupakan nama method, dan "Moviex" adalah nama controller dibawah ini
                        return RedirectToAction("Index", "Movies");
                    }
                }
                TempData["Error"] = "password is incorrect";
                return View(loginVM);
            }
            TempData["Error"] = "Email is incorrect";
            return View(loginVM);
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            var response = new RegisterVM();
            return View(response);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);

            if (user != null)
            {
                TempData["Error"] = "Email already exists";
                return View(registerVM);
            }

            var newUser = new ApplicationUser()
            {
                FullName = registerVM.FullName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress,
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (newUserResponse.Succeeded)
            {
                _logger.LogInformation("User created successfully: {Email}", registerVM.EmailAddress);

                var roleResult = await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Role assigned successfully to user: {Email}", registerVM.EmailAddress);
                    TempData["Success"] = "Account created successfully! You can now login.";
                }
                else
                {
                    _logger.LogError("Failed to assign role to user: {Email}. Errors: {Errors}",
                        registerVM.EmailAddress, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    TempData["Error"] = "Account created but role assignment failed.";
                }
            }
            else
            {
                _logger.LogError("Failed to create user: {Email}. Errors: {Errors}",
                    registerVM.EmailAddress, string.Join(", ", newUserResponse.Errors.Select(e => e.Description)));

                // Add specific error messages for common issues
                var errorMessages = newUserResponse.Errors.Select(e => e.Description).ToList();
                if (errorMessages.Any(e => e.Contains("Password")))
                {
                    TempData["Error"] = "Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
                }
                else
                {
                    TempData["Error"] = string.Join(", ", errorMessages);
                }

                return View(registerVM);
            }

            return View("RegisterCompleted");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // Method alternatif untuk melihat JSON response langsung
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponseJson()
        {
            try
            {
                // Coba authenticate dengan Google scheme terlebih dahulu
                var googleResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (googleResult.Succeeded)
                {
                    var claims = googleResult.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
                    {
                        claim.Issuer,
                        claim.OriginalIssuer,
                        claim.Type,
                        claim.Value
                    });

                    return Json(new
                    {
                        success = true,
                        claims = claims,
                        message = "Google authentication successful"
                    });
                }
                else
                {
                    // Coba authenticate dengan cookie scheme sebagai fallback
                    var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    if (cookieResult.Succeeded)
                    {
                        var claims = cookieResult.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
                        {
                            claim.Issuer,
                            claim.OriginalIssuer,
                            claim.Type,
                            claim.Value
                        });

                        return Json(new
                        {
                            success = true,
                            claims = claims,
                            message = "Google authentication successful (via cookie)"
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Google authentication failed in JSON method: {Error}", googleResult.Failure?.Message);
                        return Json(new
                        {
                            success = false,
                            error = "Google authentication failed",
                            details = googleResult.Failure?.Message ?? "Unknown error"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GoogleResponseJson");
                return Json(new
                {
                    success = false,
                    error = "An error occurred during Google authentication",
                    details = ex.Message
                });
            }
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // Method untuk debugging autentikasi
        [AllowAnonymous]
        public async Task<IActionResult> DebugAuth()
        {
            try
            {
                var googleResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
                var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var debugInfo = new
                {
                    GoogleAuth = new
                    {
                        IsAuthenticated = googleResult.Succeeded,
                        AuthenticationType = googleResult.Ticket?.AuthenticationScheme,
                        Claims = googleResult.Succeeded ? googleResult.Principal.Identities.FirstOrDefault()?.Claims.Select(c => new { c.Type, c.Value }) : null,
                        Failure = googleResult.Failure?.Message,
                        Properties = googleResult.Properties?.Items
                    },
                    CookieAuth = new
                    {
                        IsAuthenticated = cookieResult.Succeeded,
                        AuthenticationType = cookieResult.Ticket?.AuthenticationScheme,
                        Claims = cookieResult.Succeeded ? cookieResult.Principal.Identities.FirstOrDefault()?.Claims.Select(c => new { c.Type, c.Value }) : null,
                        Failure = cookieResult.Failure?.Message,
                        Properties = cookieResult.Properties?.Items
                    },
                    User = new
                    {
                        IsAuthenticated = User.Identity?.IsAuthenticated,
                        Name = User.Identity?.Name,
                        Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
                    }
                };

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        //Method yang dijalankan ketika logun with google
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Movies");
            if (remoteError != null)
            {
                TempData["Error"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login");
            }

            var info = await _signManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Error"] = "Error loading external login information.";
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // User already exists, sign in
                return LocalRedirect(returnUrl);
            }
            else
            {
                // If the user does not have an account, create one
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = info.Principal.FindFirstValue(ClaimTypes.Name)
                    };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                await _userManager.AddLoginAsync(user, info);
                await _signManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
        }


    }
}