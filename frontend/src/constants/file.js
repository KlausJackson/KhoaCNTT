export const columns = [
	{ key: 'title', label: 'Tiêu đề', width: '22%' },
	{ key: 'subjectCode', label: 'Môn học', width: '12%' },
	{
		key: 'fileType',
		label: 'Loại tài liệu',
		width: '14%',
		render: (value) => fileTypeMap[value] || value
	},
	{
		key: 'permission',
		label: 'Quyền',
		width: '14%',
		render: (value) => permissionMap[value] || value
	},
	{ key: 'viewCount', label: 'Views', width: '10%' },
	{ key: 'downloadCount', label: 'Downloads', width: '10%' }
]

export const editMetadataFields = [
	{
		name: 'title',
		label: 'Tiêu đề tài liệu',
		required: true
	},
	{
		name: 'permission',
		label: 'Quyền truy cập',
		type: 'select',
		options: [
			{
				label: 'Được tải',
				value: 'PublicDownload'
			},
			{ label: 'Chỉ xem', value: 'PublicRead' },
			{ label: 'SV chỉ xem', value: 'StudentRead' },
			{
				label: 'SV được tải',
				value: 'StudentDownload'
			},
			{ label: 'Ẩn', value: 'Hidden' }
		]
	},
	{
		name: 'subjectCode',
		label: 'Mã môn học',
		placeholder: 'Ví dụ: CSE111',
		required: false,
		fullWidth: true
	}
]

export const pendingColumns = [
	{
		key: 'type',
		label: '',
		type: 'requestType',
		width: '35px'
	},
	{
		key: 'title',
		label: 'Tiêu đề',
		width: '25%'
	},
	{
		key: 'createdAt',
		label: 'Ngày tạo',
		type: 'date',
		width: '20%'
	},
	{
		key: 'requesterName',
		label: 'Người yêu cầu',
		width: '17%'
	},
	{
		key: 'newFileName',
		label: 'Tên file mới',
		width: '20%'
	},
	{
		key: 'newFileSize',
		label: 'Kích thước mới',
		width: '17%'
	},
	{
		key: 'oldFileName',
		label: 'Tên file cũ',
		width: '20%'
	},
	{
		key: 'oldFileSize',
		label: 'Kích thước cũ',
		width: '15%'
	}
]

export const uploadFields = [
	{
		name: 'title',
		label: 'Tiêu đề',
		required: true
	},
	{
		name: 'fileType',
		label: 'Loại tài liệu',
		type: 'select',
		options: [
			{ label: 'Bài giảng', value: 'LectureNotes' },
			{ label: 'Đề thi', value: 'Test' },
			{ label: 'Biểu mẫu', value: 'Form' },
			{ label: 'Khác', value: 'Other' }
		]
	},
	{
		name: 'permission',
		label: 'Quyền truy cập',
		type: 'select',
		options: [
			{
				label: 'Được tải',
				value: 'PublicDownload'
			},
			{ label: 'Chỉ xem', value: 'PublicRead' },
			{ label: 'SV chỉ xem', value: 'StudentRead' },
			{
				label: 'SV được tải',
				value: 'StudentDownload'
			},
			{ label: 'Ẩn', value: 'Hidden' }
		]
	},
	{
		name: 'file',
		label: 'Chọn file',
		type: 'file',
		required: true,
		fullWidth: true
	},
	{
		name: 'subjectCode',
		label: 'Mã môn học',
		required: false,
		fullWidth: true
	}
]

export const replaceFields = [
	{
		name: 'oldTitle',
		label: 'Đang thay thế file',
		readOnly: true,
		fullWidth: true
	},
	{
		name: 'file',
		label: 'Chọn file thay thế mới',
		type: 'file',
		required: true,
		fullWidth: true
	}
]

export const getSearchConfig = (subjects) => [
	{
		name: 'keyword',
		label: 'Từ khóa',
		type: 'text',
		placeholder: 'Tìm kiếm...',
		colSpan: 2,
		width: 'w-[200px]'
	},
	{
		name: 'subjectCodes',
		label: 'Môn học',
		type: 'checkbox-group',
		colSpan: 3,
		options: subjects.map((s) => ({
			label: `${s.subjectName} (${s.subjectCode})`,
			value: s.subjectCode
		}))
	},
	{
		name: 'fileType',
		label: 'Loại tài liệu',
		type: 'select',
		colSpan: 2,
		options: [
			{ label: 'Tất cả', value: '' },
			{ label: 'Bài giảng', value: 'LectureNotes' },
			{ label: 'Đề thi', value: 'Test' },
			{ label: 'Biểu mẫu', value: 'Form' },
			{ label: 'Khác', value: 'Other' }
		],
		width: 'w-[140px]'
	},
	{
		name: 'pageSize',
		label: 'Số kết quả',
		type: 'number',
		defaultValue: 10,
		colSpan: 1,
		width: 'w-[90px]'
	}
]

export const fileTypeMap = {
	LectureNotes: 'Bài giảng',
	Test: 'Đề thi',
	Form: 'Biểu mẫu',
	Other: 'Khác'
}

export const permissionMap = {
	PublicDownload: 'Được tải',
	PublicRead: 'Chỉ xem',
	StudentRead: 'SV chỉ xem',
	StudentDownload: 'SV được tải',
	Hidden: 'Ẩn'
}
