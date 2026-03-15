import React, { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import studentApi from '../../api/studentApi'
import IconButton from '../../components/parts/IconButton'
import { Icon } from '@iconify/react'
import {
	calculateAvailableDates,
	eventsForSelectedDate,
	exportGradesToPdf,
	calculateGPA
} from '../../helpers/studentHelpers'

const StudentPortal = () => {
	const username = localStorage.getItem('username')
	const role = localStorage.getItem('role')

	const navigate = useNavigate()
	const [isAuthenticated, setIsAuthenticated] = useState(!!username)
	const [activeTab, setActiveTab] = useState('grades')

	const [semester, setSemester] = useState('14')
	const semestersList = studentApi.getSemesters()
	const [grades, setGrades] = useState([])
	const [schedule, setSchedule] = useState([])
	const [selectedDate, setSelectedDate] = useState(new Date())

	const [isLoadingGrades, setIsLoadingGrades] = useState(false)
	const [isLoadingSchedule, setIsLoadingSchedule] = useState(false)

	const availableDates = calculateAvailableDates(schedule)
	const events = eventsForSelectedDate(schedule, selectedDate)
	const gpa = calculateGPA(grades)

	const handleLoginRedirect = () => {
		navigate('/login')
	}

	// FETCH ĐIỂM SỐ
	useEffect(() => {
		if (isAuthenticated) {
			const fetchGrades = async () => {
				setIsLoadingGrades(true)
				try {
					const resGrades = await studentApi.getGrades()
					setGrades(resGrades || [])
				} catch (error) {
					console.error('Lỗi tải điểm:', error)
				} finally {
					setIsLoadingGrades(false)
				}
			}
			fetchGrades()
		}
	}, [isAuthenticated])

	// FETCH THỜI KHÓA BIỂU (Mỗi khi đổi học kỳ)
	useEffect(() => {
		if (isAuthenticated) {
			const fetchSchedule = async () => {
				setIsLoadingSchedule(true)
				try {
					// Truyền ID học kỳ (1-14) vào API
					const resSchedule = await studentApi.getSchedule(semester)
					setSchedule(resSchedule || [])
				} catch (error) {
					console.error('Lỗi tải thời khóa biểu:', error)
					setSchedule([])
				} finally {
					setIsLoadingSchedule(false)
				}
			}
			fetchSchedule()
		}
	}, [isAuthenticated, semester])

	// 1. MÀN HÌNH YÊU CẦU ĐĂNG NHẬP (Chưa xác thực)
	if (!isAuthenticated || role !== 'student') {
		return (
			<div className='max-w-4xl mx-auto px-4 py-16 flex justify-center'>
				<div className='bg-white rounded-2xl shadow-sm border border-gray-100 p-12 text-center max-w-2xl w-full'>
					<div className='flex justify-center mb-6 text-gray-300'>
						{/* Icon Khoá */}
						<Icon icon='mdi:lock-outline' width={96} />
					</div>
					<h2 className='text-2xl font-bold text-gray-800 mb-3'>
						Yêu cầu xác thực Cổng Sinh Viên
					</h2>
					<p className='text-gray-500 mb-8'>
						Vui lòng đăng nhập bằng tài khoản Sinh viên
						(@tlu.edu.vn) để xem điểm và thời khóa biểu.
					</p>
					<button
						onClick={handleLoginRedirect}
						className='bg-[#1f4c7a] text-white px-8 py-3 rounded-md font-medium hover:bg-[#163a5d] transition flex items-center gap-2 mx-auto'>
						Đăng nhập Cổng SV
					</button>
				</div>
			</div>
		)
	}

	// 2. MÀN HÌNH ĐÃ ĐĂNG NHẬP
	return (
		<div className='max-w-7xl mx-auto px-4 py-12 items-start'>
			<div className='flex justify-between items-start mb-6'>
				<div>
					<h2 className='text-3xl font-bold text-[#1f4c7a] mb-2 border-l-4 border-red-500 pl-4'>
						Tra cứu học tập
					</h2>
					<p className='text-gray-600 pl-5'>
						Sinh viên: <strong>{username}</strong>
					</p>
				</div>

				<button
					className={`border border-[#1f4c7a] text-[#1f4c7a] px-4 py-2 rounded flex items-center gap-2 hover:bg-blue-50 transition text-sm font-medium ${activeTab !== 'grades' && 'hidden'}`}
					onClick={async () => {
						await exportGradesToPdf(username, grades, gpa)
					}}>
					Xuất File (PDF)
				</button>
			</div>

			{/* Tabs */}
			<div className='flex gap-2 mb-6 bg-slate-100 p-1.5 rounded-lg inline-flex'>
				<button
					onClick={() => setActiveTab('grades')}
					className={`px-6 py-2 rounded-md font-medium flex items-center gap-2 transition ${activeTab === 'grades' ? 'bg-white shadow-sm text-[#1f4c7a]' : 'text-gray-500 hover:text-gray-700'}`}>
					Xem điểm
				</button>
				<button
					onClick={() => setActiveTab('schedule')}
					className={`px-6 py-2 rounded-md font-medium flex items-center gap-2 transition ${activeTab === 'schedule' ? 'bg-white shadow-sm text-[#1f4c7a]' : 'text-gray-500 hover:text-gray-700'}`}>
					Thời khóa biểu
				</button>
			</div>

			{/* Content Area */}
			<div className='bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden'>
				{/* Xem Điểm Tab */}
				{activeTab === 'grades' && (
					<div className='overflow-x-auto'>
						{isLoadingGrades ? (
							<div className='p-12 text-center text-gray-500'>
								Đang tải điểm số...
							</div>
						) : grades.length === 0 ? (
							<div className='p-12 text-center text-gray-500'>
								Chưa có dữ liệu điểm.
							</div>
						) : (
							<>
								<div className='px-6 py-4 text-gray-700 font-medium'>
									GPA toàn khóa:{' '}
									<span className='text-[#1f4c7a]'>
										{gpa}
									</span>
								</div>
								<table className='w-full text-left border-collapse'>
									<thead>
										<tr className='bg-[#1f4c7a] text-white text-xs uppercase tracking-wider'>
											<th className='px-6 py-4 font-medium'>
												Tên học phần
											</th>
											<th className='px-6 py-4 font-medium text-center'>
												Số TC
											</th>
											<th className='px-6 py-4 font-medium text-center'>
												QT
											</th>
											<th className='px-6 py-4 font-medium text-center'>
												Thi
											</th>
											<th className='px-6 py-4 font-medium text-center'>
												Tổng kết
											</th>
											<th className='px-6 py-4 font-medium text-center'>
												Điểm chữ
											</th>
										</tr>
									</thead>
									<tbody className='divide-y divide-gray-100'>
										{grades.map((grade, index) => (
											<tr
												key={index}
												className='hover:bg-slate-50 transition'>
												<td className='px-6 py-4 text-gray-800 font-medium'>
													{grade.subjectName ||
														grade.name}
												</td>
												<td className='px-6 py-4 text-center text-gray-600'>
													{grade.credits}
												</td>
												<td className='px-6 py-4 text-center text-gray-600'>
													{grade.processMark?.toFixed(
														1
													) || '-'}
												</td>
												<td className='px-6 py-4 text-center text-gray-600'>
													{grade.examMark?.toFixed(
														1
													) || '-'}
												</td>
												<td className='px-6 py-4 text-center font-bold text-gray-800'>
													{grade.finalMark?.toFixed(
														1
													) || '-'}
												</td>
												<td className='px-6 py-4 text-center font-bold text-[#1f4c7a]'>
													{grade.charMark || '-'}
												</td>
											</tr>
										))}
									</tbody>
								</table>
							</>
						)}
					</div>
				)}

				{/* Thời khoá biểu Tab */}
				{activeTab === 'schedule' && (
					<div className='p-6'>
						<div className='flex items-center gap-4 mb-6'>
							<label className='text-gray-700 font-medium'>
								Chọn học kỳ:
							</label>
							<select
								className='border border-gray-300 rounded-md px-4 py-2 text-gray-700 focus:outline-none focus:ring-2 focus:ring-[#1f4c7a]'
								value={semester}
								onChange={(e) => setSemester(e.target.value)}>
								{/* Lặp qua object semestersList */}
								{Object.entries(semestersList).map(
									([id, name]) => (
										<option key={id} value={id}>
											{name}
										</option>
									)
								)}
							</select>
						</div>

						{isLoadingSchedule ? (
							<div className='p-12 text-center text-gray-500'>
								Đang tải thời khóa biểu...
							</div>
						) : schedule.length === 0 ? (
							<div className='p-12 text-center text-gray-500'>
								Không có lịch học trong học kỳ này.
							</div>
						) : (
							<>
								<div className='flex overflow-x-auto gap-2 py-3 px-2'>
									{availableDates.map((dateStr) => {
										const date = new Date(dateStr)

										const isSelected =
											dateStr ===
											selectedDate
												.toISOString()
												.split('T')[0]

										return (
											<div
												key={dateStr}
												onClick={() =>
													setSelectedDate(date)
												}
												className={`cursor-pointer min-w-[60px] rounded-lg text-center py-2
	${isSelected ? 'bg-[#1f4c7a] text-white' : 'bg-gray-200 text-gray-700'}
	`}>
												<div className='text-xs'>
													{date.toLocaleDateString(
														'vi-VN',
														{ weekday: 'short' }
													)}
												</div>

												<div className='font-bold'>
													{date.getDate()}/
													{date.getMonth() + 1}
												</div>
											</div>
										)
									})}
								</div>
								<div className='space-y-3'>
									{events.map((event, index) => (
										<div
											key={index}
											className='flex rounded-lg overflow-hidden shadow'>
											<div className='w-1' />

											<div className='p-4 flex-1'>
												<div className='font-bold'>
													{event.subjectCode}
												</div>

												<div className='text-sm text-gray-400'>
													{event.subjectName}
												</div>

												<div className='flex gap-6 mt-2 text-sm text-gray-900'>
													<span>
														🕒 {event.startHour} -{' '}
														{event.endHour}
													</span>

													<span>📍 {event.room}</span>

													<span>
														{event.teacherName}
													</span>
												</div>
											</div>
										</div>
									))}
								</div>
							</>
						)}
					</div>
				)}
			</div>
		</div>
	)
}

export default StudentPortal
