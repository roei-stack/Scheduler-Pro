import React, { useContext, useEffect, useRef, useState } from 'react';
import { AuthContext, SERVER, daysOfWeekOptions, notifyError, notifyInfo, notifySuccess } from '../../AuthContextProvider';
import { Link } from 'react-router-dom';
import Button from '../Button';
import './Pages.css';
import './Account.css';

function InstitutionAccount() {
  const [formsIds, setFormsIds] = useState({ staffFormId: '', studentFormId: '' });
  const { username, token } = useContext(AuthContext);
  const isMountedRef = useRef(false);

  useEffect(() => {
    if (!isMountedRef.current) {
      isMountedRef.current = true;
      return;
    }
    getFormsLinks();
  }, []);

  const getFormsLinks = () => {
    fetch(`${SERVER}/Account/getFormsLinks`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      }
    }).then(response => {
      if (response.ok) {
        return response.json();
      } else {
        throw new Error(`Status code: ${response.status}`)
      }
    }).then(data => setFormsIds(data))
      .catch(error => console.log(error.message));
  }

  const handleSubmit = () => {
    fetch(`${SERVER}/Account/callInstitutionSchedulingAlgo`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
    }).then(response => {
      if (response.ok) {
        return response.json();
      } else {
        throw new Error(`status ${response.status}`);
      }
    }).then(data => {
      if (data.isSuccessful) {
        notifySuccess('Your results are ready!');
      } else {
        notifyInfo('Please try to lower your restrictions');
      }
      downloadResult(data.output);
    }).catch(error => notifyError(error.message));
  };

  const downloadResult = output => {
    // Convert dictionary object to CSV string
    let csvContent = "Course ID,Group Number,Staff IDs,Semester,Day,Start Time,End Time,Type\n";
    // Iterate over the dictionary object and add rows to the CSV
    Object.entries(output).forEach(([courseId, groups]) => {
      for (const [key, value] of Object.entries(groups)) {
        const groupNumber = key;
        const staffIds = value.staffIds
        const periods = value.periods;
        const type = value.type;

        for (const [_, period] of Object.entries(periods)) {
          const row = `${courseId},${groupNumber},${staffIds.join(';')},${period.semester},${convertStringToDayString(period.day)},${period.startTime}:00,${period.endTime}:00,${type}\n`;
          csvContent += row;
        }
      }
    });
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    // Create a download link and trigger the download
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = "data.csv";
    link.click();
  };

  function convertStringToDayString(value) {
    const intValue = parseInt(value, 10);
    const dayOption = daysOfWeekOptions.find(option => option.value === intValue);
    return dayOption ? dayOption.label : '-1';
  }

  return (
    <section className='institution-account'>
      <div className='blur-box'>
        <div>
          Welcome {username}!<br />
          Institution's staff form:<br />
          <Link to={`/form/${formsIds.staffFormId}`}>{`${window.location.origin}/form/${formsIds.staffFormId}`}</Link><br />
          Students' form:<br />
          <Link to={`/form/${formsIds.studentFormId}`}>{`${window.location.origin}/form/${formsIds.studentFormId}`}</Link><br /><br />
          <Button onClick={handleSubmit} buttonStyle='btn--outline'>Close forms and submit data</Button>
        </div>
      </div>
    </section >
  );
}

export default InstitutionAccount;
