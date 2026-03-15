import { normalizeFileSize } from '../../helpers/fileHelpers'
import { normalizeDate } from '../../helpers/helpers'
import { FilePlus, Pencil, Trash2 } from 'lucide-react'

function DataTable({ columns = [], data = [], actions, rowKey = 'id' }) {
	const renderRequestIcon = (type) => {
		switch (type) {
			case 'CreateNew':
				return <FilePlus className='w-4 h-4 text-green-600' />
			case 'Update':
				return <Pencil className='w-4 h-4 text-blue-600' />
			case 'Delete':
				return <Trash2 className='w-4 h-4 text-red-600' />
			default:
				return <span className='text-gray-400'>-</span>
		}
	}

	const getWidth = (col) => {
		if (col.width) return col.width

		if (col.key === 'createdAt') return '120px'

		if (col.type) return '35px'

		return 'auto'
	}

	return (
		<div className='bg-white border rounded-xl overflow-hidden shadow-sm'>
			<table className='w-full text-sm table-fixed'>
				<thead className='bg-gray-50 border-b'>
					<tr>
						{columns.map((col) => (
							<th
								key={col.key}
								style={{
									width:
										getWidth(col)
								}}
								className='text-left px-4 py-3 font-semibold text-gray-700'>
								{col.label}
							</th>
						))}
						{actions && (
							<th className='px-4 py-3 text-center w-32'>
								Thao tác
							</th>
						)}
					</tr>
				</thead>

				{/* BODY */}
				<tbody className='divide-y'>
					{data.length === 0 && (
						<tr>
							<td
								colSpan={columns.length + (actions ? 1 : 0)}
								className='text-center py-6 text-gray-500'>
								Không có dữ liệu
							</td>
						</tr>
					)}

					{data.map((row, index) => (
						<tr
							key={row[rowKey] ?? index}
							className='hover:bg-gray-50'>
							{columns.map((col) => {
								let value = row[col.key]

								// DATE FORMAT
								if (col.type === 'date') {
									value = normalizeDate(value)
								}

								// REQUEST TYPE ICON
								if (col.type === 'requestType') {
									value = renderRequestIcon(value)
								}

								// Normalize file size
								if (col.key.toLowerCase().includes('size')) {
									value = normalizeFileSize(value)
								}

								return (
									<td
										key={col.key}
										className='px-4 py-3 max-w-[220px] truncate'>
										{col.render
											? col.render(value, row, index)
											: (value ?? '-')}
									</td>
								)
							})}

							{actions && (
								<td className='px-4 py-3'>
									<div className='flex justify-center gap-2'>
										{actions(row)}
									</div>
								</td>
							)}
						</tr>
					))}
				</tbody>
			</table>
		</div>
	)
}

export default DataTable
