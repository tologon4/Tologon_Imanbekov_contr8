namespace contr8.Models;

public class Answer
{
    public int Id { get; set; }
    public DateTime? CreatedTime { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int? ForumTitleId { get; set; }
    public ForumTitle? ForumTitle { get; set; }
    
    
}