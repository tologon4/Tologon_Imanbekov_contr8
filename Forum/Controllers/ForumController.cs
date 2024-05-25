using contr8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace contr8.Controllers;

public class ForumController : Controller
{
    private ForumDb _db { get; set; }
    private UserManager<User> _userManager { get; set; }
    private IHttpContextAccessor _httpContextAccessor;


    public ForumController(ForumDb db, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _db = db;
    }

    
    public IActionResult Index()
    {
        return View();
    }
    
    
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        var referrer = _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
        ForumTitle? title = await _db.ForumTitles.FirstOrDefaultAsync(f => f.Id == id);
        if (title != null)
            return View(title);
        return Redirect(referrer);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CreateTitle()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTitle(ForumTitle? model)
    {
        User? user = await _userManager.GetUserAsync(User);
        if (ModelState.IsValid)
        {
            model.UserId = user.Id;
            model.User = user;
            model.CreatedTime = DateTime.UtcNow;
            model.AnswersCount = 0;
            _db.ForumTitles.Add(model);
            user.ForumTitles.Add(model);
            _db.Users.Update(user);
            _db.SaveChanges();
            return RedirectToAction();
        }
        return View(model);
    }
    
    
}