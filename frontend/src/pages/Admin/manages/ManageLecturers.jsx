import { useEffect, useState } from 'react'
import DataTable from '../../../components/table/DataTable'
import FormModal from '../../../components/modal/FormModal'
import ConfirmModal from '../../../components/modal/ConfirmModal'
import PopupMessage from '../../../components/parts/PopupMessage'
import IconButton from '../../../components/parts/IconButton'
import { handleAction } from '../../../helpers/helpers'

import lecturerApi from '../../../api/lecturerApi'
import { columns, fields } from '../../../constants/lecturer'

import { UserCog, Trash2 } from 'lucide-react'

function ManageLecturers() {
	const [lecturers, setLecturers] = useState([])
	const [showCreate, setShowCreate] = useState(false)
	const [editing, setEditing] = useState(null)
	const [warning, setWarning] = useState(null)
	const [popup, setPopup] = useState(null)

	const loadLecturers = async () => {
		try {
			const res = await lecturerApi.getAll()
			setLecturers(res.items ?? res)
		} catch (err) {
			console.error(err)
		}
	}

	useEffect(() => {
		const loadLecturers = async () => {
			try {
				const res = await lecturerApi.getAll()
				setLecturers(res.items ?? res)
			} catch (err) {
				console.error(err)
			}
		}
		loadLecturers()
	}, [])

	return (
		<div className='p-6'>
			<div className='flex justify-between mb-6'>
				<h1 className='text-xl font-semibold'>Quản lý giảng viên</h1>

				<button
					onClick={() => setShowCreate(true)}
					className='bg-blue-600 text-white px-4 py-2 rounded-lg'>
					+ Thêm giảng viên
				</button>
			</div>

			<DataTable
				columns={columns}
				data={lecturers}
				actions={(row) => (
					<>
						<IconButton
							icon={UserCog}
							onClick={() => setEditing(row)}
						/>

						<IconButton
							icon={Trash2}
							onClick={() => setWarning(row.id)}
						/>
					</>
				)}
			/>

			{showCreate && (
				<FormModal
					title='Thêm giảng viên'
					fields={fields}
					columns={3}
					width='700px'
					onSubmit={(data) =>
						handleAction(
							lecturerApi.create,
							data,
							setShowCreate,
							loadLecturers,
							setPopup
						)
					}
					onClose={() => setShowCreate(false)}
				/>
			)}

			{editing && (
				<FormModal
					title='Cập nhật giảng viên'
					fields={fields}
					columns={3}
					width='700px'
					defaultValues={editing}
					onSubmit={(data) =>
						handleAction(
							lecturerApi.update,
							{ id: editing.id, ...data },
							setEditing,
							loadLecturers,
							setPopup
						)
					}
					onClose={() => setEditing(null)}
				/>
			)}

			{warning && (
				<ConfirmModal
					title='Xác nhận xóa'
					message='Bạn có chắc chắn muốn xóa giảng viên này?'
					confirmText='Xóa'
					onConfirm={() =>
						handleAction(
							lecturerApi.delete,
							warning,
							setWarning,
							loadLecturers,
							setPopup
						)
					}
					onClose={() => setWarning(null)}
				/>
			)}

			{popup && (
				<PopupMessage message={popup} onClose={() => setPopup(null)} />
			)}
		</div>
	)
}

export default ManageLecturers
