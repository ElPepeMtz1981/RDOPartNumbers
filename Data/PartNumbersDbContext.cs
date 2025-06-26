using Microsoft.EntityFrameworkCore;
using PartNumbers.Models;

namespace PartNumbers.Data;

public class PartNumbersDbContext : DbContext
{
    public PartNumbersDbContext(DbContextOptions<PartNumbersDbContext> options) : base(options) { }

    public DbSet<PartNumberClass> PartNumbers { get; set; }
}