export const columns = [
	{
		key: 'fullName',
		label: 'Tên giảng viên'
	},

	{
		key: 'degree',
		label: 'Học vị'
	},

	{
		key: 'position',
		label: 'Chức vụ'
	},

	{
		key: 'email',
		label: 'Email'
	},

	{
		key: 'phoneNumber',
		label: 'Điện thoại'
	}
]

export const fields = [
	{
		name: 'fullName',
		label: 'Họ tên',
		required: true
	},

	{
		name: 'imageUrl',
		label: 'Ảnh'
	},

	{
		name: 'degree',
		label: 'Học vị',
		type: 'select',
		required: true,
		options: [
			{ label: 'Bachelor', value: 'Bachelor' },
			{ label: 'Master', value: 'Master' },
			{ label: 'Doctor', value: 'Doctor' },
			{ label: 'Professor', value: 'Professor' },
            { label: 'Associate Professor', value: 'Associate Professor' }
		]
	},

	{
		name: 'position',
		label: 'Chức vụ',
        required: true,
        type: 'select',
        options: [
            { label: 'Giảng viên', value: 'Giảng viên' },
            { label: 'Trưởng bộ môn', value: 'Trưởng bộ môn' },
            { label: 'Phó trưởng bộ môn', value: 'Phó trưởng bộ môn' },
            { label: 'Trưởng khoa', value: 'Trưởng khoa' },
            { label: 'Phó trưởng khoa', value: 'Phó trưởng khoa' }
        ]
	},

	{
		name: 'birthdate',
		label: 'Ngày sinh',
		type: 'date',
        required: true
	},

	{
		name: 'email',
		label: 'Email',
		type: 'email'
	},

	{
		name: 'phoneNumber',
		label: 'Số điện thoại'
	},

	{
		name: 'subjectCodes',
		label: 'Môn giảng dạy',
		fullWidth: true,
	}
]
