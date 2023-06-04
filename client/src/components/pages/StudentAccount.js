import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import Button from '../Button';
import './Pages.css';
import './Account.css';

function Account() {
  const [selectedSchedule, setSelectedSchedule] = useState(null);
  const handleScheduleClick = item => setSelectedSchedule(item);

  const schedules = [
    { id: 1, iconClass: 'fa-solid fa-book-open-reader', title: 'schedule 1' },
    { id: 2, iconClass: 'fa-solid fa-book-open-reader', title: 'schedule 2' },
    { id: 3, iconClass: 'fa-solid fa-book-open-reader', title: 'schedule 3' },
  ];

  return (
    <section className='account'>
      <nav className="side-menu">
        <div className='side-menu-title'>
          <h1>Your schedules</h1>
          <Button buttonStyle='btn--outline'><i class="fa-solid fa-plus"></i>&nbsp;New</Button>
        </div>
        <ul>
          {schedules.map(item => (
            <li key={item.id} className='side-menu-item'>
              <Link onClick={() => handleScheduleClick(item)}>
                <i className={`main-icon ${item.iconClass}`} />
                <span>{item.title}</span>
              </Link>
            </li>
          ))}
        </ul>
      </nav>
      <div className='right-side'>
        {selectedSchedule ? (<h2>{selectedSchedule.title}</h2>) : (<p>Default html...</p>)}
      </div>
    </section >
  );
}

export default Account;
