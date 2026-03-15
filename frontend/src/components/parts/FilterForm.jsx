import React, { useState } from 'react'
import { Search, ChevronDown, ChevronUp } from 'lucide-react'

function FilterForm({ fields, onSearch }) {
	const [expanded, setExpanded] = useState(false)

	const [values, setValues] = useState(() => {
		const initial = {}
		fields.forEach((f) => {
			initial[f.name] =
				f.defaultValue || (f.type === 'checkbox-group' ? [] : '')
		})
		return initial
	})

	const handleChange = (name, val) => {
		setValues((prev) => ({ ...prev, [name]: val }))
	}

	const handleSubmit = (e) => {
		e.preventDefault()
		onSearch(values)
	}

	return (
		<div className='mb-6 bg-slate-50 rounded-xl border border-gray-200'>
			{/* HEADER */}
			<div
				className='flex justify-between items-center px-5 py-3 cursor-pointer'
				onClick={() => setExpanded(!expanded)}>
				<div className='text-sm font-semibold text-gray-600'>
					Bộ lọc
				</div>

				<div className='flex items-center gap-1 text-sm text-gray-500'>
					{expanded ? (
						<>
							Thu gọn <ChevronUp size={16} />
						</>
					) : (
						<>
							Mở rộng <ChevronDown size={16} />
						</>
					)}
				</div>
			</div>

			{/* FILTER BODY */}
			{expanded && (
				<form
					onSubmit={handleSubmit}
					className='px-5 pb-5 pt-2 border-t border-gray-200'>
					<div className='flex flex-wrap items-end gap-4'>
						{fields.map((field) => (
							<div
								key={field.name}
								className={`${field.width || 'flex-1 min-w-[200px]'}`}>
								<label className='text-xs text-gray-500 font-semibold mb-1.5 block uppercase tracking-wider'>
									{field.label}
								</label>

								{field.type === 'select' ? (
									<select
										value={values[field.name]}
										onChange={(e) =>
											handleChange(
												field.name,
												e.target.value
											)
										}
										className='w-full px-3 py-2 border border-gray-300 bg-white rounded-lg focus:ring-2 focus:ring-[#1f4c7a] focus:border-[#1f4c7a] text-sm'>
										{field.options.map((opt) => (
											<option
												key={opt.value}
												value={opt.value}>
												{opt.label}
											</option>
										))}
									</select>
								) : field.type === 'checkbox-group' ? (
									<div className='border border-gray-300 rounded-lg p-2 bg-white h-[160px] overflow-y-auto'>
										{field.options.map((opt) => (
											<label
												key={opt.value}
												className='flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 px-1 rounded'>
												<input
													type='checkbox'
													checked={values[
														field.name
													].includes(opt.value)}
													onChange={(e) => {
														const checked =
															e.target.checked
														const current =
															values[field.name]

														handleChange(
															field.name,
															checked
																? [
																		...current,
																		opt.value
																	]
																: current.filter(
																		(v) =>
																			v !==
																			opt.value
																	)
														)
													}}
													className='rounded border-gray-300 text-[#1f4c7a]'
												/>
												<span>{opt.label}</span>
											</label>
										))}
									</div>
								) : (
									<input
										type={field.type || 'text'}
										placeholder={field.placeholder}
										value={values[field.name]}
										onChange={(e) =>
											handleChange(
												field.name,
												e.target.value
											)
										}
										className='w-full px-3 py-2 border border-gray-300 bg-white rounded-lg focus:ring-2 focus:ring-[#1f4c7a] focus:border-[#1f4c7a] text-sm'
									/>
								)}
							</div>
						))}

						<div className='flex-none'>
							<button
								type='submit'
								className='h-[42px] bg-[#1f4c7a] text-white px-6 rounded-lg hover:bg-[#163a5d] transition shadow-sm font-medium flex items-center gap-2'>
								<Search size={18} /> Tìm
							</button>
						</div>
					</div>
				</form>
			)}
		</div>
	)
}

export default FilterForm
