using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; init; }
    public DbSet<Constraint> Constraints { get; init; }
    public DbSet<Professor> Professors { get; init; }
    public DbSet<Course> Courses { get; init; }
    public DbSet<Group> Groups { get; init; }
    public DbSet<Room> Rooms { get; init; }
    public DbSet<Timetable> Timetables { get; init; }
    public DbSet<Timeslot> Timeslots { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure PostgreSQL UUID extension
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Email); // Email as primary key

            entity.Property(e => e.Email)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Password)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.AccountType)
                  .IsRequired()
                  .HasMaxLength(50);
        });

        // Configure Professor ownership
        modelBuilder.Entity<Professor>(entity =>
        {
            entity.ToTable("professors");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnType("uuid")
                  .HasDefaultValueSql("uuid_generate_v4()")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.HasOne<User>()
                  .WithMany(u => u.Professors)
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Course ownership
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses");
            entity.HasKey(e => e.CourseName);

            entity.Property(e => e.CourseName)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Credits).IsRequired();
            entity.Property(e => e.Package).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Semester).IsRequired();
            entity.Property(e => e.Level).IsRequired().HasMaxLength(100);

            entity.HasOne<User>()
                  .WithMany(u => u.Courses)
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Group ownership
        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("groups");
            entity.HasKey(e => e.Name);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.HasOne<User>()
                  .WithMany(u => u.Groups)
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Room ownership
        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("rooms");
            entity.HasKey(e => e.Name);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Capacity).IsRequired();

            entity.HasOne<User>()
                  .WithMany(u => u.Rooms)
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Timetable ownership
        modelBuilder.Entity<Timetable>(entity =>
        {
            entity.ToTable("timetables");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne<User>()
                  .WithMany(u => u.Timetables)
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.OwnsMany(e => e.Timeslots, timeslot =>
            {
                timeslot.ToTable("timeslots");
                timeslot.HasKey(e => new { e.TimetableId, e.Time, e.Day, e.RoomName });

                timeslot.Property(e => e.TimetableId).IsRequired();
                timeslot.Property(e => e.Day).IsRequired();
                timeslot.Property(e => e.Time).IsRequired();
                timeslot.Property(e => e.RoomName).IsRequired();

                timeslot.OwnsOne(e => e.Event, e =>
                {
                    e.Property(ev => ev.Group).HasColumnName("Group");
                    e.Property(ev => ev.EventName).HasColumnName("EventName");
                    e.Property(ev => ev.CourseName).HasColumnName("CourseName");
                    e.Property(ev => ev.CourseCredits).HasColumnName("CourseCredits");
                    e.Property(ev => ev.CoursePackage).HasColumnName("CoursePackage");
                    e.Property(ev => ev.ProfessorId).HasColumnName("ProfessorId");
                    e.Property(ev => ev.ProfessorName).HasColumnName("ProfessorName");
                    e.Property(ev => ev.WeekEvenness).HasColumnName("WeekEvenness");
                });
            });
        });

        // Configure Constraints (Unchanged from your code)
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
            entity.Property(e => e.ProfessorId).HasColumnType("uuid").IsRequired(false);
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
    }
}
