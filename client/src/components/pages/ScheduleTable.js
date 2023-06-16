import React from 'react';
import './ScheduleTable.css'

const ScheduleTable = () => {
  const daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday'];
  const startHour = 9;
  const endHour = 22;
  const timeSlots = [];

  for (let hour = startHour; hour <= endHour; hour++) {
    const timeSlot = `${hour}-${hour + 1}`;
    timeSlots.push(timeSlot);
  }

  return (
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
        {timeSlots.map((timeSlot) => (
          <tr key={timeSlot}>
            <td>{timeSlot}</td>
            {daysOfWeek.map((day) => (
              <td key={day + timeSlot}>
                hi
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default ScheduleTable;
