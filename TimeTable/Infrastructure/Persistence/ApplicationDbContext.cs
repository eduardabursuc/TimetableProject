using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Constraint> Constraints { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Timeslot> Timeslots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Constraint>(entity =>
            {
                entity.ToTable("constraints");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Event).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.ProfessorId).IsRequired(false);
                entity.Property(e => e.CourseName).IsRequired(false);
                entity.Property(e => e.RoomName).IsRequired(false);
                entity.Property(e => e.WantedRoomName).IsRequired(false);
                entity.Property(e => e.GroupName).IsRequired(false);
                entity.Property(e => e.WantedDay).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.WantedTime).HasMaxLength(50).IsRequired(false);

                // Foreign key relationships
                entity.HasOne<Professor>()
                    .WithMany()
                    .HasForeignKey(e => e.ProfessorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                entity.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(e => e.CourseName)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                entity.HasOne<Room>()
                    .WithMany()
                    .HasForeignKey(e => e.RoomName)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                entity.HasOne<Group>()
                    .WithMany()
                    .HasForeignKey(e => e.GroupName)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                entity.HasOne<Room>()
                    .WithMany()
                    .HasForeignKey(e => e.WantedRoomName)
                    .IsRequired(false);
                
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.ToTable("professors");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                
                // Many-to-many relationship with Course
                entity.HasMany(e => e.Courses).WithMany(c => c.Professors);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("courses");
                entity.HasKey(e => e.CourseName);
                entity.Property(e => e.CourseName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Credits).IsRequired();
                entity.Property(e => e.Package).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Semester).IsRequired();
                entity.Property(e => e.Level).IsRequired().HasMaxLength(100);

                // Many-to-many relationship with Professor
                entity.HasMany(e => e.Professors).WithMany(p => p.Courses);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("groups");
                entity.HasKey(e => e.Name);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("rooms");
                entity.HasKey(e => e.Name);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Capacity).IsRequired();
            });

            modelBuilder.Entity<Timeslot>(entity =>
            {
                entity.ToTable("timeslots");
                entity.HasKey(e => new { e.Time, e.Day });
                entity.Property(e => e.Day).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Time).IsRequired().HasMaxLength(50);
            });
        }
    }
}
