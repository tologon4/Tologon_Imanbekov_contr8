using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace contr8.Models;

public class ForumDb : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<User> Users { get; set; }
    public DbSet<ForumTitle> ForumTitles { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public ForumDb(DbContextOptions<ForumDb> options) : base(options) { }

}