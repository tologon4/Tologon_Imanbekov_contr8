namespace contr8.Models;

public class AnswerIndexViewModel
{
    public IEnumerable<Answer> Answers { get; set; }
    public ForumTitle ForumTitle { get; set; }
    public PageViewModel PageViewModel { get; set; }
}