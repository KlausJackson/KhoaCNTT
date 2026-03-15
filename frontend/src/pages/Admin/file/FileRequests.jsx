import { useEffect, useState } from 'react'
import fileApi from '../../../api/fileApi'
import DataTable from '../../../components/table/DataTable'
import IconButton from '../../../components/parts/IconButton'
import ApprovalModal from '../../../components/modal/ApprovalModal'
import PopupMessage from '../../../components/parts/PopupMessage'
import { pendingColumns } from '../../../constants/file'

import { ClipboardCheck } from 'lucide-react'

function FileRequests() {
	const [requests, setRequests] = useState([])
	const [selected, setSelected] = useState(null)
	const [popup, setPopup] = useState(null)

	const loadRequests = async () => {
		try {
			const res = await fileApi.getPendingList()
			setRequests(res)
		} catch (error) {
			setPopup(error.message)
		}
	}

	useEffect(() => {
			const loadRequests = async () => {
				try {
					const res = await fileApi.getPendingList()
					setRequests(res)
				} catch (error) {
					setPopup(error.message)
				}
			}
		loadRequests()
	}, [])

	const handleApprove = async (isApproved, reason) => {
		try {
			await fileApi.approve(selected.id, { isApproved, reason })
			setPopup(
				`Đã ${isApproved ? 'duyệt' : 'từ chối'} yêu cầu thành công`
			)
			setSelected(null)
			loadRequests()
		} catch (error) {
			setPopup(error.message)
		}
	}

	return (
		<div>
			<div className='flex justify-between items-center mb-6'>
			<h2 className='text-lg font-semibold mb-4'>
				Yêu cầu duyệt tài liệu
			</h2>
			 <p className='mb-4 text-gray-600'>Tổng số yêu cầu: {requests.total}</p>				
			</div>


			<DataTable
				columns={pendingColumns}
				data={requests.items}
				actions={(row) => (
					<IconButton
						icon={ClipboardCheck}
						color='blue'
						onClick={() => setSelected(row)}
					/>
				)}
			/>

			{selected && (
				<ApprovalModal
					title='Kiểm duyệt tài liệu'
					details={[
						{ label: 'Loại yêu cầu', value: selected.type },
						{ label: 'Tiêu đề', value: selected.title },
						{ label: 'Người gửi', value: selected.requesterName },
						{ label: 'Tên file mới', value: selected.newFileName }
					]}
					onConfirm={handleApprove}
					onClose={() => setSelected(null)}
				/>
			)}

			{popup && (
				<PopupMessage message={popup} onClose={() => setPopup(null)} />
			)}
		</div>
	)
}

export default FileRequests
