using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace contr8.Models;

public class EditViewModel
{
    [Required(ErrorMessage = "Please provide an Email")]
    [Remote(action: "EditCheckEmail", controller:"Validation", ErrorMessage = "This email is busy! Try again")]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Confirm Password is invalid! Try again")]
    public string? ConfirmPassword { get; set; }
    [Required(ErrorMessage = "Please provide a Username")]
    [Remote(action: "EditCheckUsername", controller:"Validation", ErrorMessage = "This username is busy! Try again")]
    [RegularExpression(@"^\S+(?:\S+)?$", ErrorMessage = "Provide with no space!")]
    public string UserName { get; set; }
}