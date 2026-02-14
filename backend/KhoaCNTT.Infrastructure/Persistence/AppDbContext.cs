
using KhoaCNTT.Domain.Entities;
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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Cấu hình khóa chính phức hợp cho bảng trung gian LecturerSubject
            modelBuilder.Entity<LecturerSubject>()
                .HasKey(ls => new { ls.LecturerId, ls.SubjectId });

            // Cấu hình quan hệ Many-to-Many
            modelBuilder.Entity<LecturerSubject>()
                .HasOne(ls => ls.Lecturer)
                .WithMany(l => l.LecturerSubjects)
                .HasForeignKey(ls => ls.LecturerId);

            modelBuilder.Entity<LecturerSubject>()
                .HasOne(ls => ls.Subject)
                .WithMany(s => s.LecturerSubjects)
                .HasForeignKey(ls => ls.SubjectId);

            // Cấu hình quan hệ News - Comment (1 News có nhiều Comment)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.News)
                .WithMany(n => n.Comments)
                .HasForeignKey(c => c.NewsId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa News thì xóa luôn Comment


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Tìm tất cả các bảng có thuộc tính 'CreatedAt'
                var createdAtProperty = entityType.FindProperty("CreatedAt");
                if (createdAtProperty != null && createdAtProperty.ClrType == typeof(DateTime))
                {
                    createdAtProperty.SetDefaultValueSql("GETDATE()");
                }

                // Tìm tất cả các bảng có thuộc tính 'IsDeleted'
                var isDeletedProperty = entityType.FindProperty("IsDeleted");
                if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
                {
                    isDeletedProperty.SetDefaultValue(false);
                }
            }

        }
    }
}