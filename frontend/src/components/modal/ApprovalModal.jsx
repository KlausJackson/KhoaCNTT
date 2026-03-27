import { useState } from 'react'
import Modal from './Modal'

function ApprovalModal({ title, details, onConfirm, onClose }) {
	const [reason, setReason] = useState('')

	return (
		<Modal
			title={title}
			width='700px'>
			<div className='bg-slate-50 p-4 rounded-lg mb-4 text-sm space-y-2 border'>
				{details.map((item, index) => (
					<p
						key={index}
						className='flex border-b border-dashed pb-1 last:border-0 last:pb-0'>
						<span className='font-medium text-gray-600 w-1/3'>
							{item.label}:
						</span>
						<span className='w-2/3 text-gray-900 font-semibold break-words'>
							{item.value || '-'}
						</span>
					</p>
				))}
			</div>

			<textarea
				placeholder='Lý do (nếu có)...'
				value={reason}
				onChange={(e) => setReason(e.target.value)}
				className='border rounded-lg p-3 w-full mt-2 focus:ring-2 focus:ring-blue-500 focus:outline-none'
				rows={3}
			/>

			<div className='flex justify-end gap-3 mt-4 pt-4 border-t'>
				<button
					className='px-4 py-2 border rounded-lg hover:bg-gray-50 transition font-medium'
					onClick={onClose}>
					Hủy bỏ
				</button>
				<button
					className='bg-red-50 text-red-600 border border-red-200 px-4 py-2 rounded-lg hover:bg-red-100 transition font-medium'
					onClick={() => onConfirm(false, reason)}>
					Từ chối
				</button>

				<button
					className='bg-[#1f4c7a] text-white px-4 py-2 rounded-lg hover:bg-[#163a5d] transition shadow-sm font-medium'
					onClick={() => onConfirm(true, reason)}>
					Chấp nhận
				</button>
			</div>
		</Modal>
	)
}

export default ApprovalModal
