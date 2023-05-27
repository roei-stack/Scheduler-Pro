import React, { useEffect } from 'react';
import { APP_NAME } from '../../AuthContextProvider';
import './Pages.css'

function Services() {
  useEffect(() => {
    window.scrollTo(0, 0);
    document.title = `Services | ${APP_NAME}`;
    return () => {
      document.title = APP_NAME;
    };
  }, []);

  return (
    <h1 className='services'>
      SERVICES
    </h1>
  );
}

export default Services;
