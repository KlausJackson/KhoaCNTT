import { useEffect, useState } from 'react'
import fileApi from '../../../api/fileApi'
import DataTable from '../../../components/table/DataTable'
import IconButton from '../../../components/parts/IconButton'
import ApprovalModal from '../../../components/modal/ApprovalModal'
import PopupMessage from '../../../components/parts/PopupMessage'
import { pendingColumns } from '../../../constants/file'

import { ClipboardCheck } from 'lucide-react'
import { handleError } from '../../../helpers/commonHelpers'
import { handleApprove } from '../../../helpers/fileHelpers'

function FileRequests() {
	const [requests, setRequests] = useState([])
	const [selected, setSelected] = useState(null)
	const [popup, setPopup] = useState(null)

	const loadRequests = async () => {
		try {
			const res = await fileApi.getPendingList()
			setRequests(res)
		} catch (err) {
			handleError(err, setPopup)
		}
	}

	useEffect(() => {
		(async () => {
			await loadRequests()
		})()
	}, [])

	return (
		<div>
			<div className='flex justify-between items-center mb-6'>
				<h2 className='text-lg font-semibold mb-4'>
					Yêu cầu duyệt tài liệu
				</h2>
				<p className='mb-4 text-gray-600'>
					Tổng số yêu cầu: {requests.total}
				</p>
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
					title='Duyệt tài liệu'
					details={[
						{ label: 'Loại yêu cầu', value: selected.type },
						{ label: 'Tiêu đề', value: selected.title },
						{ label: 'Người gửi', value: selected.requesterName },
						{ label: 'Tên file mới', value: selected.newFileName }
					]}
					onConfirm={(isApproved, reason) => handleApprove(isApproved, reason, selected.id, setPopup, setSelected, loadRequests)}
					onClose={() => setSelected(null)}
				/>
			)}

			{popup && (
				<PopupMessage message={popup.message} type={popup.type} onClose={() => setPopup(null)} />
			)}
		</div>
	)
}

export default FileRequests
