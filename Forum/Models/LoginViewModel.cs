using System.ComponentModel.DataAnnotations;

namespace contr8.Models;

public class LoginViewModel
{
    public string LoginValue { get; set; }
    public string Password { get; set; }
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}