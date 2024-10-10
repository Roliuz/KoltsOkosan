using Microsoft.EntityFrameworkCore;
using UjGyakorlas.Models;

public class KiadasokDbContext : DbContext
{
    public DbSet<Kiadas> Kiadasok { get; set; }

    // Konstruktort definiálsz, amely elfogadja a DbContextOptions paramétert
    public KiadasokDbContext(DbContextOptions<KiadasokDbContext> options) : base(options)
    {
    }

    // Az OnConfiguring metódust nem kell használnod, ha a szolgáltatásokat a Startupban konfigurálod
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=kiadasok.db");
    }
}
