import React, { useRef, useState } from 'react';
import Button from '../Button';
import { MAX_HOUR, MIN_HOUR, SERVER, notifyError, notifySuccess, semestersOptions } from '../../AuthContextProvider';
import { useNavigate, useParams } from 'react-router-dom';
import { daysOfWeekOptions } from '../../AuthContextProvider';

function StudentForm( {courseIds} ) {
  const idRef = useRef();
  const { formId: urlFormId } = useParams();
  const navigate = useNavigate();
  const [unavailableTimes, setUnavailableTimes] = useState([]);
  const [courseIdsList, setCourseIdsList] = useState([]);

  const addTime = () => {
    setUnavailableTimes([...unavailableTimes, { day: 1, semester: semestersOptions[0], startTime: MIN_HOUR, endTime: MIN_HOUR + 1 }]);
  };

  const addCourseId = () => {
    setCourseIdsList((prevList) => [...prevList, '']);
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
    const studentId = idRef.current.value;
    if (!studentId) {
      notifyError('Enter ID and try again');
      return;
    }
    const courseIdsInput = courseIdsList.filter(courseId => courseId.trim() !== '');

    const invalidCourseIds = courseIdsInput.filter(courseId => !courseIds.includes(courseId));
    if (invalidCourseIds.length > 0) {
      notifyError(`Courses ${invalidCourseIds.join(', ')} do not exist!`);
      return;
    }

    fetch(`${SERVER}/submitStudentForm/${urlFormId}/${studentId}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ unavailableTimes: unavailableTimes, courseIds: courseIdsInput })
    }).then(response => {
      if (response.ok) {
        notifySuccess('Form submitted successfully');
        navigate('/');
      } else {
        throw new Error('An error occurred, please try again later');
      }
    }).catch(error => notifyError(error.message));
  }

  return (
    <>
      Welcome!
      This form was generated for the institution you attend to<br />
      The perpous of this form is to schedual the courses based on your abilities
      <div className='setup-input-wrapper'>
        Enter your Id:<br />
        <div className='setup-menu-input'>
          <i className='fa-solid fa-arrow-right-long' />
          <input type='text' ref={idRef} autoFocus />
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
                newStartHour = isNaN(newStartHour) ? MIN_HOUR : Math.min(Math.max(newStartHour, MIN_HOUR), MAX_HOUR); // Ensure the value is within the range of 0 to 24
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
                newEndHour = isNaN(newEndHour) ? MIN_HOUR : Math.min(Math.max(newEndHour, MIN_HOUR), MAX_HOUR); // Ensure the value is within the range of 0 to 24
                const currentStartHour = updatedTimes[index].startTime;
                updatedTimes[index].endTime = Math.max(newEndHour, currentStartHour + 1); // Limit endTime to not go below startTime
                setUnavailableTimes(updatedTimes);
              }}
              placeholder='End Hour'
            />
          </div>
        ))}
        <br />
        <Button onClick={addTime}>Add Time</Button>
        <div className='setup-input-wrapper'>
          Enter your courses list for the next year
          {courseIdsList.map((courseId, index) => (
            <div key={index}>
              <input
                type='text'
                value={courseId}
                onChange={(e) => {
                  const updatedList = [...courseIdsList]; // Create a copy of the original list
                  updatedList[index] = e.target.value.trim(); // Update the value
                  setCourseIdsList(updatedList); // Update the state
                }}
                placeholder='Course ID'
              />
            </div>
          ))}
        </div>
      </div>
      <br />
      <Button onClick={addCourseId}>Add Course ID</Button>
      <Button onClick={sendForm}>Submit</Button>
    </>
  )
}

export default StudentForm;