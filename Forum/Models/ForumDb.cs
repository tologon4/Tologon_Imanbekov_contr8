using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace contr8.Models;

public class ForumDb : IdentityDbContext<User, IdentityRole<int>, int>
{
    
}