import {
	PieChart,
	Pie,
	Cell,
	Tooltip,
	Legend,
	ResponsiveContainer
} from 'recharts'

import studentApi from '../../api/studentApi'
import { useEffect, useState } from 'react'

const COLORS = [
	'#3b82f6',
	'#10b981',
	'#f59e0b',
	'#ef4444',
	'#8b5cf6',
	'#06b6d4',
	'#ec4899',
	'#84cc16',
	'#f97316',
	'#6366f1'
]

function getTopSubjects(data, limit = 10) {
	const entries = Object.entries(data).sort((a, b) => b[1] - a[1])

	const top = entries.slice(0, limit)
	const rest = entries.slice(limit)

	const otherTotal = rest.reduce((s, [, v]) => s + v, 0)

	if (otherTotal > 0) top.push(['Other', otherTotal])

	return top
}

const CustomTooltip = ({ active, payload }) => {
	if (!active || !payload || !payload.length) return null

	const d = payload[0].payload

	return (
		<div className='bg-white border p-2 shadow text-sm'>
			<div className='font-semibold'>
				{d.name} ({d.code})
			</div>
			<div>
				{d.value} files • {d.percent}%
			</div>
		</div>
	)
}

const CustomLegend = ({ payload }) => {
	if (!payload) return null

	return (
		<div className='space-y-1 text-sm'>
			{payload.map((entry, i) => {
				const d = entry.payload

				return (
					<div key={i} className='flex items-center gap-2'>
						<div
							className='w-3 h-3 rounded'
							style={{ background: entry.color }}
						/>

						<span className='font-medium'>{d.name}</span>

						<span className='text-gray-500'>({d.code})</span>

						<span className='text-gray-500'>{d.percent}%</span>
					</div>
				)
			})}
		</div>
	)
}

function DonutChart({ data }) {
	const [subjectMap, setSubjectMap] = useState({})

	useEffect(() => {
		const loadSubjects = async () => {
			const res = await studentApi.getSubjects()

			const map = {}

			res.forEach((s) => {
				map[s.subjectCode] = s.subjectName
			})

			setSubjectMap(map)
		}

		loadSubjects()
	}, [])

	if (!Object.keys(subjectMap).length) {
		return <div>Loading subjects...</div>
	}

	const topSubjects = getTopSubjects(data)

	const total = topSubjects.reduce((s, [, v]) => s + v, 0)

	const chartData = topSubjects.map(([code, value]) => ({
		code: code,
		name: subjectMap[code] || code,
		value: value,
		percent: ((value / total) * 100).toFixed(1)
	}))

	return (
		<div className='w-full h-72'>
			<ResponsiveContainer>
				<PieChart>
					<Pie
						data={chartData}
						dataKey='value'
						nameKey='name'
						cx='50%'
						cy='50%'
						innerRadius={60}
						outerRadius={90}
						paddingAngle={3}>
						{chartData.map((entry, i) => (
							<Cell key={i} fill={COLORS[i % COLORS.length]} />
						))}
					</Pie>

					<Tooltip content={<CustomTooltip />} />

					<Legend
						layout='vertical'
						align='right'
						verticalAlign='middle'
						content={<CustomLegend />}
					/>
				</PieChart>
			</ResponsiveContainer>
		</div>
	)
}

export default DonutChart
