import React, { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import newsApi from '../../../api/newsApi'
import { newsTypeLabel, newsTypeColor } from '../../../constants/news'
import { formatDate, timeAgo } from '../../../helpers/newsHelpers'
import PopupMessage from '../../../components/parts/PopupMessage'

// ── Modal xác nhận xóa ─────────────────────────────
const DeleteModal = ({ onConfirm, onCancel }) => (
	<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50'>
		<div className='bg-white rounded-2xl p-8 max-w-sm w-full mx-4 text-center shadow-2xl'>
			<div className='w-16 h-16 bg-red-50 rounded-full flex items-center justify-center mx-auto mb-4 text-3xl'>
				🗑️
			</div>
			<h3 className='text-xl font-bold text-gray-900 mb-2'>
				Xác nhận xóa tin tức
			</h3>
			<p className='text-gray-500 text-sm mb-2'>
				Bạn có chắc chắn muốn xóa tin tức này khỏi hệ thống không?
			</p>
			<p className='text-red-500 text-xs font-medium mb-6'>
				* Hành động này không thể hoàn tác và sẽ xóa toàn bộ dữ liệu
				liên quan.
			</p>
			<div className='flex gap-3'>
				<button
					onClick={onCancel}
					className='flex-1 py-3 border border-gray-200 rounded-xl font-medium text-gray-700 hover:bg-gray-50 transition'>
					Hủy bỏ
				</button>
				<button
					onClick={onConfirm}
					className='flex-1 py-3 bg-red-500 text-white rounded-xl font-medium hover:bg-red-600 transition flex items-center justify-center gap-2'>
					🗑️ Xóa tin tức
				</button>
			</div>
		</div>
	</div>
)

// ── Modal từ chối ──────────────────────────────────
const RejectModal = ({ onConfirm, onCancel }) => {
	const [reason, setReason] = useState('')
	return (
		<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50'>
			<div className='bg-white rounded-2xl p-8 max-w-sm w-full mx-4 shadow-2xl'>
				<div className='flex items-center gap-3 mb-4 pb-4 border-b'>
					<div className='w-10 h-10 bg-orange-100 rounded-full flex items-center justify-center text-xl'>
						❌
					</div>
					<h3 className='text-xl font-bold text-gray-900'>
						Từ chối phê duyệt
					</h3>
				</div>
				<p className='text-sm font-medium text-gray-800 mb-1'>
					Lý do từ chối
				</p>
				<p className='text-xs text-gray-400 mb-3'>
					Lý do này sẽ được gửi cho tác giả để họ chỉnh sửa lại nội
					dung phù hợp.
				</p>
				<textarea
					value={reason}
					onChange={(e) => setReason(e.target.value)}
					placeholder='Nhập lý do từ chối tin tức này...'
					className='w-full border border-gray-200 rounded-xl px-4 py-3 text-sm resize-none min-h-[120px] focus:outline-none focus:ring-2 focus:ring-orange-400 mb-4'
				/>
				<div className='flex gap-3 justify-end'>
					<button
						onClick={onCancel}
						className='px-5 py-2.5 border border-gray-200 rounded-xl text-sm font-medium hover:bg-gray-50 transition'>
						Hủy bỏ
					</button>
					<button
						onClick={() => onConfirm(reason)}
						className='px-5 py-2.5 bg-orange-400 text-white rounded-xl text-sm font-medium hover:bg-orange-500 transition flex items-center gap-2'>
						✈️ Xác nhận
					</button>
				</div>
			</div>
		</div>
	)
}

// ── Modal bình luận ────────────────────────────────
const CommentsModal = ({ news, onClose }) => {
	const [comments, setComments] = useState([])

	useEffect(() => {
		if (!news?.newsID) return
		newsApi
			.getComments(news.newsID)
			.then(setComments)
			.catch(() => {})
	}, [news?.newsID])

	const handleDelete = async (commentId) => {
		try {
			await newsApi.deleteComment(commentId)
			setComments((prev) => prev.filter((c) => c.id !== commentId))
		} catch {}
	}

	return (
		<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50'>
			<div className='bg-white rounded-2xl p-8 max-w-lg w-full mx-4 shadow-2xl'>
				<h3 className='text-lg font-bold text-gray-900 mb-1'>
					Bình luận bài viết
				</h3>
				<p className='text-sm text-gray-400 mb-4 pb-4 border-b'>
					Bài viết: {news?.title}
				</p>
				<div className='space-y-4 max-h-80 overflow-y-auto'>
					{comments.length === 0 ? (
						<p className='text-sm text-gray-400 text-center py-8'>
							Chưa có bình luận nào.
						</p>
					) : (
						comments.map((c) => (
							<div
								key={c.id}
								className='flex items-start gap-3 pb-4 border-b last:border-0'>
								<div className='w-9 h-9 rounded-full bg-[#1f4c7a] flex items-center justify-center text-white text-sm font-bold flex-shrink-0'>
									{c.studentName?.charAt(0) || '?'}
								</div>
								<div className='flex-1'>
									<div className='flex items-center justify-between'>
										<span className='text-sm font-semibold text-gray-800 underline'>
											{c.studentName}
										</span>
										<button
											onClick={() => handleDelete(c.id)}
											className='text-red-400 hover:text-red-600 text-xs transition'>
											Xóa
										</button>
									</div>
									<p className='text-sm text-gray-600 mt-1'>
										{c.content}
									</p>
								</div>
							</div>
						))
					)}
				</div>
				<div className='flex justify-end mt-4'>
					<button
						onClick={onClose}
						className='px-6 py-2.5 border border-gray-200 rounded-xl text-sm font-medium hover:bg-gray-50 transition'>
						Đóng
					</button>
				</div>
			</div>
		</div>
	)
}

// ── Modal thêm/sửa bài ─────────────────────────────
const EditModal = ({ news, onClose, onSubmit }) => {
	const [form, setForm] = useState({
		title: news?.title || '',
		newsType: news?.newsType || '',
		content: news?.content || ''
	})
	const isEdit = !!news?.newsID
	const set = (k, v) => setForm((f) => ({ ...f, [k]: v }))

	return (
		<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50'>
			<div className='bg-white rounded-2xl p-8 max-w-2xl w-full mx-4 shadow-2xl'>
				<h3 className='text-xl font-bold text-gray-900 mb-6'>
					{isEdit ? 'Chỉnh sửa tin tức' : 'Thêm tin tức mới'}
				</h3>
				<div className='space-y-4'>
					<div>
						<label className='text-sm font-medium text-gray-700 mb-1 block'>
							Tiêu đề bài viết{' '}
							<span className='text-red-500'>*</span>
						</label>
						<input
							value={form.title}
							onChange={(e) => set('title', e.target.value)}
							placeholder='Nhập tiêu đề tin tức...'
							className='w-full border border-gray-200 rounded-xl px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-[#1f4c7a]'
						/>
					</div>
					<div>
						<label className='text-sm font-medium text-gray-700 mb-1 block'>
							Danh mục <span className='text-red-500'>*</span>
						</label>
						<select
							value={form.newsType}
							onChange={(e) => set('newsType', e.target.value)}
							className='w-full border border-gray-200 rounded-xl px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-[#1f4c7a]'>
							<option value=''>-- Chọn danh mục --</option>
							<option value='Event'>Sự kiện</option>
							<option value='Announcement'>Thông báo</option>
							<option value='Education'>Tin giáo dục</option>
							<option value='Admission'>Tuyển sinh</option>
							<option value='Other'>Khác</option>
						</select>
					</div>
					<div>
						<label className='text-sm font-medium text-gray-700 mb-1 block'>
							Mô tả ngắn
						</label>
						<textarea
							placeholder='Đoạn trích dẫn ngắn hiển thị ở trang chủ...'
							className='w-full border border-gray-200 rounded-xl px-4 py-3 text-sm resize-none min-h-[80px] focus:outline-none focus:ring-2 focus:ring-[#1f4c7a]'
						/>
					</div>
					<div>
						<label className='text-sm font-medium text-gray-700 mb-1 block'>
							Nội dung chi tiết{' '}
							<span className='text-red-500'>*</span>
						</label>
						<textarea
							value={form.content}
							onChange={(e) => set('content', e.target.value)}
							placeholder='Nội dung bài viết...'
							className='w-full border border-gray-200 rounded-xl px-4 py-3 text-sm resize-none min-h-[160px] focus:outline-none focus:ring-2 focus:ring-[#1f4c7a]'
						/>
					</div>
				</div>
				<div className='flex justify-end gap-3 mt-6'>
					<button
						onClick={onClose}
						className='px-5 py-2.5 border border-gray-200 rounded-xl text-sm font-medium hover:bg-gray-50 transition'>
						Hủy
					</button>
					<button
						onClick={() => onSubmit(form)}
						className='px-6 py-2.5 bg-[#1f4c7a] text-white rounded-xl text-sm font-medium hover:bg-[#163a5d] transition'>
						Đăng bài
					</button>
				</div>
			</div>
		</div>
	)
}

// ── Main ───────────────────────────────────────────
const AdminNewsManagement = () => {
	const navigate = useNavigate()
	const [tab, setTab] = useState('list')
	const [newsList, setNewsList] = useState([])
	const [pendingList, setPendingList] = useState([])
	const [popup, setPopup] = useState(null)
	const [isLoading, setIsLoading] = useState(false)
	const [keyword, setKeyword] = useState('')
	const [selectedType, setSelectedType] = useState('')

	const [deleteTarget, setDeleteTarget] = useState(null)
	const [rejectTarget, setRejectTarget] = useState(null)
	const [editTarget, setEditTarget] = useState(null)
	const [commentTarget, setCommentTarget] = useState(null)

	const showPopup = (msg) => setPopup(msg)

	const loadData = async () => {
		setIsLoading(true)
		try {
			const [listRes, pending] = await Promise.all([
				newsApi.search({ pageSize: 100 }),
				newsApi.getPendingList()
			])
			setNewsList(listRes.items ?? listRes)
			setPendingList(pending)
		} catch {
			showPopup('Không thể tải dữ liệu.')
		} finally {
			setIsLoading(false)
		}
	}

	useEffect(() => {
		loadData()
	}, [])

	const handleDelete = async () => {
		try {
			await newsApi.delete(deleteTarget.newsID)
			showPopup('Đã xóa tin tức thành công.')
			setDeleteTarget(null)
			loadData()
		} catch {
			showPopup('Xóa tin tức thất bại.')
		}
	}

	const handleApprove = async (requestId) => {
		try {
			await newsApi.approve(requestId, {
				decision: 'Approved',
				rejectReason: null
			})
			showPopup('Đã phê duyệt thành công.')
			loadData()
		} catch {
			showPopup('Phê duyệt thất bại.')
		}
	}

	const handleReject = async (reason) => {
		try {
			await newsApi.approve(rejectTarget.newsRequestID, {
				decision: 'Rejected',
				rejectReason: reason
			})
			showPopup('Đã từ chối yêu cầu.')
			setRejectTarget(null)
			loadData()
		} catch {
			showPopup('Từ chối thất bại.')
		}
	}

	const handleSubmitEdit = async (form) => {
		try {
			if (editTarget?.newsID) {
				await newsApi.update(editTarget.newsID, form)
				showPopup('Đã gửi yêu cầu chỉnh sửa.')
			} else {
				await newsApi.create(form)
				showPopup('Đã tạo tin tức mới.')
			}
			setEditTarget(null)
			loadData()
		} catch {
			showPopup('Thao tác thất bại.')
		}
	}

	const filtered = newsList.filter((n) => {
		const matchKw = n.title.toLowerCase().includes(keyword.toLowerCase())
		const matchType = selectedType ? n.newsType === selectedType : true
		return matchKw && matchType
	})

	return (
		<div className='p-6'>
			{/* Header */}
			<div className='flex items-center justify-between mb-6'>
				<h1 className='text-2xl font-bold text-[#1f4c7a]'>
					Quản lý Tin tức & Sự kiện
				</h1>
				<button
					onClick={() => setEditTarget({})}
					className='flex items-center gap-2 bg-[#1f4c7a] text-white px-4 py-2.5 rounded-lg text-sm font-medium hover:bg-[#163a5d] transition'>
					+ Thêm mới
				</button>
			</div>

			{/* Tabs */}
			<div className='flex border-b border-gray-200 mb-6'>
				{[
					{ key: 'list', label: 'Danh sách Bài viết' },
					{
						key: 'pending',
						label: 'Danh sách chờ duyệt',
						count: pendingList.length
					}
				].map((t) => (
					<button
						key={t.key}
						onClick={() => setTab(t.key)}
						className={`px-4 py-2.5 text-sm font-medium border-b-2 transition flex items-center gap-2 ${
							tab === t.key
								? 'border-[#1f4c7a] text-[#1f4c7a]'
								: 'border-transparent text-gray-500 hover:text-gray-700'
						}`}>
						{t.label}
						{t.count > 0 && (
							<span className='bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center'>
								{t.count}
							</span>
						)}
					</button>
				))}
			</div>

			{/* TAB: DANH SÁCH */}
			{tab === 'list' && (
				<div className='bg-white rounded-xl border border-gray-100 shadow-sm p-5'>
					<div className='flex gap-3 mb-5'>
						<div className='flex-1 relative'>
							<span className='absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm'>
								🔍
							</span>
							<input
								type='text'
								placeholder='Tìm kiếm theo tiêu đề bài viết...'
								value={keyword}
								onChange={(e) => setKeyword(e.target.value)}
								className='w-full border border-gray-200 rounded-lg pl-9 pr-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#1f4c7a]'
							/>
						</div>
						<select
							value={selectedType}
							onChange={(e) => setSelectedType(e.target.value)}
							className='border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none w-[200px]'>
							<option value=''>-- Tất cả danh mục --</option>
							<option value='Event'>Sự kiện</option>
							<option value='Announcement'>Thông báo</option>
							<option value='Education'>Tin giáo dục</option>
							<option value='Admission'>Tuyển sinh</option>
							<option value='Other'>Khác</option>
						</select>
					</div>

					<table className='w-full text-sm'>
						<thead>
							<tr className='text-left text-gray-500 border-b border-gray-100 text-xs uppercase tracking-wide'>
								<th className='py-3 font-medium w-[40%]'>
									Tiêu đề bài viết
								</th>
								<th className='py-3 font-medium w-[15%]'>
									Danh mục
								</th>
								<th className='py-3 font-medium w-[15%]'>
									Tương tác
								</th>
								<th className='py-3 font-medium w-[15%]'>
									Trạng thái
								</th>
								<th className='py-3 font-medium w-[10%]'>
									Thao tác
								</th>
							</tr>
						</thead>
						<tbody className='divide-y divide-gray-50'>
							{isLoading ? (
								<tr>
									<td
										colSpan={5}
										className='py-12 text-center text-gray-400'>
										Đang tải...
									</td>
								</tr>
							) : filtered.length === 0 ? (
								<tr>
									<td
										colSpan={5}
										className='py-12 text-center text-gray-400'>
										Không có bài viết nào.
									</td>
								</tr>
							) : (
								filtered.map((item) => (
									<tr
										key={item.newsID}
										className='hover:bg-gray-50 transition'>
										<td className='py-3 pr-4'>
											<p className='font-medium text-gray-800 truncate max-w-xs'>
												{item.title}
											</p>
											<p className='text-xs text-gray-400 mt-0.5 truncate max-w-xs'>
												{item.content?.slice(0, 60)}...
											</p>
										</td>
										<td className='py-3'>
											<span
												className={`text-xs font-medium px-2.5 py-1 rounded-full ${newsTypeColor[item.newsType] || 'bg-gray-100 text-gray-600'}`}>
												{newsTypeLabel[item.newsType] ||
													item.newsType}
											</span>
										</td>
										<td className='py-3'>
											<button
												onClick={() =>
													setCommentTarget(item)
												}
												className='flex items-center gap-1 text-blue-500 hover:text-blue-700 text-xs transition'>
												💬 Bình luận
											</button>
										</td>
										<td className='py-3'>
											<span className='text-xs font-medium px-2.5 py-1 rounded-full bg-green-100 text-green-700'>
												Công khai
											</span>
										</td>
										<td className='py-3'>
											<div className='flex items-center gap-2'>
												<button
													onClick={() =>
														setEditTarget(item)
													}
													className='text-gray-400 hover:text-[#1f4c7a] transition'
													title='Chỉnh sửa'>
													✏️
												</button>
												<button
													onClick={() =>
														setDeleteTarget(item)
													}
													className='text-gray-400 hover:text-red-500 transition'
													title='Xóa'>
													🗑️
												</button>
											</div>
										</td>
									</tr>
								))
							)}
						</tbody>
					</table>
				</div>
			)}

			{/* TAB: CHỜ DUYỆT */}
			{tab === 'pending' && (
				<div className='bg-white rounded-xl border border-gray-100 shadow-sm p-5'>
					<h3 className='font-semibold text-gray-800 mb-4'>
						Bài viết đang chờ phê duyệt
					</h3>
					<table className='w-full text-sm'>
						<thead>
							<tr className='text-left text-gray-500 border-b border-gray-100 text-xs uppercase tracking-wide'>
								<th className='py-3 font-medium w-[28%]'>
									Tiêu đề bài viết
								</th>
								<th className='py-3 font-medium w-[12%]'>
									Danh mục
								</th>
								<th className='py-3 font-medium w-[13%]'>
									Người soạn
								</th>
								<th className='py-3 font-medium w-[14%]'>
									Thời gian gửi
								</th>
								<th className='py-3 font-medium w-[10%]'>
									Loại
								</th>
								<th className='py-3 font-medium w-[10%]'>
									Trạng thái
								</th>
								<th className='py-3 font-medium w-[10%]'>
									Thao tác
								</th>
							</tr>
						</thead>
						<tbody className='divide-y divide-gray-50'>
							{isLoading ? (
								<tr>
									<td
										colSpan={7}
										className='py-12 text-center text-gray-400'>
										Đang tải...
									</td>
								</tr>
							) : pendingList.length === 0 ? (
								<tr>
									<td
										colSpan={7}
										className='py-12 text-center text-gray-400'>
										Không có yêu cầu nào đang chờ duyệt.
									</td>
								</tr>
							) : (
								pendingList.map((item) => (
									<tr
										key={item.newsRequestID}
										className='hover:bg-gray-50 transition'>
										<td className='py-3 pr-4'>
											<p className='font-medium text-gray-800 truncate max-w-[200px]'>
												{item.title}
											</p>
										</td>
										<td className='py-3'>
											<span
												className={`text-xs font-medium px-2.5 py-1 rounded-full ${newsTypeColor[item.newsType] || 'bg-gray-100 text-gray-600'}`}>
												{newsTypeLabel[item.newsType] ||
													item.newsType}
											</span>
										</td>
										<td className='py-3 text-gray-600 text-xs'>
											{item.requesterName || 'Editor'}
										</td>
										<td className='py-3 text-gray-500 text-xs'>
											{formatDate(item.createdAt)}
										</td>
										<td className='py-3'>
											<span
												className={`text-xs font-medium px-2.5 py-1 rounded-full ${
													item.requestType ===
													'Replace'
														? 'bg-orange-100 text-orange-600'
														: 'bg-blue-100 text-blue-600'
												}`}>
												{item.requestType === 'Replace'
													? 'Chỉnh sửa'
													: 'Đăng mới'}
											</span>
										</td>
										<td className='py-3'>
											<span className='text-xs font-medium px-2.5 py-1 rounded-full bg-yellow-100 text-yellow-700'>
												Từ chối
											</span>
										</td>
										<td className='py-3'>
											<div className='flex items-center gap-2'>
												<button
													onClick={() =>
														handleApprove(
															item.newsRequestID
														)
													}
													className='text-green-500 hover:text-green-700 transition'
													title='Duyệt'>
													✅
												</button>
												<button
													onClick={() =>
														setRejectTarget(item)
													}
													className='text-red-400 hover:text-red-600 transition'
													title='Từ chối'>
													❌
												</button>
												<button
													onClick={() =>
														navigate(
															`/news/${item.newsID}`
														)
													}
													className='text-gray-400 hover:text-[#1f4c7a] transition'
													title='Xem chi tiết'>
													👁️
												</button>
											</div>
										</td>
									</tr>
								))
							)}
						</tbody>
					</table>
				</div>
			)}

			{/* Modals */}
			{deleteTarget && (
				<DeleteModal
					onConfirm={handleDelete}
					onCancel={() => setDeleteTarget(null)}
				/>
			)}
			{rejectTarget && (
				<RejectModal
					onConfirm={handleReject}
					onCancel={() => setRejectTarget(null)}
				/>
			)}
			{editTarget !== null && (
				<EditModal
					news={editTarget}
					onClose={() => setEditTarget(null)}
					onSubmit={handleSubmitEdit}
				/>
			)}
			{commentTarget && (
				<CommentsModal
					news={commentTarget}
					onClose={() => setCommentTarget(null)}
				/>
			)}

			{popup && (
				<PopupMessage message={popup} onClose={() => setPopup(null)} />
			)}
		</div>
	)
}

export default AdminNewsManagement
