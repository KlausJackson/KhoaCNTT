
using KhoaCNTT.Domain.Entities;
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

            // Cấu hình các trường
            builder.Property(f => f.Title).HasMaxLength(250).IsRequired();
            builder.Property(f => f.FileName).HasMaxLength(500).IsRequired();
            builder.Property(f => f.FilePath).HasMaxLength(500).IsRequired();

            // Cấu hình quan hệ
            builder.HasIndex(f => f.SubjectCode);
        }
    }
}