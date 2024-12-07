using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{ 
      public DbSet<User> Users { get; init; }
    public DbSet<Constraint> Constraints { get; init; }
    public DbSet<Professor> Professors { get; init; }
    public DbSet<Course> Courses { get; init; }
    public DbSet<Group> Groups { get; init; }
    public DbSet<Room> Rooms { get; init; }
    public DbSet<Timetable> Timetables { get; init; }

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
                  .WithMany()
                  .HasForeignKey(e => e.UserEmail)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Group ownership
        modelBuilder.Entity<Group>(entity =>
        {
              entity.ToTable("groups");
              entity.HasKey(e => e.Id); // Use Id as primary key

              entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

              entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

              entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserEmail)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Course ownership
        modelBuilder.Entity<Course>(entity =>
        {
              entity.ToTable("courses");
              entity.HasKey(e => e.Id); // Use Id as primary key

              entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

              entity.Property(e => e.CourseName)
                    .IsRequired()
                    .HasMaxLength(200);

              entity.Property(e => e.Credits).IsRequired();
              entity.Property(e => e.Package).IsRequired().HasMaxLength(200);
              entity.Property(e => e.Semester).IsRequired();
              entity.Property(e => e.Level).IsRequired().HasMaxLength(100);

              entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserEmail)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Room ownership
        modelBuilder.Entity<Room>(entity =>
        {
              entity.ToTable("rooms");
              entity.HasKey(e => e.Id);

              entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .ValueGeneratedOnAdd();

              entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

              entity.Property(e => e.Capacity).IsRequired();

              entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserEmail)
                    .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure Timetable
        modelBuilder.Entity<Timetable>(entity =>
        {
            entity.ToTable("timetables");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .ValueGeneratedNever();

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(e => e.UserEmail)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Configure Events within Timetable
            entity.OwnsMany(e => e.Events, eventEntity =>
            {
                eventEntity.ToTable("events");
                eventEntity.HasKey(e => e.Id );

                eventEntity.Property(e => e.GroupId).IsRequired();
                eventEntity.Property(e => e.EventName).IsRequired();
                eventEntity.Property(e => e.CourseId).IsRequired();
                eventEntity.Property(e => e.RoomId).IsRequired();
                eventEntity.Property(e => e.ProfessorId).IsRequired();
                eventEntity.Property(e => e.Duration);

                // Configure Timeslot within Event
                eventEntity.OwnsOne(e => e.Timeslot, timeslot =>
                {
                    timeslot.Property(t => t.Day).IsRequired().HasMaxLength(50);
                    timeslot.Property(t => t.Time).IsRequired().HasMaxLength(50);
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
            entity.Property(e => e.CourseId).HasColumnType("uuid").IsRequired(false);
            entity.Property(e => e.RoomId).HasColumnType("uuid").IsRequired(false);
            entity.Property(e => e.WantedRoomId).HasColumnType("uuid").IsRequired(false);
            entity.Property(e => e.GroupId).HasColumnType("uuid").IsRequired(false);
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
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);

            entity.HasOne<Room>()
                  .WithMany()
                  .HasForeignKey(e => e.RoomId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);

            entity.HasOne<Group>()
                  .WithMany()
                  .HasForeignKey(e => e.GroupId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);

            entity.HasOne<Room>()
                  .WithMany()
                  .HasForeignKey(e => e.WantedRoomId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);
        });
    }
}
