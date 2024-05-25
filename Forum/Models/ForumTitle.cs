using System.ComponentModel.DataAnnotations;

namespace contr8.Models;

public class ForumTitle
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Заполните поле Title!")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Заполните поле Content!")]
    public string Content { get; set; }
    public DateTime? CreatedTime { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Answer>? Answers { get; set; }
    public int? AnswersCount { get; set; }
}