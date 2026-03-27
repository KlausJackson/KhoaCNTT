import React, { useState, useEffect, useRef } from "react";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import newsApi from "../../api/newsApi";
import { newsTypeLabel, newsTypeColor } from "../../constants/news";
import { formatDateTime, timeAgo } from "../../helpers/newsHelpers";
import PopupMessage from "../../components/parts/PopupMessage";
import Button from "../../components/parts/Button";

// ── Popover xóa bình luận (hiện tại vị trí click) ──────────────
const CommentPopover = ({ x, y, onDelete, onClose }) => {
  const ref = useRef(null);

  useEffect(() => {
    const handler = (e) => {
      if (ref.current && !ref.current.contains(e.target)) onClose();
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, [onClose]);

  return (
    <div
      ref={ref}
      style={{ position: "fixed", top: y, left: x, zIndex: 1000 }}
      className="bg-white border border-gray-200 rounded-xl shadow-xl py-1 min-w-[160px]"
    >
      <button
        onClick={onDelete}
        className="w-full text-left px-4 py-2.5 text-sm text-red-500 hover:bg-red-50 transition flex items-center gap-2"
      >
        🗑️ Xóa bình luận
      </button>
    </div>
  );
};

// ── Modal xác nhận xóa bình luận ────────────────────────────────
const ConfirmDeleteModal = ({ onConfirm, onCancel }) => (
  <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
    <div className="bg-white rounded-2xl p-8 max-w-sm w-full mx-4 text-center shadow-2xl">
      <div className="w-16 h-16 bg-red-50 rounded-full flex items-center justify-center mx-auto mb-4 text-3xl">
        🗑️
      </div>
      <h3 className="text-xl font-bold text-gray-900 mb-2">Xóa bình luận</h3>
      <p className="text-gray-500 text-sm mb-6">
        Bạn có chắc chắn muốn xóa bình luận này không? Hành động này không thể
        hoàn tác.
      </p>
      <div className="flex gap-3">
        <button
          onClick={onCancel}
          className="flex-1 py-3 border border-gray-200 rounded-xl font-medium text-gray-700 hover:bg-gray-50 transition"
        >
          Hủy bỏ
        </button>
        <button
          onClick={onConfirm}
          className="flex-1 py-3 bg-red-500 text-white rounded-xl font-medium hover:bg-red-600 transition"
        >
          Đồng ý
        </button>
      </div>
    </div>
  </div>
);

// ── Component chính ─────────────────────────────────────────────
const NewsDetail = () => {
  const { id } = useParams();
  const location = useLocation();
  const navigate = useNavigate();

  const [popup, setPopup] = useState(null);
  const [news, setNews] = useState(location.state || null);
  const [relatedNews, setRelatedNews] = useState([]);
  const [comments, setComments] = useState([]);
  const [commentText, setCommentText] = useState("");
  const [error, setError] = useState(null);

  // Popover state
  const [popover, setPopover] = useState(null); // { x, y, commentId }
  // Modal xác nhận xóa
  const [confirmDelete, setConfirmDelete] = useState(null); // commentId

  // Lấy role từ localStorage để kiểm tra có phải admin không
  const role = localStorage.getItem("role"); // "Admin" | "Student" | null
  const isAdmin = role === "Admin";

  const hasLoaded = useRef(false);

  // Load bài viết
  useEffect(() => {
    if (!id || hasLoaded.current) return;
    const loadNews = async () => {
      try {
        hasLoaded.current = true;
        const data = await newsApi.getById(id);
        setNews(data);
      } catch (err) {
        console.error(err);
        setError("Không tìm thấy bài viết");
      }
    };
    loadNews();
  }, [id]);

  // Load bình luận
  useEffect(() => {
    const loadComments = async () => {
      try {
        const data = await newsApi.getComments(id);
        setComments(data);
      } catch (err) {
        console.log("Không load được bình luận", err);
      }
    };
    if (id) loadComments();
  }, [id]);

  // Load tin liên quan
  useEffect(() => {
    if (!news?.newsType) return;
    newsApi
      .search({ newsType: news.newsType, pageSize: 5 })
      .then((res) => {
        const data = res.items ?? res;
        setRelatedNews(data.filter((n) => n.newsID !== Number(id)).slice(0, 3));
      })
      .catch(() => {});
  }, [id, news?.newsType]);

  // Popup tự đóng
  useEffect(() => {
    if (!popup) return;
    const t = setTimeout(() => setPopup(null), 3000);
    return () => clearTimeout(t);
  }, [popup]);

  // ── Gửi bình luận ───────────────────────────────────
  const handleComment = async () => {
    if (!commentText.trim()) {
      setPopup("Nội dung bình luận không được để trống.");
      return;
    }
    try {
      await newsApi.postComment(id, { content: commentText });
      setCommentText("");
      const updatedComments = await newsApi.getComments(id);
      setComments(updatedComments);
    } catch (err) {
      const msg =
        err.response?.data?.message ||
        err.response?.data?.detail ||
        "Vui lòng đăng nhập để bình luận";
      setPopup(msg);
    }
  };

  // ── Click vào bình luận → hiện popover (chỉ admin) ──
  const handleCommentClick = (e, commentId) => {
    if (!isAdmin) return;
    e.preventDefault();
    e.stopPropagation();
    setPopover({ x: e.clientX, y: e.clientY, commentId });
  };

  // ── Bắt đầu xóa: đóng popover, mở modal xác nhận ────
  const handleRequestDelete = () => {
    setConfirmDelete(popover.commentId);
    setPopover(null);
  };

  // ── Xác nhận xóa bình luận ───────────────────────────
  const handleConfirmDelete = async () => {
    try {
      await newsApi.deleteComment(confirmDelete);
      setComments((prev) => prev.filter((c) => c.commentID !== confirmDelete));
      setPopup("Đã xóa bình luận thành công.");
    } catch (err) {
      setPopup(
        err.response?.data?.message || "Không thể xóa bình luận. Thử lại sau.",
      );
    } finally {
      setConfirmDelete(null);
    }
  };

  // ── Render error ─────────────────────────────────────
  if (error) {
    return (
      <div className="max-w-5xl mx-auto px-4 py-12">
        <Button link="/news" message="Trở về Trang chủ" />
        <div className="mt-6 flex flex-col lg:flex-row gap-8">
          <div className="flex-1 bg-white rounded-2xl border border-gray-100 p-16 flex flex-col items-center gap-4 text-center">
            <div className="text-red-400 text-5xl">🔗</div>
            <h3 className="text-xl font-semibold text-gray-800">
              Không tìm thấy bài viết
            </h3>
            <p className="text-gray-500 text-sm">
              Tin tức này hiện không còn tồn tại hoặc đã bị ẩn khỏi hệ thống.
            </p>
            <button
              onClick={() => navigate("/news")}
              className="mt-2 px-5 py-2 border border-gray-300 rounded-lg text-sm hover:bg-gray-50 transition"
            >
              🏠 Về trang chủ
            </button>
          </div>
          <div className="w-full lg:w-72">
            <NewsSidebar relatedNews={relatedNews} navigate={navigate} />
          </div>
        </div>
      </div>
    );
  }

  if (!news)
    return (
      <div className="max-w-5xl mx-auto px-4 py-12 text-center text-gray-500">
        Đang tải bài viết...
      </div>
    );

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      <Button link="/news" message="Trở về Trang chủ" />

      <div className="flex flex-col lg:flex-row gap-8 mt-4">
        <div className="flex-1 bg-white rounded-xl shadow-sm border border-gray-100 p-8">
          {/* Badge */}
          <div className="mb-3">
            <span
              className={`text-xs font-semibold px-3 py-1 rounded-full ${newsTypeColor[news.newsType] || "bg-gray-100 text-gray-600"}`}
            >
              {newsTypeLabel[news.newsType] || news.newsType}
            </span>
          </div>

          {/* Tiêu đề */}
          <h1 className="text-3xl font-bold text-[#1f4c7a] leading-tight mb-4">
            {news.title}
          </h1>

          {/* Mô tả ngắn */}
          {news.resourceContent && (
            <div className="mb-6 bg-[#f8fafc] border-l-4 border-[#1f4c7a] pl-5 py-4 rounded-r-xl italic text-gray-600 leading-relaxed">
              {news.resourceContent}
            </div>
          )}

          {/* Meta */}
          <div className="flex items-center gap-4 text-sm text-gray-500 border-b pb-4 mb-6 flex-wrap">
            <span>📅 {formatDateTime(news.createdAt)}</span>
            <span>👁 {news.viewCount || 0} lượt xem</span>
            <span>✍️ Quản trị viên Khoa</span>
          </div>

          {/* Nội dung chi tiết */}
          <div className="prose max-w-none text-gray-700 leading-relaxed whitespace-pre-wrap">
            {news.content || (
              <p className="text-gray-400 italic">
                Không có nội dung chi tiết.
              </p>
            )}
          </div>

          {/* ── PHẦN BÌNH LUẬN ── */}
          <div className="mt-10 border-t pt-6">
            <h3 className="text-lg font-semibold text-gray-800 mb-4">
              💬 Bình luận ({comments.length})
            </h3>

            {isAdmin && (
              <p className="text-xs text-blue-500 mb-4 bg-blue-50 px-3 py-2 rounded-lg">
                💡 Click vào bình luận để xem tùy chọn xóa.
              </p>
            )}

            {/* Form bình luận — chỉ hiện khi không phải admin */}
            {!isAdmin && (
              <div className="flex gap-3 mb-6">
                <div className="w-9 h-9 rounded-full bg-gray-300 flex items-center justify-center text-white text-sm flex-shrink-0">
                  👤
                </div>
                <div className="flex-1">
                  <textarea
                    value={commentText}
                    onChange={(e) => setCommentText(e.target.value)}
                    placeholder="Nhập bình luận của bạn về bài viết này..."
                    className="w-full border border-gray-200 rounded-lg px-4 py-3 text-sm resize-none focus:outline-none focus:ring-2 focus:ring-[#1f4c7a] min-h-[80px]"
                  />
                  <div className="flex justify-end mt-2">
                    <button
                      onClick={handleComment}
                      className="flex items-center gap-2 bg-[#1f4c7a] text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-[#163a5d] transition"
                    >
                      ✈️ Gửi bình luận
                    </button>
                  </div>
                </div>
              </div>
            )}

            {/* Danh sách bình luận */}
            <div className="space-y-4">
              {comments.length === 0 ? (
                <p className="text-gray-500 text-sm italic">
                  Chưa có bình luận nào.
                </p>
              ) : (
                comments.map((c) => (
                  <div
                    key={c.commentID}
                    className={`flex gap-3 rounded-xl p-2 transition ${
                      isAdmin
                        ? "cursor-pointer hover:bg-red-50 hover:ring-1 hover:ring-red-200"
                        : ""
                    }`}
                    onClick={(e) => handleCommentClick(e, c.commentID)}
                    title={isAdmin ? "Click để xóa bình luận" : undefined}
                  >
                    <div className="w-9 h-9 rounded-full bg-[#1f4c7a] flex items-center justify-center text-white text-sm font-bold flex-shrink-0">
                      {c.studentName?.charAt(0) || "?"}
                    </div>
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <span className="text-sm font-semibold text-gray-800">
                          {c.studentName}
                        </span>
                        <span className="text-xs text-gray-400">
                          {c.msv &&
                            `(${/^\d+$/.test(c.msv) ? "Sinh viên" : "Quản trị viên"})`}
                        </span>
                        <span className="text-xs text-gray-400">
                          {timeAgo(c.createdAt)}
                        </span>
                      </div>
                      <p className="text-sm text-gray-600">{c.content}</p>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>

        {/* Sidebar */}
        <div className="w-full lg:w-72 flex-shrink-0">
          <NewsSidebar relatedNews={relatedNews} navigate={navigate} />
        </div>
      </div>

      {/* Popover */}
      {popover && (
        <CommentPopover
          x={popover.x}
          y={popover.y}
          onDelete={handleRequestDelete}
          onClose={() => setPopover(null)}
        />
      )}

      {/* Modal xác nhận xóa */}
      {confirmDelete && (
        <ConfirmDeleteModal
          onConfirm={handleConfirmDelete}
          onCancel={() => setConfirmDelete(null)}
        />
      )}

      {popup && <PopupMessage message={popup} onClose={() => setPopup(null)} />}
    </div>
  );
};

// Sidebar
const NewsSidebar = ({ relatedNews, navigate }) => (
  <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 sticky top-4">
    <h3 className="font-semibold text-gray-800 mb-4 text-sm uppercase tracking-wide">
      Tin tức khác
    </h3>
    <div className="space-y-4">
      {relatedNews.length === 0 ? (
        <p className="text-sm text-gray-400">Không có tin liên quan.</p>
      ) : (
        relatedNews.map((item) => (
          <div
            key={item.newsID}
            onClick={() => navigate(`/news/${item.newsID}`, { state: item })}
            className="flex gap-3 cursor-pointer group"
          >
            <div className="w-14 h-14 rounded-lg bg-[#e8f0f9] flex items-center justify-center text-xl flex-shrink-0">
              📰
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-gray-700 group-hover:text-[#1f4c7a] transition line-clamp-2">
                {item.title}
              </p>
              <p className="text-xs text-gray-400 mt-1">
                {timeAgo(item.createdAt)}
              </p>
            </div>
          </div>
        ))
      )}
    </div>
  </div>
);

export default NewsDetail;
