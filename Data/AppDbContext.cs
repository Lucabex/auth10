using Microsoft.EntityFrameworkCore;
using auth10.Models;

namespace auth10.Data;

public class AppDbContex : DbContext
{
    public AppDbContex(DbContextOptions<AppDbContex> options) : base(options)
    {
        
    }
    public DbSet<User> User{get;set;}
}