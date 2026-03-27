function Modal({ title, children, width }) {
	return (
		<div className='fixed inset-0 flex items-center justify-center bg-black/40 z-50'>
			<div
				className='bg-white rounded-xl shadow-xl px-6 py-6'
				style={{ width }}>
				<div className='flex justify-between items-center mb-4'>
					<h2 className='font-semibold text-lg'>{title}</h2>

					{/* <button onClick={onClose}>✕</button> */}
				</div>

				{children}
			</div>
		</div>
	)
}

export default Modal
