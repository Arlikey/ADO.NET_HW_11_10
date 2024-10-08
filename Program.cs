using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            /*db.Users.AddRange([
                new User { Username = "Test 1", Password = "1234"},
                new User { Username = "Arlikey", Password = "qwerty"},
                new User { Username = "Test 3", Password = "5678"},
            ]);
            db.SaveChanges();

            db.Settings.AddRange([
                new Settings { UserId = 1},
                new Settings { UserId = 2, Theme = "Dark", IsTwoFactorEnabled = true},
                new Settings { UserId = 3},
            ]);
            db.SaveChanges();*/

            var user = db.Users.FirstOrDefault(user => user.Id == 2);
            db.Entry(user).Reference(u => u.Settings).Load();

            Console.WriteLine($"User: {user.Username}");
            Console.WriteLine($"""
                Settings:   name:        {user.Settings.Name}
                            description: {user.Settings.Description}
                            theme:       {user.Settings.Theme}
                            language:    {user.Settings.Language}
                            date format: {user.Settings.DateFormat}
                """);

            var delete = db.Users.FirstOrDefault(user => user.Id == 3);
            if (delete != null)
            {
                db.Users.Remove(delete);
                db.SaveChanges();
            }
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public Settings Settings { get; set; }
}
public class Settings
{
    public int Id { get; set; }
    public string Name { get; set; } = "default";
    public string Description { get; set; } = "settings settled by default";
    public string Theme { get; set; } = "Light";
    public string Language { get; set; } = "en";
    public bool IsTwoFactorEnabled { get; set; } = false;
    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public int UserId { get; set; }
    public User User { get; set; }
}
public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public ApplicationContext() => Database.EnsureCreated();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Settings)
            .WithOne(s => s.User)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EFCoreDB;Trusted_Connection=True;");
    }
}