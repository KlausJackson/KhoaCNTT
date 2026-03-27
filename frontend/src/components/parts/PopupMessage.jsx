import { useEffect } from 'react'
import { Icon } from '@iconify/react'

const PopupMessage = ({ message, onClose, type }) => {
	useEffect(() => {
		const timer = setTimeout(() => {
			onClose()
		}, 3000)

		return () => clearTimeout(timer) // cleanup
	}, [message])
	
	if (!message) return null

	return (
		<div className='fixed bottom-6 right-6 z-50'>
			<div className='flex items-start gap-3 bg-white border border-red-200 shadow-lg rounded-lg px-4 py-3 min-w-[260px] animate-slide-in'>
				<div className='flex-1'>
					<p className='text-sm text-gray-600'>
						<Icon
							icon={
								type === 'success'
									? 'mdi:check-circle-outline'
									: 'mdi:alert-circle-outline'
							}
							className={`inline-block mr-2 text-lg ${
								type === 'success' ? 'text-green-500' : 'text-red-500'
							}`}
						/>
						{message}</p>
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
