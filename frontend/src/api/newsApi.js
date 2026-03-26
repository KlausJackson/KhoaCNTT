import axiosClient from "./axiosClient";

// Khớp với C# enum ApprovalDecision { Approved = 0, Rejected = 1 }
const approvalDecisionMap = {
  Approved: 0,
  Rejected: 1,
};

// Chuẩn hóa payload về PascalCase để C# [FromBody] binding luôn nhận đúng
const toPayload = (form) => ({
  Title: form.title ?? form.Title ?? "",
  NewsType: form.newsType ?? form.NewsType ?? "",
  Content: form.content ?? form.Content ?? "",
  ResourceContent: form.resourceContent ?? form.ResourceContent ?? "",
  ...(form.TargetNewsID !== undefined && { TargetNewsID: form.TargetNewsID }),
});

const newsApi = {
  // ── Public ────────────────────────────────────────────
  search: (params) => axiosClient.get("/News", { params }),

  getById: (id) => axiosClient.get(`/News/${id}`),

  // ── Admin - Tạo/Sửa/Xóa ──────────────────────────────
  create: (data) => axiosClient.post("/News/requests/create", toPayload(data)),

  update: (id, data) =>
    axiosClient.post(
      "/News/requests/update",
      toPayload({ ...data, TargetNewsID: id }),
    ),

  delete: (id) => axiosClient.delete(`/News/${id}`),

  // ── Admin - Duyệt bài ─────────────────────────────────
  getPendingList: () => axiosClient.get("/News/requests/pending"),

  approve: (id, data) =>
    axiosClient.put(`/News/requests/${id}/approve`, {
      ...data,
      decision: approvalDecisionMap[data.decision] ?? data.decision,
    }),

  // ── Bình luận ─────────────────────────────────────────
  // GET /api/News/{newsId}/comments — public
  getComments: (newsId) => axiosClient.get(`/News/${newsId}/comments`),

  // POST /api/News/{newsId}/comments — sinh viên đăng nhập
  // Sinh viên login bằng MSV → localStorage "username" chính là MSV
  postComment: (newsId, data) => {
    const msv = localStorage.getItem("username") || "";
    const studentName = localStorage.getItem("username") || "Sinh viên";
    return axiosClient.post(`/News/${newsId}/comments`, {
      ...data,
      msv,
      studentName,
    });
  },

  // DELETE /api/News/comments/{commentId} — admin cấp 1,2,3
  deleteComment: (commentId) =>
    axiosClient.delete(`/News/comments/${commentId}`),
};

export default newsApi;
