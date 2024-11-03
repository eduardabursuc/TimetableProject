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
                entity.Property(e => e.Event).HasColumnName("event").HasMaxLength(200);

                // Foreign key relationships
                entity.HasOne<Professor>()
                    .WithMany()
                    .HasForeignKey(e => e.ProfessorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne<Room>()
                    .WithMany()
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne<Group>()
                    .WithMany()
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne<Room>()
                    .WithMany()
                    .HasForeignKey(e => e.WantedRoomId)
                    .IsRequired(false);

                entity.HasOne<Timeslot>()
                    .WithMany()
                    .HasForeignKey(e => e.WantedTimeslotId)
                    .OnDelete(DeleteBehavior.Restrict) // Prevent cascading delete
                    .IsRequired(false);

                // Many-to-many relationship with Timeslot
                entity.HasMany(e => e.Timeslots)
                    .WithMany(t => t.Constraints)
                    .UsingEntity<Dictionary<string, object>>(
                        "ConstraintTimeslot",
                        j => j.HasOne<Timeslot>().WithMany().HasForeignKey("TimeslotId"),
                        j => j.HasOne<Constraint>().WithMany().HasForeignKey("ConstraintId"));
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.ToTable("professors");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                // Many-to-many relationship with Course
                entity.HasMany(e => e.Courses).WithMany(c => c.Professors);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("courses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

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
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("rooms");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Capacity).IsRequired();
            });

            modelBuilder.Entity<Timeslot>(entity =>
            {
                entity.ToTable("timeslots");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Day).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Time).IsRequired().HasMaxLength(50);

                // Many-to-many relationship with Constraint
                entity.HasMany(t => t.Constraints)
                    .WithMany(c => c.Timeslots)
                    .UsingEntity<Dictionary<string, object>>(
                        "ConstraintTimeslot",
                        j => j.HasOne<Constraint>().WithMany().HasForeignKey("ConstraintId"),
                        j => j.HasOne<Timeslot>().WithMany().HasForeignKey("TimeslotId"));
            });
        }
    }
}
