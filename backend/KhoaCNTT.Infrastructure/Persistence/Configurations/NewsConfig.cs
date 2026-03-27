using KhoaCNTT.Domain.Entities.NewsEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KhoaCNTT.Infrastructure.Persistence.Configurations
{
    public class NewsConfiguration :
        IEntityTypeConfiguration<News>,
        IEntityTypeConfiguration<NewsResource>,
        IEntityTypeConfiguration<NewsRequest>,
        IEntityTypeConfiguration<NewsApproval>,
        IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("News");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("NewsID");
            builder.Property(x => x.Title).HasColumnType("nvarchar(255)").IsRequired();
            builder.Property(x => x.NewsType).HasConversion<string>().HasColumnType("varchar(50)");

            // FK
            builder.HasOne(x => x.CurrentResource).WithMany().HasForeignKey(x => x.CurrentResourceId).OnDelete(DeleteBehavior.Restrict);

            // ĐÃ SỬA: Gọi Navigation Property "Admin", map vào Foreign Key "CreatedBy"
            builder.HasOne(x => x.Admin).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        }

        public void Configure(EntityTypeBuilder<NewsResource> builder)
        {
            builder.ToTable("NewsResource");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("NewsResourceID");

            // ĐÃ ĐIỀU CHỈNH: Khớp với DB Design (nvarchar(255))
            builder.Property(x => x.Content).HasColumnType("nvarchar(255)").IsRequired();

            // FK
            builder.HasOne(x => x.Admin).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        }

        public void Configure(EntityTypeBuilder<NewsRequest> builder)
        {
            builder.ToTable("NewsRequest");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("NewsRequestID");
            builder.Property(x => x.Title).HasColumnType("nvarchar(255)").IsRequired();

            builder.Property(x => x.RequestType).HasConversion<string>().HasColumnType("varchar(50)");
            builder.Property(x => x.NewsType).HasConversion<string>().HasColumnType("varchar(50)");

            // FK
            builder.HasOne(x => x.TargetNews).WithMany().HasForeignKey(x => x.TargetNewsId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.NewResource).WithMany().HasForeignKey(x => x.NewResourceId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.OldResource).WithMany().HasForeignKey(x => x.OldResourceId).OnDelete(DeleteBehavior.Restrict);
        }

        public void Configure(EntityTypeBuilder<NewsApproval> builder)
        {
            builder.ToTable("NewsApproval");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("NewsApprovalID");
            builder.Property(x => x.Decision).HasConversion<string>().HasColumnType("varchar(50)");
            builder.Property(x => x.Reason).HasColumnType("nvarchar(255)");

            // FK
            builder.HasOne(x => x.NewsRequest).WithMany().HasForeignKey(x => x.NewsRequestId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Admin).WithMany().HasForeignKey(x => x.ApproverId).OnDelete(DeleteBehavior.Restrict);
        }

        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("CommentID");
            builder.Property(x => x.MSV).HasColumnType("char(20)").IsRequired();
            builder.Property(x => x.StudentName).HasColumnType("nvarchar(100)").IsRequired();
            builder.Property(x => x.Content).HasColumnType("nvarchar(500)").IsRequired();

            // FK
            builder.HasOne(x => x.News).WithMany(n => n.Comments).HasForeignKey(x => x.NewsId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}