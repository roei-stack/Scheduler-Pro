import React, { useState } from 'react';
import './ScheduleTable.css'
import { MAX_HOUR, MIN_HOUR } from '../../AuthContextProvider';
import Button from '../Button';

const DaysPerWeek = 5;
const HoursPerWeek = DaysPerWeek * (MAX_HOUR - MIN_HOUR);

const ScheduleTable = ({ schedule }) => {
  const daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday'];
  const startHour = MIN_HOUR;
  const endHour = MAX_HOUR;
  const timeSlots = [];
  const [selectedSemester, setSelectedSemester] = useState('a');

  // initialize table rows
  for (let hour = startHour; hour < endHour; hour++) {
    const timeSlot = `${hour}-${hour + 1}`;
    timeSlots.push(timeSlot);
  }

  const indexForWeeklyHour = (day, hour, semester) => {
    let index = (day - 1) * (MAX_HOUR - MIN_HOUR) + (hour - MIN_HOUR);
    if (semester == 'b') {
      index += HoursPerWeek;
    }
    if (semester == 'summer') {
      index += 2 * HoursPerWeek;
    }
    return index;
  }

  const getDescription = (day, hour, semester) => {
    const index = indexForWeeklyHour(day, hour, semester);
    const item = schedule[index];
    if (item.groupData.isEmpty) {
      return (<></>);
    }
    return (
      <>
        {item.groupData.courseName}
        <br />
        {item.groupData.lessonType}
      </>
    );
  }

  return (
    <>
      <table className='table'>
        <thead>
          <tr>
            <th>Time</th>
            {daysOfWeek.map((day) => (
              <th key={day}>{day}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {timeSlots.map((timeSlot, timeIndex) => (
            <tr key={timeSlot}>
              <td>{timeSlot}</td>
              {daysOfWeek.map((day, dayIndex) => (
                <td key={day + timeSlot}>
                  {getDescription(dayIndex + 1, timeIndex + MIN_HOUR, selectedSemester)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
      <div>
        <Button onClick={() => setSelectedSemester('a')}>Semester A</Button><br /><br />
        <Button onClick={() => setSelectedSemester('b')}>Semester B</Button><br /><br />
        <Button onClick={() => setSelectedSemester('summer')}>Summer Semester</Button>
      </div>
    </>
  );
};

export default ScheduleTable;
