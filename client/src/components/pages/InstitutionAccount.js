import React, { useContext, useEffect, useRef, useState } from 'react';
import { AuthContext, SERVER, notifyError } from '../../AuthContextProvider';
import { Link } from 'react-router-dom';
import Button from '../Button';
import './Pages.css';
import './Account.css';

function InstitutionAccount() {
  const [formsIds, setFormsIds] = useState({staffFormId: '', studentFormId: ''});
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
          console.log('yay');
      } else {
          throw new Error(`status ${response.status}`);
      }
  }).catch(error => notifyError(error.message));
  };


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
