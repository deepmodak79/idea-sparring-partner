using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
