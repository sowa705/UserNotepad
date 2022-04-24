using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UserNotepad.Models;

namespace UserNotepad
{
    public class NotepadDbContext : DbContext
    {
        public NotepadDbContext(DbContextOptions<NotepadDbContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(b => b.Attributes)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = -1,
                Name = "Test",
                Surname = "User",
                Gender = Gender.Male,
                Birthday = new DateTime(1980, 1, 1),
                Attributes = new Dictionary<string, string>() {
                    { "Attrib1", "Value" }
                }
            });

        }

        public DbSet<User> Users { get; set; }
    }
}
