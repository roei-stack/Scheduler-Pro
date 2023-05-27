import React from 'react';
import CardItem from './CardItem';
import './Cards.css';

function Cards() {
  return (
    <div className='cards'>
      <h1>Learn more about our project!</h1>
      <div className='cards-container'>
        <div className='cards-wrapper'>
          <ul className='cards-items'>
            <CardItem
              src='images/img-pupilcard.avif'
              text='Plan your next semester now - Create your own scedual in a new, smart way'
              label='For Students'
              to='/sign-in'
            />
            <CardItem
              src='images/img-schoolcard.webp'
              text="Optimize your institution's efficency - plan the school's entire scedule now, without the usual headace"
              label='For Schools'
              to='/sign-in'
            />
          </ul>
        </div>
      </div>
    </div>
  );
}

export default Cards;
