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


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SendAnswer(string? content, int? forumId)
    {
        User? user =  await _db.Users.Include(a => a.Answers).FirstOrDefaultAsync(u => u.Id == int.Parse(_userManager.GetUserId(User)));
        ForumTitle? forum = await _db.ForumTitles.Include(a => a.Answers).FirstOrDefaultAsync(f => f.Id == forumId);
        if (!string.IsNullOrEmpty(content) && forumId.HasValue && forum != null)
        {
            Answer answer = new Answer()
            {
                CreatedTime = DateTime.UtcNow,
                Content = content,
                UserId = user.Id,
                User = user,
                ForumTitleId = forumId
            };
            _db.Answers.Add(answer);
            user.Answers.Add(answer);
            forum.Answers.Add(answer);
            _db.Users.Update(user);
            _db.ForumTitles.Update(forum);
            _db.SaveChanges();
            return Json(new { 
                isSuccess = true,
                AnwserVar = new {
                    UserAvatar = user.Avatar,
                    UserName = user.UserName,
                    CreatedTime = answer.CreatedTime.Value.ToString("dd.mm.yyyy hh:mm:ss"),
                    Content = answer.Content
                } 
            });
        }
        return Json(new {isSuccess = false});
    }
    
    public IActionResult Index()
    {
        var titles = _db.ForumTitles.Include(u => u.User).ToList();
        return View(titles);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        var referrer = _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
        ForumTitle? title = await _db.ForumTitles.Include(u => u.User)
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(f => f.Id == id);
        ViewBag.Answers = _db.Answers.Include(u => u.User).Where(a => title.Answers.Select(a => a.Id).Contains(a.Id)).ToList();
        User? user = await _userManager.GetUserAsync(User);
        ViewBag.CurrentUser = user;
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