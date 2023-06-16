import React, { useEffect } from 'react';
import { APP_NAME } from '../../AuthContextProvider';
import MainSection from '../MainSection';
import Cards from '../Cards';

function Home() {
  useEffect(() => {
    window.scrollTo(0, 0);
    document.title = `${APP_NAME}: Let's get efficent`;
    return () => document.title = APP_NAME;
  }, []);

  const download = () => {
    const filename = 'data.csv';
    const content = 'Name,Email\nJohn Doe,johndoe@example.com\nJane Smith,janesmith@example.com';
    const csvContent = content;
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }

  return (
    <>
      <button onClick={() => download()}>download</button>
      <MainSection />
      <Cards />
    </>
  );
}

export default Home;
