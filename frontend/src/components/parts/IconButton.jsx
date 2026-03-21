import { Icon } from '@iconify/react'

function IconButton({ icon: Icon, color, onClick, message = '' }) {
	return (
		<button
			onClick={onClick}
			className={`p-2 rounded-lg border border-gray-200 hover:bg-${color}-50`}>
			<Icon width={18} strokeWidth={2} />
			{message}
		</button>
	)
}

export default IconButton
