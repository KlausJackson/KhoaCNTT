
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KhoaCNTT.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<FileResource> Files { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<LecturerSubject> LecturerSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. AdminUsers ---
            modelBuilder.Entity<AdminUser>(e =>
            {
                e.Property(x => x.Username).HasColumnType("VARCHAR(100)").IsRequired();
                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Email).HasColumnType("VARCHAR(255)").IsRequired();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.CreatedAt).HasColumnType("DATETIME2").HasDefaultValueSql("SYSDATETIME()");
                // Check constraint Level >= 1 (EF Core 8+)
                e.ToTable(t => t.HasCheckConstraint("CK_Admin_Level", "[Level] >= 1"));
            });

            // --- 2. Subjects ---
            modelBuilder.Entity<Subject>(e =>
            {
                e.Property(x => x.SubjectCode).HasColumnType("VARCHAR(50)").IsRequired();
                e.HasIndex(x => x.SubjectCode).IsUnique();
                e.Property(x => x.Credits).HasDefaultValue(3);
                e.ToTable(t => t.HasCheckConstraint("CK_Subject_Credits", "[Credits] > 0"));
            });

            // --- 3. Lecturers ---
            modelBuilder.Entity<Lecturer>(e =>
            {
                e.Property(x => x.Email).HasColumnType("VARCHAR(255)").IsRequired();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Birthdate).HasColumnType("DATE");
            });

            // --- 4. News ---
            modelBuilder.Entity<News>(e =>
            {
                e.Property(x => x.Content).HasColumnType("VARCHAR(MAX)");
                e.Property(x => x.NewsType).HasConversion<string>().HasColumnType("VARCHAR(100)"); // Enum -> String
                e.Property(x => x.Status).HasColumnType("VARCHAR(50)").HasDefaultValue("Pending");
                e.Property(x => x.PublishedDate).HasColumnType("DATETIME2");

                e.HasOne(n => n.CreatedBy)
                 .WithMany(a => a.NewsList)
                 .HasForeignKey(n => n.CreatedById)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // --- 5. Comments ---
            modelBuilder.Entity<Comment>(e =>
            {
                e.Property(x => x.MSV).HasColumnType("CHAR(10)");
                e.Property(x => x.CreatedAt).HasColumnType("DATETIME2").HasDefaultValueSql("SYSDATETIME()");
            });

            // --- 6. FileResources ---
            modelBuilder.Entity<FileResource>(e =>
            {
                e.Property(x => x.Permission).HasConversion<string>().HasColumnType("VARCHAR(50)").HasDefaultValue(FilePermission.PublicRead);
                e.Property(x => x.Status).HasConversion<string>().HasColumnType("VARCHAR(50)").HasDefaultValue(FileStatus.Pending);
                e.Property(x => x.CreatedAt).HasColumnType("DATETIME2").HasDefaultValueSql("SYSDATETIME()");

                // Quan hệ Subject
                e.HasOne(f => f.Subject)
                 .WithMany(s => s.Files)
                 .HasForeignKey(f => f.SubjectId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ CreatedBy (Admin)
                e.HasOne(f => f.CreatedBy)
                 .WithMany(a => a.CreatedFiles)
                 .HasForeignKey(f => f.CreatedById)
                 .OnDelete(DeleteBehavior.NoAction); // Tránh vòng lặp Cascade

                // Quan hệ ApprovedBy (Admin)
                e.HasOne(f => f.ApprovedBy)
                 .WithMany(a => a.ApprovedFiles)
                 .HasForeignKey(f => f.ApprovedById)
                 .OnDelete(DeleteBehavior.NoAction); // Tránh vòng lặp Cascade
            });

            // --- 7. LecturerSubjects ---
            modelBuilder.Entity<LecturerSubject>()
                .HasKey(ls => new { ls.LecturerId, ls.SubjectId });

            // set isDeleted mặc định là false cho mọi bảng trừ LecturerSubject
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType != typeof(LecturerSubject))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<bool>("IsDeleted")
                        .HasDefaultValue(false);
                }
            }

            
        }
    }
}