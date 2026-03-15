import { useEffect, useState } from 'react'
import DataTable from '../../../components/table/DataTable'
import IconButton from '../../../components/parts/IconButton'
import FormModal from '../../../components/modal/FormModal'
import adminApi from '../../../api/adminApi'
import { columns, fields } from '../../../constants/admin'
import PopupMessage from '../../../components/parts/PopupMessage'
import ConfirmModal from '../../../components/modal/ConfirmModal'
import { handleAction } from '../../../helpers/helpers'

import { UserCog, Lock, Unlock, Trash2 } from 'lucide-react'

function ManageAdmin() {
	const [admins, setAdmins] = useState([])
	const [showCreate, setShowCreate] = useState(false)
	const [popup, setPopup] = useState(null)
	const [editingAdmin, setEditingAdmin] = useState(null)
	const [warning, setWarning] = useState(null)

	const loadAdmins = async () => {
		try {
			const res = await adminApi.getAll()
			const filteredAdmins = res.filter((admin) => admin.level !== 1)
			setAdmins(filteredAdmins)
		} catch (err) {
			console.error(err)
		}
	}

	useEffect(() => {
		const fetch = async () => {
			const res = await adminApi.getAll()
			const filteredAdmins = res.filter((admin) => admin.level !== 1)
			setAdmins(filteredAdmins)
		}
		fetch()
	}, [])

	return (
		<div className='p-6'>
			<div className='flex justify-between mb-6'>
				<h1 className='text-xl font-semibold'>
					Quản lý tài khoản quản trị viên
				</h1>

				<button
					onClick={() => setShowCreate(true)}
					className='bg-blue-600 text-white px-4 py-2 rounded-lg'>
					+ Thêm tài khoản
				</button>
			</div>

			<DataTable
				columns={columns}
				data={admins}
				actions={(row) => (
					<>
						<IconButton
							icon={UserCog}
							onClick={() => {
								setEditingAdmin(row)
							}}
						/>

						{row.isActive === 1 ? (
							<IconButton
								icon={Lock}
								onClick={() =>
									handleAction(adminApi.update, {
										id: row.id,
										fullName: row.fullName,
										email: row.email,
										level: row.level,
										isActive: false
									})
								}
							/>
						) : (
							<IconButton
								icon={Unlock}
								onClick={() =>
									handleAction(adminApi.update, {
										id: row.id,
										fullName: row.fullName,
										email: row.email,
										level: row.level,
										isActive: true
									}, null, loadAdmins, setPopup)
								}
							/>
						)}

						<IconButton
							icon={Trash2}
							onClick={() => {
								setWarning(row.id)
							}}
						/>
					</>
				)}
			/>

			{showCreate && (
				<FormModal
					title='Thêm tài khoản quản trị'
					fields={fields}
					onSubmit={(data) =>
						handleAction(adminApi.create, data, setShowCreate, loadAdmins, setPopup)
					}
					onClose={() => setShowCreate(false)}
				/>
			)}

			{editingAdmin && (
				<FormModal
					title='Cập nhật tài khoản'
					fields={fields}
					defaultValues={editingAdmin}
					onSubmit={(data) =>
						handleAction(
							adminApi.update,
							{ id: editingAdmin.id, ...data },
							setEditingAdmin,
							loadAdmins,
							setPopup
						)
					}
					onClose={() => setEditingAdmin(null)}
				/>
			)}
			{popup && (
				<PopupMessage onClose={() => setPopup(null)} message={popup} />
			)}
			{warning && (
				<ConfirmModal
					title='Xác nhận xóa'
					message='Bạn có chắc chắn muốn xóa tài khoản này?'
					onConfirm={async () => {
						await handleAction(adminApi.delete, warning, setWarning, loadAdmins, setPopup)
					}}
					onClose={() => setWarning(null)}
					confirmText='Xóa'
					color='red'
				/>
			)}
		</div>
	)
}

export default ManageAdmin
