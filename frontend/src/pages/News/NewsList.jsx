import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import newsApi from "../../api/newsApi";
import {
  newsTypeOptions,
  newsTypeLabel,
  newsTypeColor,
} from "../../constants/news";
import { formatDate, timeAgo, getPagination } from "../../helpers/newsHelpers";
import PopupMessage from "../../components/parts/PopupMessage";

const PAGE_SIZE = 10;

// Hàm chuẩn hóa NewsType để so sánh (Đảm bảo chạy đúng kể cả khi BE trả về số 0,1,2 hay chữ)
const normalizeNewsType = (val) => {
  if (val === 0 || val === "0") return "Event";
  if (val === 1 || val === "1") return "Announcement";
  if (val === 2 || val === "2") return "Education";
  if (val === 3 || val === "3") return "Admission";
  if (val === 4 || val === "4") return "Other";
  return String(val);
};

const NewsList = () => {
  const navigate = useNavigate();
  const [allNews, setAllNews] = useState([]); // fetch 1 lần, giữ toàn bộ
  const [popup, setPopup] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [keyword, setKeyword] = useState("");
  const [selectedType, setSelectedType] = useState("");
  const [page, setPage] = useState(1);

  // Fetch toàn bộ 1 lần khi mount
  useEffect(() => {
    const fetchAll = async () => {
      setIsLoading(true);
      try {
        const res = await newsApi.search({});
        setAllNews(res.items ?? res);
      } catch {
        setPopup("Không thể tải danh sách tin tức.");
      } finally {
        setIsLoading(false);
      }
    };
    fetchAll();
  }, []);

  // Filter trên frontend mỗi khi keyword hoặc selectedType thay đổi
  const filtered = allNews.filter((item) => {
    // 1. Chỉ tìm kiếm trong Tiêu đề (Bỏ tìm trong content để không dính thẻ HTML)
    const matchKeyword = keyword
      ? item.title?.toLowerCase().includes(keyword.toLowerCase())
      : true;

    // 2. Chuẩn hóa dữ liệu trước khi so sánh Danh mục
    const matchType = selectedType
      ? normalizeNewsType(item.newsType) === selectedType
      : true;

    return matchKeyword && matchType;
  });

  const totalPages = Math.ceil(filtered.length / PAGE_SIZE);
  const news = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);
  const pages = getPagination(page, totalPages);

  return (
    <div className="max-w-5xl mx-auto px-4 py-12">
      <h2 className="text-3xl font-bold text-[#1f4c7a] mb-8 border-l-4 border-red-500 pl-4">
        Tin tức & Sự kiện
      </h2>

      {/* Filter */}
      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        <input
          type="text"
          placeholder="Tìm kiếm theo tiêu đề bài viết..."
          value={keyword}
          onChange={(e) => {
            setKeyword(e.target.value);
            setPage(1);
          }}
          className="flex-1 border border-gray-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#1f4c7a]"
        />
        <select
          value={selectedType}
          onChange={(e) => {
            setSelectedType(e.target.value);
            setPage(1);
          }}
          className="border border-gray-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#1f4c7a] w-full sm:w-[200px]"
        >
          {/* Đã thêm option "Tất cả danh mục" để người dùng có thể bỏ lọc */}
          <option value="">-- Tất cả danh mục --</option>
          <option value="Event">Sự kiện</option>
          <option value="Announcement">Thông báo</option>
          <option value="Education">Tin giáo dục</option>
          <option value="Admission">Tuyển sinh</option>
          <option value="Other">Khác</option>
        </select>
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex gap-1 mb-4 flex-wrap">
          {pages.map((p, i) =>
            p === "..." ? (
              <span key={i} className="px-3 py-1 text-gray-400">
                ...
              </span>
            ) : (
              <button
                key={i}
                onClick={() => setPage(p)}
                className={`px-3 py-1 rounded text-sm font-medium transition ${
                  p === page
                    ? "bg-[#1f4c7a] text-white"
                    : "bg-gray-100 text-gray-600 hover:bg-gray-200"
                }`}
              >
                {p}
              </button>
            ),
          )}
        </div>
      )}

      {/* List */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 divide-y divide-gray-100">
        {isLoading ? (
          <p className="text-center py-12 text-gray-500">Đang tải dữ liệu...</p>
        ) : news.length === 0 ? (
          <p className="text-center py-12 text-gray-500">
            Không tìm thấy tin tức phù hợp.
          </p>
        ) : (
          news.map((item) => {
            const normalizedType = normalizeNewsType(item.newsType);
            return (
              <div
                key={item.newsID}
                onClick={() =>
                  navigate(`/news/${item.newsID}`, {
                    state: item,
                  })
                }
                className="flex gap-4 p-5 cursor-pointer hover:bg-gray-50 transition group"
              >
                {/* Icon */}
                <div className="flex-shrink-0 w-12 h-12 rounded-lg bg-[#e8f0f9] flex items-center justify-center text-2xl">
                  {normalizedType === "Event"
                    ? "📅"
                    : normalizedType === "Announcement"
                      ? "📢"
                      : normalizedType === "Education"
                        ? "🎓"
                        : normalizedType === "Admission"
                          ? "✏️"
                          : "📰"}
                </div>

                {/* Nội dung */}
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1 flex-wrap">
                    <span
                      className={`text-xs font-medium px-2 py-0.5 rounded-full ${newsTypeColor[normalizedType] || "bg-gray-100 text-gray-600"}`}
                    >
                      {newsTypeLabel[normalizedType] || normalizedType}
                    </span>
                    <span className="text-xs text-gray-400">
                      {timeAgo(item.createdAt)}
                    </span>
                  </div>
                  <h3 className="font-semibold text-gray-800 group-hover:text-[#1f4c7a] transition truncate">
                    {item.title}
                  </h3>
                  {/* Đã xóa hiển thị raw HTML Content, thay bằng text sạch gọn gàng */}
                  <p className="text-sm text-gray-500 mt-1 line-clamp-2 italic">
                    Nhấn vào để xem chi tiết bài viết...
                  </p>
                </div>

                {/* Stats */}
                <div className="flex-shrink-0 text-right text-xs text-gray-400 flex flex-col justify-center gap-1">
                  <span>👁 {item.viewCount}</span>
                  <span>{formatDate(item.createdAt)}</span>
                </div>
              </div>
            );
          })
        )}
      </div>

      {popup && <PopupMessage message={popup} onClose={() => setPopup(null)} />}
    </div>
  );
};

export default NewsList;
