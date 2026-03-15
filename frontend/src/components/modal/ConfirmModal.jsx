import Modal from './Modal'

function ConfirmModal({
	title,
	message,
	confirmText,
	color,
	onConfirm,
	onClose,
}) {
	return (
		<Modal title={title} onClose={onClose}>
			<p className='text-gray-600 mb-6'>{message}</p>

			<div className='flex justify-end gap-3'>
				<button
					onClick={onClose}
					className='px-4 py-2 border rounded-lg'>
					Hủy
				</button>

				<button
					onClick={onConfirm}
					className={`px-4 py-2 rounded-lg bg-${color}-500`}>
					{confirmText}
				</button>
			</div>
		</Modal>
	)
}

export default ConfirmModal
