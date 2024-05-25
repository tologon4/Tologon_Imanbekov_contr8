using Microsoft.AspNetCore.Identity;

namespace contr8.Models;

public class User : IdentityUser<int>
{
    public string? Avatar { get; set; }
}