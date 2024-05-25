using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace contr8.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Заполните поле Email")]
    [Remote(action: "CheckEmail", controller:"Validation", ErrorMessage = "Этот Email уже занят, попробуйте еще раз!")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Заполните поле Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Заполните поле Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Неправильный Confirm Password, попробуйте еще раз!")]
    public string ConfirmPassword { get; set; }
    [Required(ErrorMessage = "Заполните поле Username")]
    [Remote(action: "CheckUsername", controller:"Validation", ErrorMessage = "Этот UserName уже занят, попробуйте еще раз!")]
    [RegularExpression(@"^\S+(?:\S+)?$", ErrorMessage = "Заполните UserName без пробела!")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Заполните поле Номер Телефона")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Заполните в формате x-цифра: 0 xxx xx xx xx")]
    public string PhoneNumber { get; set; }
}