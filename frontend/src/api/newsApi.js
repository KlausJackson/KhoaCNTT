import axiosClient from "./axiosClient";

// ── Token helper ──────────────────────────────────────────────
// Admin token được cache lại ngay khi đọc được JWT hợp lệ
// để tránh bị ghi đè bởi token GUID của sinh viên
const isJwt = (t) => typeof t === "string" && t.startsWith("eyJ");

const getAdminToken = () => {
  // Ưu tiên key riêng "adminToken" (nếu có)
  const cached = localStorage.getItem("adminToken");
  if (cached) return cached;
  // Fallback: nếu "token" hiện tại là JWT thì dùng luôn và cache lại
  const current = localStorage.getItem("token");
  if (isJwt(current)) {
    localStorage.setItem("adminToken", current);
    return current;
  }
  return current;
};

const adminCfg = () => ({
  headers: { Authorization: `Bearer ${getAdminToken()}` },
});

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
  create: (data) =>
    axiosClient.post("/News/requests/create", toPayload(data), adminCfg()),

  update: (id, data) =>
    axiosClient.post(
      "/News/requests/update",
      toPayload({ ...data, TargetNewsID: id }),
      adminCfg(),
    ),

  delete: (id) => axiosClient.delete(`/News/${id}`, adminCfg()),

  // ── Admin - Duyệt bài ─────────────────────────────────
  getPendingList: () => axiosClient.get("/News/requests/pending", adminCfg()),

  approve: (id, data) =>
    axiosClient.put(
      `/News/requests/${id}/approve`,
      {
        ...data,
        decision: approvalDecisionMap[data.decision] ?? data.decision,
      },
      adminCfg(),
    ),

  // ── Bình luận ─────────────────────────────────────────
  getComments: (newsId) => axiosClient.get(`/News/${newsId}/comments`),

  postComment: (newsId, data) => {
    const role = localStorage.getItem("role");
    const msv =
      role === "student" ? localStorage.getItem("username") || "" : "";
    const studentName = msv || "Sinh viên";
    return axiosClient.post(`/News/${newsId}/comments`, {
      ...data,
      msv,
      studentName,
    });
  },

  deleteComment: (commentId) =>
    axiosClient.delete(`/News/comments/${commentId}`, adminCfg()),
};

export default newsApi;
