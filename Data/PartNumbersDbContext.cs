using Microsoft.EntityFrameworkCore;
using RDOXMES.Models;

namespace RDOXMES.Data;

public class PartNumbersDbContext : DbContext
{
    public PartNumbersDbContext(DbContextOptions<PartNumbersDbContext> options) : base(options) { }

    public DbSet<PartNumberClass> PartNumbers { get; set; }
}