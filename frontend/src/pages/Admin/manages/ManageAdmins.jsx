import { useEffect, useState } from 'react'
import DataTable from '../../../components/table/DataTable'
import IconButton from '../../../components/parts/IconButton'
import FormModal from '../../../components/modal/FormModal'
import adminApi from '../../../api/adminApi'
import { columns, fields } from '../../../constants/admin'
import PopupMessage from '../../../components/parts/PopupMessage'
import ConfirmModal from '../../../components/modal/ConfirmModal'
import { handleAction } from '../../../helpers/commonHelpers'
import { Icon } from '@iconify/react'

import { UserCog, Lock, Unlock, Trash2 } from 'lucide-react'

function ManageAdmin() {
	const [admins, setAdmins] = useState([])
	const [showCreate, setShowCreate] = useState(false)
	const [popup, setPopup] = useState(null)
	const [editingAdmin, setEditingAdmin] = useState(null)
	const [warning, setWarning] = useState(null)
	// warning: handleAction,

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
		(async () => {
			await loadAdmins()
		})()
	}, [])

	return (
		<div className='p-6'>
			<div className='flex justify-between mb-6'>
				<h1 className='text-xl font-semibold'>
					Quản lý Tài khoản Quản trị viên
				</h1>

				<button
					onClick={() => setShowCreate(true)}
					className='bg-blue-600 text-white px-4 py-2 rounded-lg'>
					Thêm tài khoản mới
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

						<IconButton
							icon={row.isActive === 1 ? Lock : Unlock}
							onClick={() => {
								const isActive = row.isActive === 1
								setWarning({
									color: isActive ? 'yellow' : 'green',
									title:
										'Xác nhận ' +
										(isActive
											? 'vô hiệu hóa tài khoản'
											: 'mở khóa tài khoản'),
									message:
										'Bạn có chắc chắn muốn ' +
										(isActive ? 'vô hiệu hóa' : 'mở khóa') +
										' tài khoản này?',
									action: async () =>
										await adminApi.update({
											id: row.id,
											fullName: row.fullName,
											email: row.email,
											level: row.level,
											isActive: isActive ? false : true
										}),
									popup: isActive
										? 'Vô hiệu hóa tài khoản thành công.'
										: 'Mở khóa tài khoản thành công.',
									icon: isActive
										? 'mdi:lock-outline'
										: 'mdi:lock-open-outline'
								})
							}}
						/>

						<IconButton
							icon={Trash2}
							onClick={() => {
								setWarning({
									title: 'Xác nhận xóa tài khoản',
									message:
										'Bạn có chắc chắn muốn xóa tài khoản này?',
									action: () => adminApi.delete(row.id),
									popup: 'Xóa tài khoản thành công.',
									color: 'red',
									icon: "mdi:delete-outline"
								})
							}}
						/>
					</>
				)}
			/>

			{showCreate && (
				<FormModal
					title='Thêm tài khoản quản trị viên'
					fields={fields}
					confirmText='Tạo tài khoản'
					onSubmit={(data) => {
						const formData = Object.fromEntries(data.entries())
						formData.level = Number(formData.level)
						formData.isActive = Boolean(formData.isActive)
						handleAction(
							adminApi.create,
							formData,
							setShowCreate,
							loadAdmins,
							setPopup,
						)
					}
						
					}
					onClose={() => setShowCreate(false)}
				/>
			)}

			{editingAdmin && (
				<FormModal
					title='Cập nhật thông tin tài khoản'
					fields={fields.filter(f => f.name !== "isActive")}
					defaultValues={editingAdmin}
					confirmText='Lưu thay đổi'
					onSubmit={(formData) => {
						const data = Object.fromEntries(formData.entries())
						data.level = Number(data.level)
						
						handleAction(
							adminApi.update,
							{ id: editingAdmin.id, ...data },
							setEditingAdmin,
							loadAdmins,
							setPopup,
						)
					}
						
					}
					onClose={() => setEditingAdmin(null)}
				/>
			)}
			{popup && (
				<PopupMessage onClose={() => setPopup(null)} message={popup.message} type={popup.type} />
			)}

			{warning && (
				<ConfirmModal
					title={warning.title}
					message={warning.message}
					color={warning.color}
					icon={warning.icon}
					onConfirm={async () => {
						await handleAction(
							warning.action,
							null,
							setWarning,
							loadAdmins,
							setPopup,
							warning.popup
						)
					}}
					onClose={() => setWarning(null)}
					confirmText='Xác nhận'
				/>
			)}
		</div>
	)
}

export default ManageAdmin
