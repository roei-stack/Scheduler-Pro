import React, { useEffect, useContext } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { AuthContext, APP_NAME } from '../../AuthContextProvider';
import StudentAccount from './StudentAccount';
import InstitutionAccount from './InstitutionAccount';
import InstitutionSetup from './InstitutionSetup';

function Account() {
  const { username, isInstitution, isSetupNeeded } = useContext(AuthContext);
  const { username: urlUsername } = useParams();
  const navigate = useNavigate();

  useEffect(() => {
    if (username !== urlUsername) {
      navigate('/');
      return;
    }
    window.scrollTo(0, 0);
    document.title = `${username} | ${APP_NAME}`;
    return () => document.title = APP_NAME;
  });

  return (
    <>
      {isSetupNeeded ? (
        <InstitutionSetup />
      ) : (
        <>
          {isInstitution ? <InstitutionAccount /> : <StudentAccount />}
        </>
      )}
    </>
  );
}

export default Account;