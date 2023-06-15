import React, { useRef, useState } from 'react';
import Button from '../Button';
import { SERVER, notifyError, notifySuccess } from '../../AuthContextProvider';
import { useNavigate, useParams } from 'react-router-dom';

const daysOfWeekOptions = [
  { label: 'Sunday', value: 1 },
  { label: 'Monday', value: 2 },
  { label: 'Tuesday', value: 3 },
  { label: 'Wednesday', value: 4 },
  { label: 'Thursday', value: 5 },
  { label: 'Friday', value: 6 },
  { label: 'Saturday', value: 7 }
];

const semestersOptions = ['A', 'B', 'Summer'];

function StaffForm() {
  const nameRef = useRef();
  const { formId: urlFormId } = useParams();
  const navigate = useNavigate();
  const [unavailableTimes, setUnavailableTimes] = useState([]);

  const addTime = () => {
    setUnavailableTimes([...unavailableTimes, { day: 1, semester: 'A', startTime: 0, endTime: 1 }]);
  };

  const handleDayOfWeekChange = (index, value) => {
    const day = parseInt(value);
    const updatedTimes = [...unavailableTimes];
    updatedTimes[index].day = day;
    setUnavailableTimes(updatedTimes);
  };

  const handleSemesterChange = (index, value) => {
    const updatedTimes = [...unavailableTimes];
    updatedTimes[index].semester = value;
    setUnavailableTimes(updatedTimes);
  };

  const sendForm = () => {
    const staffId = nameRef.current.value;
    if (!staffId) {
      notifyError('Enter ID and try again');
      return;
    }

    fetch(`${SERVER}/submitStaffForm/${urlFormId}/${staffId}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(unavailableTimes)
    }).then(response => {
      if (response.ok) {
        notifySuccess('Form submitted successfully');
        navigate('/');
      } else {
        throw new Error('An error occurred, check your staff id');
      }
    }).catch(error => notifyError(error.message));
  }

  return (
    <>
      Welcome!<br />
      This form was generated for the institution you work in<br />
      The perpous of this form is to schedual the courses based on the staff' abilities
      <div className='setup-input-wrapper'>
        Enter your Id:<br />
        <div className='setup-menu-input'>
          <i className='fa-solid fa-arrow-right-long' />
          <input type='text' ref={nameRef} autoFocus />
        </div>
      </div>
      <div className='setup-input-wrapper'>
        Enter times in which you are unavilable:<br />
        {unavailableTimes.map((time, index) => (
          <div key={index}>
            <select
              value={time.day}
              onChange={(e) => handleDayOfWeekChange(index, e.target.value)}
              placeholder='Day of Week'
            >
              {daysOfWeekOptions.map((option) => (
                <option key={option.value} value={option.value}>{option.label}</option>
              ))}
            </select>
            <select
              value={time.semester}
              onChange={(e) => handleSemesterChange(index, e.target.value)}
              placeholder='Semester'
            >
              {semestersOptions.map((option) => (
                <option key={option} value={option}>{option}</option>
              ))}
            </select>
            <input
              type='number'
              value={time.startTime}
              onChange={(e) => {
                const updatedTimes = [...unavailableTimes];
                let newStartHour = parseInt(e.target.value);
                newStartHour = isNaN(newStartHour) ? 0 : Math.min(Math.max(newStartHour, 0), 24); // Ensure the value is within the range of 0 to 24
                const currentEndHour = updatedTimes[index].endTime;
                updatedTimes[index].startTime = Math.min(newStartHour, currentEndHour - 1); // Limit startTime to not exceed endTime
                setUnavailableTimes(updatedTimes);
              }}
              placeholder='Start Hour'
            />
            <input
              type='number'
              value={time.endTime}
              onChange={(e) => {
                const updatedTimes = [...unavailableTimes];
                let newEndHour = parseInt(e.target.value);
                newEndHour = isNaN(newEndHour) ? 0 : Math.min(Math.max(newEndHour, 0), 24); // Ensure the value is within the range of 0 to 24
                const currentStartHour = updatedTimes[index].startTime;
                updatedTimes[index].endTime = Math.max(newEndHour, currentStartHour + 1); // Limit endTime to not go below startTime
                setUnavailableTimes(updatedTimes);
              }}
              placeholder='End Hour'
            />
          </div>
        ))}
      </div>
      <br />
      <Button onClick={addTime}>Add Time</Button>
      <Button onClick={sendForm}>Submit</Button>
    </>
  )
}

export default StaffForm;