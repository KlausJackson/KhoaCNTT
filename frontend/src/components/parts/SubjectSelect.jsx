import { useState, useEffect } from 'react'
import studentApi from '../../api/studentApi'

function SubjectSelect({ value, onChange }) {
	const [keyword, setKeyword] = useState('')
	const [subjects, setSubjects] = useState([])

	useEffect(() => {
		studentApi.getSubjects().then(setSubjects).catch(console.error)
	}, [])

	useEffect(() => {
		if (value && subjects.length) {
			const s = subjects.find((x) => x.subjectCode === value)
			if (s) {
				setKeyword(`${s.subjectCode} – ${s.subjectName}`)
			}
		}
	}, [value, subjects])

	const filtered = subjects.filter((s) =>
		(s.subjectCode + ' ' + s.subjectName)
			.toLowerCase()
			.includes(keyword.toLowerCase())
	)

	return (
		<div className='border border-gray-300 rounded-lg p-2 bg-white'>
			<input
				type='text'
				value={keyword}
				placeholder='Tìm mã hoặc tên môn...'
				onChange={(e) => {
					const val = e.target.value
					setKeyword(val)
					onChange(val)
				}}
				className='w-full border-b mb-2 pb-1 text-sm outline-none'
			/>

			<div className='max-h-40 overflow-y-auto space-y-1'>
				{filtered.map((s) => (
					<div
						key={s.subjectCode}
						onClick={() => {
							onChange(s.subjectCode || '')
							setKeyword(`${s.subjectCode} – ${s.subjectName}`)
						}}
						className={`px-2 py-1 rounded cursor-pointer text-sm hover:bg-gray-100
						${value === s.subjectCode ? 'bg-blue-100' : ''}`}>
						<span className='font-medium'>{s.subjectCode}</span>{' '}
						<span className='text-gray-500'>– {s.subjectName}</span>
					</div>
				))}
			</div>
			{value && (
				<button
					type='button'
					onClick={() => {
						setKeyword('')
						onChange('')
					}}
					className='ml-2 text-gray-400 hover:text-red-500'>
					✕
				</button>
			)}
		</div>
	)
}

export default SubjectSelect
