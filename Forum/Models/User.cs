using Microsoft.AspNetCore.Identity;

namespace contr8.Models;

public class User : IdentityUser<int>
{
    public string? Avatar { get; set; }
    public ICollection<ForumTitle>? ForumTitles { get; set; }
    public ICollection<Answer>? Answers { get; set; }
    public int? AnswersCount { get; set; }
}