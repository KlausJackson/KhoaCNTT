import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'

export function calculateGPA(grades) {
    
	let totalCredits = 0
	let totalPoints = 0

	const gradePoints = {
		A: 4.0,
		B: 3.0,
		C: 2.0,
		D: 1.0,
		F: 0.0
	}
	grades.forEach((g) => {
		if (g.isCalculated && g.charMark && g.charMark != 'F') {
			const points = gradePoints[g.charMark] ?? 0
			totalPoints += points * g.credits
			totalCredits += g.credits
		}
	})

	const gpa =
		totalCredits === 0 ? '0.00' : (totalPoints / totalCredits).toFixed(2)
    return gpa
}

export const exportGradesToPdf = async (username, grades, gpa) => {
	const doc = new jsPDF();

    const fontUrl = "https://cdn.jsdelivr.net/gh/googlefonts/noto-fonts/hinted/ttf/NotoSans/NotoSans-Regular.ttf"

	const res = await fetch(fontUrl)
	const fontBuffer = await res.arrayBuffer()

    function arrayBufferToBase64(buffer) {
        let binary = ''
        const bytes = new Uint8Array(buffer)
        const len = bytes.byteLength

        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i])
        }

        return window.btoa(binary)
    }

	const fontBase64 = arrayBufferToBase64(fontBuffer)

	doc.addFileToVFS("NotoSans.ttf", fontBase64)
	doc.addFont("NotoSans.ttf", "NotoSans", "normal")
	doc.setFont("NotoSans")
    doc.setFontSize(12)

	const tableData = grades.map((g) => [
		g.subjectName || g.name,
		g.credits,
		g.processMark?.toFixed(1) || '-',
		g.examMark?.toFixed(1) || '-',
		g.finalMark?.toFixed(1) || '-',
		g.charMark || '-'
	])

	doc.text(`Sinh viên: ${username}`, 14, 15)
	doc.text(`GPA toàn khóa: ${gpa}`, 14, 22)

	autoTable(doc, {
		startY: 30,
		head: [['Subject Name', 'Credits', 'QT', 'Thi', 'Final Score', 'Char Mark']],
		body: tableData,
		styles: {
			font: 'NotoSans',
			fontSize: 10
		},
		headStyles: {
			font: 'NotoSans',
            fontStyle: 'normal',
			fillColor: [31,76,122],
			textColor: 'white'
		},
		bodyStyles: { font: 'NotoSans', fontSize: 10 },
		didParseCell: function (data) {
			data.cell.styles.font = 'NotoSans'
		}
	})

	doc.save(`bang-diem-${username}.pdf`)
}

export const eventsForSelectedDate = (schedule, selected) => {
	return Array.isArray(schedule)
		? schedule.filter((event) => {
				const targetWeekday = convertWeekIndex(event.weekIndex)

			if (targetWeekday === undefined) return false

			let start = new Date(event.startDateTimestamp)
			let end = new Date(event.endDateTimestamp)

			let current = new Date(selected)

			return (
				current >= start &&
				current <= end &&
				current.getDay() === targetWeekday
			)
		})
	: []
}

export const calculateAvailableDates = (schedule) => {
	const uniqueDates = new Set()

	schedule.forEach((classItem) => {
		if (!classItem.startDateTimestamp || !classItem.endDateTimestamp) return

		const targetWeekday = convertWeekIndex(classItem.weekIndex)

		if (targetWeekday === undefined) return

		let start = new Date(classItem.startDateTimestamp)
		let end = new Date(classItem.endDateTimestamp)

		start.setHours(0, 0, 0, 0)
		end.setHours(0, 0, 0, 0)

		for (
			let day = new Date(start);
			day <= end;
			day.setDate(day.getDate() + 1)
		) {
			if (day.getDay() === targetWeekday) {
				uniqueDates.add(day.toLocaleDateString('sv-SE'))
			}
		}
	})

	return [...uniqueDates].sort()
}

const convertWeekIndex = (weekIndex) => {
	const map = {
		2: 1,
		3: 2,
		4: 3,
		5: 4,
		6: 5,
		7: 6,
		8: 0
	}

	return map[weekIndex]
}
	// const mockGrades = [
	// 	{
	// 		id: 1,
	// 		code: 'CSE481',
	// 		name: 'Phát triển dự án phần mềm',
	// 		credits: 3,
	// 		process: 9.0,
	// 		exam: 8.5,
	// 		final: 8.7,
	// 		letter: 'A',
	// 		letterColor: 'text-green-600'
	// 	},
	// 	{
	// 		id: 2,
	// 		code: 'CSE305',
	// 		name: 'Lập trình Web',
	// 		credits: 3,
	// 		process: 8.0,
	// 		exam: 7.0,
	// 		final: 7.4,
	// 		letter: 'B',
	// 		letterColor: 'text-blue-600'
	// 	},
	// 	{
	// 		id: 3,
	// 		code: 'MATH101',
	// 		name: 'Đại số tuyến tính',
	// 		credits: 3,
	// 		process: 6.5,
	// 		exam: 5.0,
	// 		final: 5.6,
	// 		letter: 'C',
	// 		letterColor: 'text-yellow-600'
	// 	}
	// ]

	// const mockSchedule = [
	// 	{
	// 		id: 1,
	// 		day: 'Thứ 2',
	// 		time: '1 - 3 (07:00 - 09:25)',
	// 		subject: 'Lập trình Web (Thực hành)',
	// 		room: 'Phòng Máy 3 - Tòa C',
	// 		teacher: 'ThS. Nguyễn Văn A'
	// 	},
	// 	{
	// 		id: 2,
	// 		day: 'Thứ 4',
	// 		time: '4 - 6 (09:35 - 12:00)',
	// 		subject: 'Phát triển dự án phần mềm',
	// 		room: 'Phòng 205 - Tòa B1',
	// 		teacher: 'TS. Cù Việt Dũng'
	// 	},
	// 	{
	// 		id: 3,
	// 		day: 'Thứ 6',
	// 		time: '7 - 9 (13:00 - 15:25)',
	// 		subject: 'An toàn thông tin',
	// 		room: 'Phòng 301 - Tòa C',
	// 		teacher: 'TS. Phạm Văn B'
	// 	}
	// ]