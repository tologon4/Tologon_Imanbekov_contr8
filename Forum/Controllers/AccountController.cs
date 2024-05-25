using contr8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace contr8.Controllers;

public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private readonly IWebHostEnvironment _environment;


    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User? user = await _userManager.FindByEmailAsync(model.LoginValue);
            if (user == null)
                user = await _userManager.FindByNameAsync(model.LoginValue);
            if (user != null)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction();
                }
            }
            ModelState.AddModelError("", "Неправильный логин или пароль!\nИли вы заблокированы!");
        }
        ModelState.AddModelError("", "Неправильная заполненная форма!");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, IFormFile uploadedFile)
    {
        if (ModelState.IsValid)
        {
            string newFileName = Path.ChangeExtension($"{model.UserName.Trim()}-ProfileN=1", Path.GetExtension(uploadedFile.FileName));
            string path= $"/userImages/" + newFileName.Trim();
            using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }
            User user = new User()
            {
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                Avatar = path
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction();
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
        ModelState.AddModelError("", "Что-то пошло не так, проверьте все данные!");
        return View(model);
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}