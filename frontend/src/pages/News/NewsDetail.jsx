import React, { useState, useEffect, useRef } from 'react'
import { useParams, useLocation } from 'react-router-dom'
import newsApi from '../../api/newsApi'
import { formatDateTime, timeAgo } from '../../helpers/newsHelpers'
import PopupMessage from '../../components/parts/PopupMessage'
import Button from '../../components/parts/Button'

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
			style={{ position: 'fixed', top: y, left: x, zIndex: 1000 }}
			className='bg-white border rounded-xl shadow-xl py-1 min-w-[160px]'>
			<button
				onClick={onDelete}
				className='w-full text-left px-4 py-2 text-sm text-red-500 hover:bg-red-50'>
				Xóa bình luận
			</button>
		</div>
	)
}

const ConfirmDeleteModal = ({ onConfirm, onCancel }) => (
	<div className='fixed inset-0 bg-black/40 flex items-center justify-center z-50'>
		<div className='bg-white rounded-xl p-6 w-full max-w-sm text-center'>
			<h3 className='font-bold mb-2'>Xóa bình luận</h3>
			<div className='flex gap-2 mt-4'>
				<button
					onClick={onCancel}
					className='flex-1 border rounded py-2'>
					Hủy
				</button>
				<button
					onClick={onConfirm}
					className='flex-1 bg-red-500 text-white rounded py-2'>
					Đồng ý
				</button>
			</div>
		</div>
	</div>
)

const NewsDetail = () => {
	const { id } = useParams()
	const location = useLocation()

	const [news, setNews] = useState(location.state || null)
	const [comments, setComments] = useState([])

	const [commentText, setCommentText] = useState('')
	const [popup, setPopup] = useState(null)
	const [error, setError] = useState(null)

	const [popover, setPopover] = useState(null)
	const [confirmDelete, setConfirmDelete] = useState(null)

	const role = localStorage.getItem('role')
	const isAdmin =
		role === 'Admin' ||
		role === 'Admin1' ||
		role === 'Admin2' ||
		role === 'Admin3'

	// ===== LOAD NEWS VÀ COMMENTS GỘP CHUNG =====
	const loadData = async () => {
		try {
			const data = await newsApi.getById(id)
			setNews(data)
			setComments(data.comments || []) // Comment được lấy từ trong object news
		} catch {
			setError('Không tìm thấy bài viết')
		}
	}

	useEffect(() => {
		if (id) loadData()
	}, [id])

	// ===== POST COMMENT =====
	const handleComment = async () => {
		if (!commentText.trim()) {
			setPopup('Không được để trống.')
			return
		}
		try {
			// Payload chuẩn của Backend (Có thể đổi msv và tên tuỳ logic user của bạn)
			await newsApi.postComment(id, {
				msv: 'Guest',
				studentName: 'Khách viếng thăm',
				content: commentText
			})
			setCommentText('')
			await loadData() // Tải lại để thấy comment mới
		} catch (err) {
			setPopup(err.response?.data?.message || 'Có lỗi xảy ra')
		}
	}

	// ===== DELETE FLOW =====
	const handleCommentClick = (e, commentId) => {
		if (!isAdmin) return
		e.preventDefault()
		setPopover({ x: e.clientX, y: e.clientY, commentId })
	}

	const handleConfirmDelete = async () => {
		try {
			await newsApi.deleteComment(confirmDelete)
			await loadData()
			setPopup('Đã xóa bình luận')
		} catch {
			setPopup('Xóa thất bại')
		} finally {
			setConfirmDelete(null)
		}
	}

	if (error) return <div className='p-10 text-center'>{error}</div>
	if (!news) return <div className='p-10 text-center'>Loading...</div>

	return (
		<div className='max-w-5xl mx-auto p-6'>
			<Button link='/news' message='Quay lại' />
			<h1 className='text-2xl font-bold mt-4'>{news.title}</h1>
			<div className='text-sm text-gray-500 mt-2 flex gap-4'>
				<span>Đăng bởi: {news.createdBy}</span>
				<span>{formatDateTime(news.createdAt)}</span>
				<span>Lượt xem: {news.viewCount}</span>
			</div>
			<div className='mt-6 whitespace-pre-wrap text-gray-800 leading-relaxed'>
				{news.content}
			</div>

			{/* COMMENTS */}
			<div className='mt-10 pt-6 border-t'>
				<h3 className='font-semibold mb-4'>
					Bình luận ({comments.length})
				</h3>
				{!isAdmin && (
					<div className='mb-6'>
						<textarea
							value={commentText}
							onChange={(e) => setCommentText(e.target.value)}
							placeholder='Nhập bình luận của bạn...'
							className='w-full border p-3 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500'
						/>
						<button
							onClick={handleComment}
							className='mt-2 px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-xl transition'>
							Gửi bình luận
						</button>
					</div>
				)}
				<div className='space-y-4'>
					{comments.map((c) => (
						<div
							key={c.commentID}
							onClick={(e) => handleCommentClick(e, c.commentID)}
							className={`p-4 rounded-xl border ${isAdmin ? 'cursor-pointer hover:bg-red-50 hover:border-red-200' : 'bg-gray-50'}`}>
							<div className='text-sm font-bold text-gray-800'>
								{c.studentName}
							</div>
							<div className='text-sm text-gray-600 mt-1'>
								{c.content}
							</div>
							<div className='text-xs text-gray-400 mt-2'>
								{timeAgo(c.createdAt)}
							</div>
						</div>
					))}
				</div>
			</div>

			{popup && (
				<PopupMessage message={popup} onClose={() => setPopup(null)} />
			)}
			{popover && (
				<CommentPopover
					x={popover.x}
					y={popover.y}
					onDelete={() => {
						setConfirmDelete(popover.commentId)
						setPopover(null)
					}}
					onClose={() => setPopover(null)}
				/>
			)}
			{confirmDelete && (
				<ConfirmDeleteModal
					onConfirm={handleConfirmDelete}
					onCancel={() => setConfirmDelete(null)}
				/>
			)}
		</div>
	)
}

export default NewsDetail
