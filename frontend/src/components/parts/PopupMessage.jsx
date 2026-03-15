const PopupMessage = ({ message, onClose }) => {
	if (!message) return null

	return (
		<div className='fixed bottom-6 right-6 z-50'>
			<div className='flex items-start gap-3 bg-white border border-red-200 shadow-lg rounded-lg px-4 py-3 min-w-[260px] animate-slide-in'>
				<div className='flex-1'>
					<p className='text-sm text-gray-600'>{message}</p>
				</div>
				<button
					onClick={onClose}
					className='text-gray-400 hover:text-gray-600 text-sm'>
					✕
				</button>
			</div>
		</div>
	)
}

export default PopupMessage
