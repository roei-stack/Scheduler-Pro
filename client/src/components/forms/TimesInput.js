import React, { useState } from 'react'
import { MAX_HOUR, MIN_HOUR, daysOfWeekOptions, semestersOptions } from '../../AuthContextProvider';
import Button from '../Button';

function TimesInput( {times, setTimes, index} ) {
    const handleDayOfWeekChange = (index, value) => {
        const day = parseInt(value);
        const updatedTimes = [...times];
        updatedTimes[index].day = day;
        setTimes(updatedTimes);
    };

    const handleSemesterChange = (index, value) => {
        const updatedTimes = [...times];
        updatedTimes[index].semester = value;
        setTimes(updatedTimes);
    };

    const addTime = () => {
        const newValue = [...times, { day: 1, semester: semestersOptions[0], startTime: MIN_HOUR, endTime: MIN_HOUR + 1, index: index }]
        setTimes(newValue);
    };

    return (
        <>{times.map((time, index) => (
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
                        const updatedTimes = [...times];
                        let newStartHour = parseInt(e.target.value);
                        newStartHour = isNaN(newStartHour) ? MIN_HOUR : Math.min(Math.max(newStartHour, MIN_HOUR), MAX_HOUR); // Ensure the value is within the range of 0 to 24
                        const currentEndHour = updatedTimes[index].endTime;
                        updatedTimes[index].startTime = Math.min(newStartHour, currentEndHour - 1); // Limit startTime to not exceed endTime
                        setTimes(updatedTimes);
                    }}
                    placeholder='Start Hour'
                />
                <input
                    type='number'
                    value={time.endTime}
                    onChange={(e) => {
                        const updatedTimes = [...times];
                        let newEndHour = parseInt(e.target.value);
                        newEndHour = isNaN(newEndHour) ? MIN_HOUR : Math.min(Math.max(newEndHour, MIN_HOUR), MAX_HOUR); // Ensure the value is within the range of 0 to 24
                        const currentStartHour = updatedTimes[index].startTime;
                        updatedTimes[index].endTime = Math.max(newEndHour, currentStartHour + 1); // Limit endTime to not go below startTime
                        setTimes(updatedTimes);
                    }}
                    placeholder='End Hour'
                />
            </div>
        ))}
        <br />
        <Button onClick={addTime}>Add Time</Button>
        <br />
        </>
    )
}

export default TimesInput;