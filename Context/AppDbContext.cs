using Microsoft.EntityFrameworkCore;
using SaginPortal.Models;

namespace SaginPortal.Context;

public class AppDbContext : DbContext {
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        
    }
    public DbSet<UserModel> User { get; set; }

}