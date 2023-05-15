using Microsoft.EntityFrameworkCore;

namespace SaginPortal.Context;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        
    }
}