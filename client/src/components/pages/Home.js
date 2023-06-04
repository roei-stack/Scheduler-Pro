import React, { useEffect } from 'react';
import { APP_NAME } from '../../AuthContextProvider';
import MainSection from '../MainSection';
import Cards from '../Cards';

function Home() {
  useEffect(() => {
    window.scrollTo(0, 0);
    document.title = `${APP_NAME}: Let's get efficent`;
    return () => document.title = APP_NAME;
  });

  return (
    <>
      <MainSection />
      <Cards />
    </>
  );
}

export default Home;
