using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.DTOs.News;

// ── Responses ──────────────────────────────────────────────────

public class NewsResponse
{
    public int NewsID { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;   // from CurrentNewsResource
    public int ViewCount { get; set; }
    public NewsType NewsType { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class NewsResourceResponse
{
    public int NewsResourceID { get; set; }
    public string Content { get; set; } = null!;
    public int CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class NewsRequestResponse
{
    public int NewsRequestID { get; set; }
    public int? TargetNewsID { get; set; }
    public string Title { get; set; } = null!;
    public NewsType NewsType { get; set; }
    public string Content { get; set; } = null!;
    public RequestType RequestType { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class NewsApprovalResponse
{
    public int NewsApprovalID { get; set; }
    public int ApproverID { get; set; }
    public int NewsRequestID { get; set; }
    public ApprovalDecision Decision { get; set; }
    public string? RejectReason { get; set; }
    public DateTime? CreatedAt { get; set; }
}

// ── Requests ────────────────────────────────────────────────────

/// <summary>
/// Bước 1 - Lecturer/Admin tạo nội dung mới (NewsResource) và gửi yêu cầu đăng tin
/// </summary>
public class CreateNewsRequest
{
    public string Title { get; set; } = null!;
    public NewsType NewsType { get; set; }
    /// <summary>Nội dung bài viết (sẽ tạo NewsResource mới)</summary>
    public string Content { get; set; } = null!;
}

/// <summary>
/// Bước 1 - Gửi yêu cầu sửa tin tức đã tồn tại
/// </summary>
public class UpdateNewsRequest
{
    /// <summary>ID của News cần sửa</summary>
    public int TargetNewsID { get; set; }
    public string Title { get; set; } = null!;
    public NewsType NewsType { get; set; }
    /// <summary>Nội dung mới (sẽ tạo NewsResource mới)</summary>
    public string Content { get; set; } = null!;
}

/// <summary>
/// Admin duyệt hoặc từ chối yêu cầu
/// </summary>
public class ApproveNewsRequest
{
    public int NewsRequestID { get; set; }
    public ApprovalDecision Decision { get; set; }
    public string? RejectReason { get; set; }
}

/// <summary>
/// Admin xóa tin tức (hard delete)
/// </summary>
public class DeleteNewsRequest
{
    public int NewsID { get; set; }
}