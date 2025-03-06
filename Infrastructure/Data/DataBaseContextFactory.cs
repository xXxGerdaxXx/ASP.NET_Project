using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Data;

public class DataBaseContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Brodins\\Documents\\ASP_Net.mdf;Integrated Security=True;Connect Timeout=30",
            sqlOptions => sqlOptions.MigrationsAssembly("Infrastructure")
        );

        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Warning)
                      .EnableSensitiveDataLogging(false);

        return new AppDbContext(optionsBuilder.Options);
    }
}
