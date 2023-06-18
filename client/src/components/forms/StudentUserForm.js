import React, { useContext, useEffect, useRef, useState } from 'react';
import Button from '../Button';
import { AuthContext, MAX_HOUR, MIN_HOUR, SERVER, notifyError, notifySuccess, semestersOptions } from '../../AuthContextProvider';
import { useNavigate, useParams } from 'react-router-dom';
import { daysOfWeekOptions } from '../../AuthContextProvider';
import FileUpload from '../FileUpload';
import TimesInput from './TimesInput';

function StudentUserForm({ onAbort }) {
    const { token } = useContext(AuthContext);
    const [institutionNames, setInstitutionNames] = useState([]);
    const [selectedInstitution, setSelectedInstitution] = useState('');
    const [courseIdsList, setCourseIdsList] = useState([]);
    const [fullCoursesList, setFullCoursesList] = useState([]);
    const nameRef = useRef();
    const learningDaysRef = useRef();
    const hoursPerDayRef = useRef();
    const semesterAref = useRef(null);
    const semesterBref = useRef(null);
    const semesterSummerRef = useRef(null);

    const addCourseId = () => {
        setCourseIdsList((prevList) => [...prevList, '']);
    };

    const isMountedRef = useRef(false);
    useEffect(() => {
        if (!isMountedRef.current) {
            isMountedRef.current = true;
            return;
        }
        getInstitutionNames();
    }, []);

    const getInstitutionNames = () => {
        fetch(`${SERVER}/Account/finishedInstitutionNames`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        }).then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error(`An error occured (${response.status})`);
            }
        }).then(data => setInstitutionNames(data.names))
            .catch(error => {
                notifyError(error.message);
                onAbort();
            });
    }

    const [lectureTimes, setLectureTimes] = useState([]);
    const [exreciseTimes, setExreciseTimes] = useState([]);
    const [unavilableTimes, setUnavilableTimes] = useState([]);

    const addFullCourse = () => {
        setFullCoursesList((prevList) => [...prevList, {
            id: 'course id',
            name: 'course name',
            lectureDuration: 1,
            exreciseDuration: 1,
            lectureTimes: [],
            exreciseTimes: []
        }]);
    };

    const handleSubmit = () => {
        const name = nameRef.current.value;
        if (!name) {
            notifyError('provide a name');
            onAbort();
        }
        const learningDays = parseInt(learningDaysRef.current.value, 10);
        if (isNaN(learningDays)) {
            notifyError('provide learning days');
            onAbort();
        }
        const hoursPerDay = parseInt(hoursPerDayRef.current.value, 10);
        if (isNaN(hoursPerDay)) {
            notifyError('provide hours per day');
            onAbort();
        }
        const semesterA = semesterAref.current.checked;
        const semesterB = semesterBref.current.checked;
        const semesterSummer = semesterSummerRef.current.checked;
        const unavilableTimesFinal = unavilableTimes.map(({ index, ...item }) => item);

        if (selectedInstitution != '') {
            // send course id's list
            fetch(`${SERVER}/Account/studentInputWithInstitution`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    name: name,
                    institutionName: selectedInstitution,
                    learningDays: learningDays,
                    hoursPerDay: hoursPerDay,
                    semesterA: semesterA,
                    semesterB: semesterB,
                    semesterSummer: semesterSummer,
                    unavilableTimes: unavilableTimesFinal,
                    courseIds: courseIdsList
                })
            }).then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error('Something went wrong');
                }
            }).then(data => console.log(data))
                .catch(error => notifyError(error.message));
        } else {
            // send full courses
            const coursesData = fullCoursesList;
            for (var index in coursesData) {
                const filteredLectureTimes = lectureTimes.filter(item => item.index == index).map(({ index, ...item }) => item);
                const filteredExreciseTimes = exreciseTimes.filter(item => item.index == index).map(({ index, ...item }) => item);
                coursesData[index].lectureTimes = filteredLectureTimes;
                coursesData[index].exreciseTimes = filteredExreciseTimes;
            }
            fetch(`${SERVER}/Account/studentInputWithoutInstitution`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    name: name,
                    learningDays: learningDays,
                    hoursPerDay: hoursPerDay,
                    semesterA: semesterA,
                    semesterB: semesterB,
                    semesterSummer: semesterSummer,
                    unavilableTimes: unavilableTimesFinal,
                    coursesData: coursesData
                })
            }).then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error('Something went wrong');
                }
            }).then(data => console.log(data))
                .catch(error => notifyError(error.message));
        }
    }

    return (
        <div className='form'>
            <div className='blur-box'>
                <div className='box-container'>
                    Welcome! Help us help you create your schedule by this form<br />
                    <div className='setup-input-wrapper'>
                        Schedule's name:<br />
                        <div className='setup-menu-input'>
                            <i className='fa-solid fa-arrow-right-long' />
                            <input type='text' ref={nameRef} />
                        </div>
                    </div>
                    Prefered number of learning days:<br />
                    <div className='setup-menu-input'>
                        <i className='fa-solid fa-arrow-right-long' />
                        <input type='number' ref={learningDaysRef} />
                    </div>
                    Prefered number of hours per day:<br />
                    <div className='setup-menu-input'>
                        <i className='fa-solid fa-arrow-right-long' />
                        <input type='number' ref={hoursPerDayRef} />
                    </div>
                    <div>
                        Preffered semesters to learn on:<br />
                        <small><input type='checkbox' ref={semesterAref} />A</small>&nbsp;
                        <small><input type='checkbox' ref={semesterBref} />B</small>&nbsp;
                        <small><input type='checkbox' ref={semesterSummerRef} />Summer</small>&nbsp;
                    </div>
                    Pick your institution
                    <select placeholder='browse' onChange={e => setSelectedInstitution(e.target.value)}>
                        <option value={''}>None</option>
                        {institutionNames.map((name, index) => (
                            <option key={index} value={name}>{name}</option>
                        ))}
                    </select>
                    <div>
                        Enter times in which you are unavilable:<br />
                        <TimesInput times={unavilableTimes} setTimes={setUnavilableTimes} index={0} />
                        Enter your courses list for the next year
                        {selectedInstitution !== '' ?
                            <div className='setup-input-wrapper'>
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
                                <br />
                                <Button onClick={addCourseId}>Add Course id</Button>
                            </div> :
                            <div className='setup-input-wrapper'>
                                {fullCoursesList.map((course, index) => (
                                    <div key={index}>Course #{index + 1}:<div>
                                        <input type='text' placeholder='Course id' onChange={e => {
                                            const updatedCourses = [...fullCoursesList];
                                            updatedCourses[index].id = e.target.value;
                                            setFullCoursesList(updatedCourses);
                                        }}>
                                        </input>
                                        <input type='text' placeholder='Course name' onChange={e => {
                                            const updatedCourses = [...fullCoursesList];
                                            updatedCourses[index].name = e.target.value;
                                            setFullCoursesList(updatedCourses);
                                        }}>
                                        </input>
                                        <input type='number' placeholder='Lecture duration' onChange={e => {
                                            const updatedCourses = [...fullCoursesList];
                                            let newValue = parseInt(e.target.value);
                                            newValue = isNaN(newValue) ? 0 : Math.min(Math.max(newValue, 0), 5);
                                            updatedCourses[index].lectureDuration = newValue;
                                            setFullCoursesList(updatedCourses);
                                        }}>
                                        </input>
                                        <input type='number' placeholder='Exrecise duration' onChange={e => {
                                            const updatedCourses = [...fullCoursesList];
                                            let newValue = parseInt(e.target.value);
                                            newValue = isNaN(newValue) ? 0 : Math.min(Math.max(newValue, 0), 5);
                                            updatedCourses[index].exreciseDuration = newValue;
                                            setFullCoursesList(updatedCourses);
                                        }}>
                                        </input>
                                        <br />
                                        Lecture times:
                                        <TimesInput times={lectureTimes} setTimes={setLectureTimes} index={index} />
                                        <br />
                                        Exrecise Times:
                                        <TimesInput times={exreciseTimes} setTimes={setExreciseTimes} index={index} />
                                    </div></div>
                                ))}
                                <br />
                                <Button onClick={addFullCourse}>Add Course</Button>
                            </div>
                        }
                    </div>
                    <br />
                    <Button onClick={handleSubmit}>Submit</Button>
                    <Button onClick={onAbort}>Cancel</Button>
                </div>
            </div>
        </div>
    )
}

export default StudentUserForm;