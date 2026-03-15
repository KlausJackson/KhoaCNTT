import { Link } from 'react-router-dom'

function Button({ link, message }) {
	return (
		<Link
			to={link}
			className='inline-flex items-center gap-2 mb-6 px-4 py-2 rounded-lg border border-gray-200 bg-blue-950 shadow-sm hover:shadow-md hover:bg-blue-900 transition text-white font-medium'>
			<span className='text-lg'>←</span>
			<span>{message}</span>
		</Link>
	)
}

export default Button
