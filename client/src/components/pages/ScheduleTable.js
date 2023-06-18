import React from 'react';
import './ScheduleTable.css'
import { MAX_HOUR, MIN_HOUR } from '../../AuthContextProvider';

const ScheduleTable = () => {
  const daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday'];
  const startHour = MIN_HOUR;
  const endHour = MAX_HOUR;
  const timeSlots = [];

  // initialize table rows
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
