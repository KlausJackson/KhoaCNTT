
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Entities.FileEntities;
using KhoaCNTT.Domain.Entities.NewsEntities;
using KhoaCNTT.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KhoaCNTT.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- ADMIN & SUBJECT ---
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<LecturerSubject> LecturerSubjects { get; set; }

        // --- FILE MODULE ---
        public DbSet<FileEntity> FileEntities { get; set; }
        public DbSet<FileResource> FileResources { get; set; }
        public DbSet<FileRequest> FileRequests { get; set; }
        public DbSet<FileApproval> FileApprovals { get; set; }

        // --- NEWS MODULE ---
        public DbSet<News> News { get; set; }
        public DbSet<NewsResource> NewsResources { get; set; }
        public DbSet<NewsRequest> NewsRequests { get; set; }
        public DbSet<NewsApproval> NewsApprovals { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // --- 1. Admin ---
            modelBuilder.Entity<Admin>(e =>
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


            // --- 4. Comments ---
            modelBuilder.Entity<Comment>(e =>
            {
                e.Property(x => x.MSV).HasColumnType("CHAR(10)");
                e.Property(x => x.CreatedAt).HasColumnType("DATETIME2").HasDefaultValueSql("SYSDATETIME()");
            });

            // --- 5. LecturerSubjects ---
            modelBuilder.Entity<LecturerSubject>()
                .HasKey(ls => new { ls.LecturerId, ls.SubjectCode });

            modelBuilder.Entity<LecturerSubject>()
                .HasOne(ls => ls.Lecturer)
                .WithMany(l => l.LecturerSubjects)
                .HasForeignKey(ls => ls.LecturerId);

            modelBuilder.Entity<LecturerSubject>()
                .HasOne(ls => ls.Subject)
                .WithMany(s => s.LecturerSubjects)
                .HasForeignKey(ls => ls.SubjectCode);

            //// set isDeleted mặc định là false cho mọi bảng trừ LecturerSubject
            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    if (entityType.ClrType != typeof(LecturerSubject))
            //    {
            //        modelBuilder.Entity(entityType.ClrType)
            //            .Property<bool>("IsDeleted")
            //            .HasDefaultValue(false);
            //    }
            //
        }
    }
}