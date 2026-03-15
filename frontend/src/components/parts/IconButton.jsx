import { Icon } from '@iconify/react'

function IconButton({ icon, color, onClick, message = '' }) {
	return (
		<button
			onClick={onClick}
			className={`p-2 rounded-lg border border-gray-200 hover:bg-${color}-50`}>
			<Icon icon={icon} width={18} />
			{message}
		</button>
	)
}

export default IconButton
