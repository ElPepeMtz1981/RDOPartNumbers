using Microsoft.EntityFrameworkCore;
using RDOXMES.PartNumbers;

namespace RDOXMES.PartNumbers;

public class PartNumbersDbContext : DbContext
{
    public PartNumbersDbContext(DbContextOptions<PartNumbersDbContext> options) : base(options) { }

    public DbSet<PartNumberClass> PartNumbers { get; set; }
}