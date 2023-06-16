import React, { useEffect, useState, useRef, useContext } from 'react';
import { Link } from 'react-router-dom';
import Button from '../Button';
import './Pages.css';
import './Account.css';
import { AuthContext, SERVER, notifyError } from '../../AuthContextProvider';
import ScheduleTable from './ScheduleTable';

function Account() {
  const { token } = useContext(AuthContext);

  const isMountedRef = useRef(false);
  useEffect(() => {
    if (!isMountedRef.current) {
      isMountedRef.current = true;
      return;
    }
    //fetchScheduels()
  }, []);

  const fetchScheduels = () => {
    fetch(`${SERVER}/Account/schedualList`, {
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
    }).then(data => {

    }).catch(error => notifyError(error.message));
  };

  return (
    <section className='account'>
      <nav className="side-menu">
        <div className='side-menu-title'>
          <h1>Your schedules</h1>
          <Button buttonStyle='btn--outline'><i class="fa-solid fa-plus"></i>&nbsp;New</Button>
        </div>
        <ul>
          {[...Array(3)].map((item) => (
            <li key={item} className='side-menu-item'>
              <Link onClick={() => { }}>
                <i className={`main-icon fa-solid fa-book-open-reader`} />
                <span>schedual</span>
              </Link>
            </li>
          ))}
        </ul>
      </nav>
      <div className='right-side'>
        <ScheduleTable />
      </div>
    </section >
  );
}

export default Account;
