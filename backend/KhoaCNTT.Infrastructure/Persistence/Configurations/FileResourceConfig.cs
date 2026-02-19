
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhoaCNTT.Infrastructure.Persistence.Configurations
{
    public class FileResourceConfig : IEntityTypeConfiguration<FileResource>
    {
        public void Configure(EntityTypeBuilder<FileResource> builder)
        {
            // Tên bảng
            builder.ToTable("FileResources");

            builder.Property(f => f.Title).HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(f => f.FileName).HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(f => f.FilePath).HasColumnType("VARCHAR(500)").IsRequired();
            builder.Property(f => f.ContentType).HasColumnType("VARCHAR(100)").IsRequired();

            // Enum lưu dạng String
            builder.Property(f => f.Status).HasConversion<string>().HasColumnType("VARCHAR(50)").HasDefaultValue(FileStatus.Pending);
            builder.Property(f => f.Permission).HasConversion<string>().HasColumnType("VARCHAR(50)").HasDefaultValue(FilePermission.PublicRead);

            // Mapping tên cột khóa ngoại (CreatedById -> CreatedBy)
            builder.Property(f => f.CreatedById).HasColumnName("CreatedBy");
            builder.Property(f => f.ApprovedById).HasColumnName("ApprovedBy");

            // Quan hệ Subject
            builder.HasOne(f => f.Subject)
                   .WithMany(s => s.Files)
                   .HasForeignKey(f => f.SubjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Admin
            builder.HasOne(f => f.CreatedBy)
                   .WithMany(a => a.CreatedFiles)
                   .HasForeignKey(f => f.CreatedById)
                   .OnDelete(DeleteBehavior.NoAction); // Tránh vòng lặp cascade
        }
    }
}