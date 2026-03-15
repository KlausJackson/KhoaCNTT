import { useState } from 'react'
import FileList from './FileList'
import FileRequests from './FileRequests'

function ManageFiles() {
	const [tab, setTab] = useState('files')

	return (
		<div className='p-6'>
			<div className='flex gap-6 border-b mb-6'>
				<button
					onClick={() => setTab('files')}
					className={`pb-2 ${tab === 'files' ? 'border-b-2 border-blue-600 font-semibold' : ''}`}>
					Tài liệu
				</button>

				<button
					onClick={() => setTab('requests')}
					className={`pb-2 ${tab === 'requests' ? 'border-b-2 border-blue-600 font-semibold' : ''}`}>
					Yêu cầu duyệt
				</button>
			</div>

			{tab === 'files' && <FileList />}

			{tab === 'requests' && <FileRequests />}
		</div>
	)
}

export default ManageFiles
