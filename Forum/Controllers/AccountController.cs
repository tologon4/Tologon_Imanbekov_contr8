using contr8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace contr8.Controllers;

public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private IWebHostEnvironment _environment;
    private IHttpContextAccessor _httpContextAccessor;
    private ForumDb _db { get; set; }


    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
        IWebHostEnvironment environment, ForumDb db, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Profile(int? userId)
    {
        var referrer = _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
        if (userId.HasValue)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
                return View(user);
            return Redirect(referrer);
        }
        return Redirect(referrer);
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
                    return RedirectToAction("Profile", new {userId = user.Id });
                }
            }
            ModelState.AddModelError("", "Неправильный логин или пароль!");
        }
        ModelState.AddModelError("", "Неправильная заполненная форма!");
        return View(model);
    }

    [Authorize]
    public IActionResult Edit()
    {
        User model = _db.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User)));
        EditViewModel user = new EditViewModel()
        {
            Email = model.Email,
            UserName = model.UserName,
        };
        return View(user);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(EditViewModel model, IFormFile? uploadedFile)
    {
        User user = _db.Users.FirstOrDefault(u=>u.Id==int.Parse(_userManager.GetUserId(User)));
        var buffer = user.Avatar.Split('=');
        var buffer2 = buffer[buffer.Length - 1].Split('.');
        if (ModelState.IsValid)
        {
            string? path = null;
            if (uploadedFile!=null)
            {
                string newFileName = Path.ChangeExtension($"{model.UserName.Trim()}-ProfileN={int.Parse(buffer2[0])+1}", Path.GetExtension(uploadedFile.FileName));
                path= $"/userImages/" + newFileName.Trim();
                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.Avatar = path != null ? path : user.Avatar;
            user.PasswordHash = model.Password != null ? _userManager.PasswordHasher.HashPassword(user, model.Password) : user.PasswordHash;
            await _userManager.UpdateAsync(user);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Profile", new {userId=user.Id});
        }
        ModelState.AddModelError("", "Something went wrong! Please check all info");
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
                Avatar = path,
                AnswersCount = 0
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Profile", new {userId = user.Id });
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