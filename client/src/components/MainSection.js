import React, { useEffect, useRef } from 'react';
import Button from './Button';
import './MainSection.css';

function MainSection() {
  const videoRef = useRef(null);

  useEffect(() => {
    // control background video speed
    videoRef.current.playbackRate = 0.45;
  }, []);

  return (
    <div className='main-container'>
      <video src='/videos/video-homebg.mp4' autoPlay loop muted ref={videoRef} />
      <h1>LET'S GET EFFICENT</h1>
      <p>Organize your calender today!</p>
      <div className='main-btns'>
        <Button to='/sign-in' buttonStyle='btn--outline' buttonSize='btn--large'>GET STARTED</Button>
        <Button to='/services' buttonStyle='btn--primary' buttonSize='btn--large'>
          LEARN MORE<i class="main-icon fa-solid fa-book-open-reader"></i>
        </Button>
      </div>
    </div>
  );
}

export default MainSection;
