import React, { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import newsApi from '../../../api/newsApi'
import PopupMessage from '../../../components/parts/PopupMessage'

// ── Các Component dùng chung (Modal) ─────────────────────────────
const DeleteModal = ({ onConfirm, onCancel }) => (
	<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50'>
		<div className='bg-white rounded-2xl p-8 max-w-sm w-full mx-4 text-center shadow-2xl'>
			<div className='w-16 h-16 bg-red-50 rounded-full flex items-center justify-center mx-auto mb-4 text-3xl'>
				🗑️
			</div>
			<h3 className='text-xl font-bold text-gray-900 mb-2'>
				Xác nhận xóa tin tức
			</h3>
			<p className='text-gray-500 text-sm mb-6'>
				Hành động này không thể hoàn tác và sẽ xóa toàn bộ dữ liệu liên
				quan.
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
				<textarea
					value={reason}
					onChange={(e) => setReason(e.target.value)}
					placeholder='Nhập lý do từ chối...'
					className='w-full border border-gray-200 rounded-xl px-4 py-3 text-sm min-h-[120px] focus:outline-none focus:ring-2 focus:ring-orange-400 mb-4'
				/>
				<div className='flex gap-3 justify-end'>
					<button
						onClick={onCancel}
						className='px-5 py-2.5 border rounded-xl text-sm font-medium hover:bg-gray-50'>
						Hủy bỏ
					</button>
					<button
						onClick={() => onConfirm(reason)}
						className='px-5 py-2.5 bg-orange-400 text-white rounded-xl text-sm font-medium hover:bg-orange-500'>
						✈️ Xác nhận
					</button>
				</div>
			</div>
		</div>
	)
}

const CommentPopover = ({ x, y, onDelete, onClose }) => {
	const ref = useRef(null)
	useEffect(() => {
		const handler = (e) => {
			if (ref.current && !ref.current.contains(e.target)) onClose()
		}
		document.addEventListener('mousedown', handler)
		return () => document.removeEventListener('mousedown', handler)
	}, [onClose])
	return (
		<div
			ref={ref}
			style={{
				position: 'fixed',
				top: Math.min(y, window.innerHeight - 80),
				left: Math.min(x, window.innerWidth - 180),
				zIndex: 9999
			}}
			className='bg-white border rounded-xl shadow-xl py-1 min-w-[160px]'>
			<button
				onClick={onDelete}
				className='w-full text-left px-4 py-2 text-sm text-red-500 hover:bg-red-50'>
				🗑️ Xóa bình luận
			</button>
		</div>
	)
}

const ConfirmDeleteCommentModal = ({ onConfirm, onCancel }) => (
	<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-[60]'>
		<div className='bg-white rounded-2xl p-8 max-w-sm w-full mx-4 text-center shadow-2xl'>
			<h3 className='text-xl font-bold mb-2'>Xóa bình luận</h3>
			<div className='flex gap-3 mt-6'>
				<button
					onClick={onCancel}
					className='flex-1 py-3 border rounded-xl'>
					Hủy bỏ
				</button>
				<button
					onClick={onConfirm}
					className='flex-1 py-3 bg-red-500 text-white rounded-xl'>
					Đồng ý
				</button>
			</div>
		</div>
	</div>
)

const CommentsModal = ({ news, onClose }) => {
	const [comments, setComments] = useState([])
	const [popover, setPopover] = useState(null)
	const [confirmId, setConfirmId] = useState(null)
	const [successMsg, setSuccessMsg] = useState('')

	useEffect(() => {
		if (!news?.id) return
		// Lấy thông tin News (bao gồm comments từ DTO mới của Backend)
		newsApi
			.getById(news.id)
			.then((data) => setComments(data?.comments || []))
			.catch(() => {})
	}, [news?.id])

	const handleCommentClick = (e, commentId) => {
		e.stopPropagation()
		setPopover({ x: e.clientX, y: e.clientY, commentId })
	}
	const handleRequestDelete = () => {
		setConfirmId(popover.commentId)
		setPopover(null)
	}

	const handleConfirmDelete = async () => {
		try {
			await newsApi.deleteComment(confirmId)
			setComments((prev) => prev.filter((c) => c.commentID !== confirmId))
			setSuccessMsg('Đã xóa bình luận thành công.')
			setTimeout(() => setSuccessMsg(''), 3000)
		} catch (err) {
			setSuccessMsg('Không thể xóa. Thử lại sau.')
			setTimeout(() => setSuccessMsg(''), 3000)
		} finally {
			setConfirmId(null)
		}
	}

	return (
		<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50'>
			<div className='bg-white rounded-2xl p-8 max-w-lg w-full mx-4 shadow-2xl'>
				<h3 className='text-lg font-bold text-gray-900 mb-4 border-b pb-2'>
					Bình luận: {news?.title}
				</h3>
				{successMsg && (
					<div className='mb-3 px-4 py-2 bg-green-50 text-green-700 text-sm rounded-lg'>
						✅ {successMsg}
					</div>
				)}
				<div className='space-y-2 max-h-80 overflow-y-auto'>
					{comments.length === 0 ? (
						<p className='text-sm text-gray-400 text-center py-8'>
							Chưa có bình luận nào.
						</p>
					) : (
						comments.map((c) => (
							<div
								key={c.commentID}
								onClick={(e) =>
									handleCommentClick(e, c.commentID)
								}
								className='flex items-start gap-3 p-3 rounded-xl cursor-pointer hover:bg-red-50 transition border'>
								<div className='flex-1'>
									<span className='text-sm font-semibold text-gray-800'>
										{c.studentName}
									</span>
									<p className='text-sm text-gray-600'>
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
						className='px-6 py-2 border rounded-xl text-sm'>
						Đóng
					</button>
				</div>
			</div>
			{popover && (
				<CommentPopover
					x={popover.x}
					y={popover.y}
					onDelete={handleRequestDelete}
					onClose={() => setPopover(null)}
				/>
			)}
			{confirmId && (
				<ConfirmDeleteCommentModal
					onConfirm={handleConfirmDelete}
					onCancel={() => setConfirmId(null)}
				/>
			)}
		</div>
	)
}

const EditModal = ({ news, onClose, onSubmit }) => {
	const isEdit = !!news?.id
	const [form, setForm] = useState({
		title: news?.title ?? '',
		newsType: news?.newsType ?? '',
		content: news?.content ?? ''
	})
	const [errors, setErrors] = useState({})
	const set = (k, v) => setForm((f) => ({ ...f, [k]: v }))

	const handleSubmit = () => {
		const newErrors = {}
		if (!form.title.trim()) newErrors.title = 'Bắt buộc'
		if (!form.newsType) newErrors.newsType = 'Bắt buộc'
		if (!form.content.trim()) newErrors.content = 'Bắt buộc'
		if (Object.keys(newErrors).length > 0) {
			setErrors(newErrors)
			return
		}
		onSubmit(form)
	}

	return (
		<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50 overflow-y-auto py-8'>
			<div className='bg-white rounded-2xl p-8 max-w-2xl w-full mx-4 shadow-2xl'>
				<h3 className='text-xl font-bold mb-6'>
					{isEdit ? 'Chỉnh sửa tin tức' : 'Thêm tin tức mới'}
				</h3>
				<div className='space-y-5'>
					<div>
						<label className='text-sm font-medium mb-1 block'>
							Tiêu đề
						</label>
						<input
							value={form.title}
							onChange={(e) => set('title', e.target.value)}
							className='w-full border rounded-xl px-4 py-2'
						/>
					</div>
					<div>
						<label className='text-sm font-medium mb-1 block'>
							Danh mục
						</label>
						<select
							value={form.newsType}
							onChange={(e) => set('newsType', e.target.value)}
							className='w-full border rounded-xl px-4 py-2'>
							<option value=''>-- Chọn danh mục --</option>
							<option value='Event'>Sự kiện</option>
							<option value='Announcement'>Thông báo</option>
							<option value='Education'>Tin giáo dục</option>
							<option value='Admission'>Tuyển sinh</option>
							<option value='Recruitment'>Tuyển dụng</option>
							<option value='Other'>Khác</option>
						</select>
					</div>
					<div>
						<label className='text-sm font-medium mb-1 block'>
							Nội dung
						</label>
						<textarea
							value={form.content}
							onChange={(e) => set('content', e.target.value)}
							className='w-full border rounded-xl px-4 py-2 min-h-[180px]'
						/>
					</div>
				</div>
				<div className='flex justify-end gap-3 mt-8'>
					<button
						onClick={onClose}
						className='px-6 py-2 border rounded-xl text-sm'>
						Thoát
					</button>
					<button
						onClick={handleSubmit}
						className='px-8 py-2 bg-[#1f4c7a] text-white rounded-xl text-sm'>
						{isEdit ? 'Lưu' : 'Đăng bài'}
					</button>
				</div>
			</div>
		</div>
	)
}

// Vì DTO Pending Backend không trả về chi tiết Content, ta chỉ hiện Tóm tắt ở Preview Modal
const PreviewModal = ({ item, onClose, onApprove, onReject, canApprove }) => (
	<div className='fixed inset-0 bg-black/60 flex items-center justify-center z-50'>
		<div className='bg-white rounded-3xl p-8 max-w-2xl w-full shadow-2xl'>
			<h2 className='text-2xl font-bold mb-4'>{item.title}</h2>
			<p className='text-gray-500 text-sm mb-6'>
				Yêu cầu từ: {item.requesterName} • Loại: {item.requestType}
			</p>
			<div className='flex justify-end gap-3 mt-4 border-t pt-4'>
				<button
					onClick={onClose}
					className='px-6 py-2 border rounded-xl'>
					Đóng
				</button>
				{canApprove && (
					<>
						<button
							onClick={onReject}
							className='px-6 py-2 bg-orange-500 text-white rounded-xl'>
							Từ chối
						</button>
						<button
							onClick={onApprove}
							className='px-6 py-2 bg-green-600 text-white rounded-xl'>
							Duyệt
						</button>
					</>
				)}
			</div>
		</div>
	</div>
)

// ── Main Component ───────────────────────────────────────────
const AdminNewsManagement = () => {
	const navigate = useNavigate()
	const getAdminLevelFromToken = () => {
		try {
			const token = localStorage.getItem('token')
			if (!token) return 3
			const payload = JSON.parse(atob(token.split('.')[1]))
			return parseInt(payload['Level'] ?? '3', 10)
		} catch {
			return 3
		}
	}
	const adminLevel = getAdminLevelFromToken()
	const canApprove = adminLevel === 0 || adminLevel === 1 || adminLevel === 2

	const [tab, setTab] = useState('list')
	const [newsList, setNewsList] = useState([])
	const [pendingList, setPendingList] = useState([])
	const [popup, setPopup] = useState(null)
	const [isLoading, setIsLoading] = useState(false)

	const [keyword, setKeyword] = useState('')
	const [selectedType, setSelectedType] = useState('')
	const [pendingKeyword, setPendingKeyword] = useState('')

	const [deleteTarget, setDeleteTarget] = useState(null)
	const [rejectTarget, setRejectTarget] = useState(null)
	const [editTarget, setEditTarget] = useState(null)
	const [commentTarget, setCommentTarget] = useState(null)
	const [previewTarget, setPreviewTarget] = useState(null)

	const showPopup = (msg) => setPopup(msg)

	const loadData = async () => {
		setIsLoading(true)
		try {
			const listRes = await newsApi.search({ pageSize: 100 })
			setNewsList(listRes.items ?? [])
			const pendingRes = await newsApi.getPendingList()
			setPendingList(pendingRes.items ?? [])
		} catch {
			showPopup('Lỗi khi tải dữ liệu.')
		} finally {
			setIsLoading(false)
		}
	}

	useEffect(() => {
		loadData()
	}, [])

	const handleDelete = async () => {
		try {
			await newsApi.delete(deleteTarget.id) // Sửa thành id
			showPopup('Đã xóa tin tức.')
			setDeleteTarget(null)
			loadData()
		} catch {
			showPopup('Xóa thất bại.')
		}
	}

	const handleApprove = async (requestId) => {
		try {
			// Dùng payload chuẩn
			await newsApi.approve(requestId, { isApproved: true, reason: null })
			showPopup('Đã phê duyệt.')
			loadData()
		} catch (err) {
			showPopup('Lỗi phê duyệt.')
		}
	}

	const handleReject = async (reason) => {
		try {
			// Dùng payload chuẩn
			await newsApi.approve(rejectTarget.id, {
				isApproved: false,
				reason: reason
			})
			showPopup('Đã từ chối.')
			setRejectTarget(null)
			loadData()
		} catch (err) {
			showPopup('Lỗi từ chối.')
		}
	}

	const handleSubmitEdit = async (form) => {
		try {
			if (editTarget?.id) {
				await newsApi.update(editTarget.id, form)
				showPopup('Đã gửi yêu cầu chỉnh sửa.')
			} else {
				await newsApi.create(form)
				showPopup('Đã tạo yêu cầu tin tức mới.')
			}
			setEditTarget(null)
			loadData()
		} catch {
			showPopup('Thao tác thất bại.')
		}
	}

	const filtered = newsList.filter(
		(n) =>
			n.title.toLowerCase().includes(keyword.toLowerCase()) &&
			(!selectedType || n.newsType === selectedType)
	)
	const filteredPending = pendingList.filter((n) =>
		n.title.toLowerCase().includes(pendingKeyword.toLowerCase())
	)

	return (
		<div className='p-6'>
			<div className='flex items-center justify-between mb-6'>
				<h1 className='text-2xl font-bold text-[#1f4c7a]'>
					Quản lý Tin tức
				</h1>
				<button
					onClick={() => setEditTarget({})}
					className='bg-[#1f4c7a] text-white px-4 py-2 rounded-lg'>
					+ Thêm mới
				</button>
			</div>

			<div className='flex border-b mb-6 gap-4'>
				<button
					onClick={() => setTab('list')}
					className={`pb-2 ${tab === 'list' ? 'border-b-2 border-blue-600 font-bold' : 'text-gray-500'}`}>
					Danh sách Bài viết
				</button>
				<button
					onClick={() => setTab('pending')}
					className={`pb-2 ${tab === 'pending' ? 'border-b-2 border-blue-600 font-bold' : 'text-gray-500'}`}>
					Chờ duyệt ({pendingList.length})
				</button>
			</div>

			{tab === 'list' && (
				<div className='bg-white p-5 rounded-xl shadow-sm border'>
					<div className='flex gap-3 mb-5'>
						<input
							type='text'
							placeholder='Tìm tiêu đề...'
							value={keyword}
							onChange={(e) => setKeyword(e.target.value)}
							className='border p-2 rounded w-full'
						/>
					</div>
					<table className='w-full text-sm text-left'>
						<thead>
							<tr className='border-b bg-gray-50'>
								<th className='p-3'>Tiêu đề</th>
								<th className='p-3'>Thao tác</th>
							</tr>
						</thead>
						<tbody>
							{filtered.map((item) => (
								<tr
									key={item.id}
									className='border-b hover:bg-gray-50'>
									<td className='p-3'>{item.title}</td>
									<td className='p-3 flex gap-3'>
										<button
											onClick={() =>
												setCommentTarget(item)
											}
											className='text-blue-500'>
											💬
										</button>
										<button
											onClick={() => {
												// Gọi API getById để lấy Content fill vào form Edit
												newsApi
													.getById(item.id)
													.then((res) =>
														setEditTarget(res)
													)
											}}
											className='text-gray-500'>
											✏️
										</button>
										<button
											onClick={() =>
												setDeleteTarget(item)
											}
											className='text-red-500'>
											🗑️
										</button>
									</td>
								</tr>
							))}
						</tbody>
					</table>
				</div>
			)}

			{tab === 'pending' && (
				<div className='bg-white p-5 rounded-xl shadow-sm border'>
					<table className='w-full text-sm text-left'>
						<thead>
							<tr className='border-b bg-gray-50'>
								<th className='p-3'>Tiêu đề</th>
								<th className='p-3'>Người yêu cầu</th>
								<th className='p-3'>Thao tác</th>
							</tr>
						</thead>
						<tbody>
							{filteredPending.map((item) => (
								<tr
									key={item.id}
									className='border-b hover:bg-gray-50'>
									<td className='p-3'>{item.title}</td>
									<td className='p-3'>
										{item.requesterName}
									</td>
									<td className='p-3 flex gap-3'>
										{canApprove && (
											<>
												<button
													onClick={() =>
														handleApprove(item.id)
													}
													className='text-green-500'>
													✅
												</button>
												<button
													onClick={() =>
														setRejectTarget(item)
													}
													className='text-red-500'>
													❌
												</button>
											</>
										)}
										<button
											onClick={() =>
												setPreviewTarget(item)
											}
											className='text-blue-500'>
											👁️
										</button>
									</td>
								</tr>
							))}
						</tbody>
					</table>
				</div>
			)}

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
			{editTarget && (
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
			{previewTarget && (
				<PreviewModal
					item={previewTarget}
					canApprove={canApprove}
					onClose={() => setPreviewTarget(null)}
					onApprove={() => {
						handleApprove(previewTarget.id)
						setPreviewTarget(null)
					}}
					onReject={() => {
						setRejectTarget(previewTarget)
						setPreviewTarget(null)
					}}
				/>
			)}
			{popup && (
				<PopupMessage message={popup} onClose={() => setPopup(null)} />
			)}
		</div>
	)
}
export default AdminNewsManagement
