import React, { useEffect, useState, useRef, useContext } from 'react';
import { Link } from 'react-router-dom';
import Button from '../Button';
import './Pages.css';
import './Account.css';
import { AuthContext, SERVER, notifyError } from '../../AuthContextProvider';
import ScheduleTable from './ScheduleTable';
import StudentUserForm from '../forms/StudentUserForm';

function Account() {
  const { token } = useContext(AuthContext);
  const [schedules, setSchedules] = useState([]);
  const [showForm, setShowFrom] = useState(false);

  const isMountedRef = useRef(false);
  useEffect(() => {
    if (!isMountedRef.current) {
      isMountedRef.current = true;
      return;
    }
    fetchScheduels()
  }, []);

  const fetchScheduels = () => {
    fetch(`${SERVER}/Account/schedulesList`, {
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
    }).then(data => setSchedules(data.schedules))
      .catch(error => notifyError(error.message));
  };

  const isEmptySchedules = () => schedules.length === 0;

  return (
    <>{showForm? <StudentUserForm onAbort={() => setShowFrom(false)}/> :
    <section className='account'>
      <nav className="side-menu">
        <div className='side-menu-title'>
          <h1>{isEmptySchedules() ? 'No schedules found' : 'Your schedules'}</h1>
          <Button onClick={() => setShowFrom(true)} buttonStyle='btn--outline'><i class="fa-solid fa-plus"></i>&nbsp;New</Button>
        </div>
        {!isEmptySchedules() && 
        <ul>
          {[...Array(3)].map((item) => (
            <li key={item} className='side-menu-item'>
              <Link onClick={() => { }}>
                <i className={`main-icon fa-solid fa-book-open-reader`} />
                <span>schedual</span>
              </Link>
            </li>
          ))}
        </ul>}
      </nav>
      <div className='right-side'>
        {isEmptySchedules() ? <>Click "New" to create a new schedual</> : <ScheduleTable />}
      </div>
    </section >}</>
  );
}

export default Account;
