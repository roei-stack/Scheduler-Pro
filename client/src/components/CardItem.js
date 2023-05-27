import React from 'react';
import { Link } from 'react-router-dom';
import { LINKS } from '../AuthContextProvider';

function CardItem(props) {
  const checkLink = LINKS.includes(props.to) ? props.to : LINKS[0];

  return (
    <li className='cards-item'>
      <Link to={checkLink} className='cards-item-link' >
        <figure className='cards-item-pic-wrap' data-category={props.label}>
          <img className='cards-item-img' alt='card pic' src={props.src} />
        </figure>
        <div className='cards-item-info'>
          <h5 className='cards-item-text'>{props.text}</h5>
        </div>
      </Link>
    </li>
  );
}

export default CardItem;
