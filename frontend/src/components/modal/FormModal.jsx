import Modal from './Modal'
import SubjectSelect from '../parts/SubjectSelect'
import SubjectMultiSelect from '../parts/SubjectMultiSelect'

function FormModal({
	title,
	fields,
	defaultValues,
	onSubmit,
	onClose,
	confirmText = 'Xác nhận',
	columns = 2,
	width = '750px',
}) {
	const handleSubmit = (e) => {
		e.preventDefault()
		const formData = new FormData(e.target)
		// const formData = Object.fromEntries(formData.entries())
		onSubmit(formData)
	}
	const isEdit = !!defaultValues

	return (
		<Modal title={title} width={width}>
			<form
				onSubmit={handleSubmit}
				className='flex flex-col max-h-[80vh]'>
				{/* BODY */}
				<div className='overflow-y-auto pr-2 flex-1'>
					<div
						className={`grid gap-4 ${
							columns === 3
								? 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3'
								: columns === 2
									? 'grid-cols-1 sm:grid-cols-2'
									: 'grid-cols-1'
						}`}>
						{fields.map((field) => (
							<div
								key={field.name}
								className={
									field.fullWidth
										? 'col-span-full'
										: field.colSpan === 2
											? 'sm:col-span-2'
											: ''
								}>
								<label className='text-sm font-medium text-gray-700 block mb-1'>
									{field.label}
									{field.required && (
										<span className='text-red-500 ml-1'>
											*
										</span>
									)}
								</label>

								{/* SELECT */}
								{field.type === 'select' ? (
									<select
										name={field.name}
										required={field.required}
										defaultValue={
											defaultValues?.[field.name] ?? ''
										}
										className='w-full border border-gray-300 rounded-lg p-2'>
										{field.options.map((opt) => (
											<option
												key={opt.value}
												value={opt.value}>
												{opt.label}
											</option>
										))}
									</select>
								) : field.key === 'subjectCode' ||
								  field.name === 'subjectCode' ? (
									<>
										<input
											type='hidden'
											name={field.name}
											defaultValue={
												defaultValues?.[field.name] ??
												''
											}
										/>
										<SubjectSelect
											value={
												defaultValues?.[field.name] ??
												''
											}
											onChange={(value) => {
												document.querySelector(
													`[name="${field.name}"]`
												).value = value
											}}
										/>
									</>
								) : field.key === 'subjectCodes' ||
								  field.name === 'subjectCodes' ? (
									<SubjectMultiSelect
										values={
											defaultValues?.[field.name] || []
										}
										onChange={(values) => {
											document.querySelector(
												`[name="${field.name}"]`
											).value = JSON.stringify(values)
										}}
									/>
								) : field.type === 'textarea' ? (
									<textarea
										name={field.name}
										required={field.required}
										rows={field.rows || 4}
										defaultValue={
											defaultValues?.[field.name] ??
											defaultValues?.[
												field.name
													.charAt(0)
													.toUpperCase() +
													field.name.slice(1)
											] ??
											''
										}
										className='w-full border border-gray-300 rounded-lg p-2 resize-y focus:ring-2 focus:ring-[#1f4c7a] focus:border-[#1f4c7a] outline-none'
									/>
								) : field.type === 'file' ? (
									<input
										type='file'
										name={field.name}
										required={field.required}
										className='w-full border border-gray-300 rounded-lg p-1.5 file:mr-4 file:py-1.5 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-[#1f4c7a] hover:file:bg-blue-100 cursor-pointer'
									/>
								) : (
									<input
										name={field.name}
										type={field.type || 'text'}
										required={field.required}
										readOnly={field.readOnly && isEdit}
										placeholder={field.placeholder}
										disabled={field.disabled && isEdit}
										defaultValue={
											defaultValues?.[field.name] ?? ''
										}
										className={`w-full border rounded-lg p-2 focus:ring-2 focus:ring-[#1f4c7a] outline-none ${
											field.readOnly
												? 'bg-gray-100 text-gray-500 border-gray-200'
												: 'border-gray-300 focus:border-[#1f4c7a]'
										}`}
									/>
								)}
							</div>
						))}
					</div>
				</div>

				{/* FOOTER */}
				<div className='flex justify-end gap-3 mt-5 pt-4 border-t sticky bottom-0 bg-white'>
					<button
						type='button'
						onClick={onClose}
						className='px-4 py-2 border rounded-lg hover:bg-gray-50 transition font-medium'>
						Hủy
					</button>

					<button
						type='submit'
						className='px-4 py-2 bg-[#1f4c7a] text-white rounded-lg hover:bg-[#163a5d] transition shadow-sm font-medium'>
						{confirmText}
					</button>
				</div>
			</form>
		</Modal>
	)
}

export default FormModal
