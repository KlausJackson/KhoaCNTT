import { Icon } from '@iconify/react'
import Modal from './Modal'

const colorMap = {
	green: 'bg-green-500 hover:bg-green-600',
	red: 'bg-red-500 hover:bg-red-600',
	blue: 'bg-blue-500 hover:bg-blue-600',
	yellow: 'bg-yellow-500 hover:bg-yellow-600'
}

const iconBgMap = {
	green: 'bg-green-100 text-green-600',
	red: 'bg-red-100 text-red-600',
	blue: 'bg-blue-100 text-blue-600',
	yellow: 'bg-yellow-100 text-yellow-600'
}

function ConfirmModal({
	title,
	message,
	confirmText,
	color = 'green',
	icon,
	onConfirm,
	onClose
}) {
	return (
		<Modal width='450px'>
			<div className='text-center px-2 py-4'>
				{/* Icon circle */}
				<div className={`w-16 h-16 mx-auto mb-4 flex items-center justify-center rounded-full ${iconBgMap[color]}`}>
					<Icon icon={icon} className='text-3xl' />
				</div>

				{/* Title */}
				<h2 className='text-lg font-semibold mb-2'>{title}</h2>

				{/* Message */}
				<p className='text-gray-500 mb-6'>{message}</p>

				{/* Actions */}
				<div className='flex gap-3'>
					<button
						onClick={onClose}
						className='flex-1 px-4 py-2 border rounded-lg hover:bg-gray-50'>
						Hủy bỏ
					</button>

					<button
						onClick={onConfirm}
						className={`flex-1 px-4 py-2 text-white rounded-lg flex items-center justify-center gap-2 ${colorMap[color]}`}>
						<Icon icon={icon} />
						{confirmText}
					</button>
				</div>
			</div>
		</Modal>
	)
}

export default ConfirmModal
