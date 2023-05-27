import React from 'react';
import { Link } from 'react-router-dom';
import { LINKS } from '../AuthContextProvider';
import './Button.css';

const STYLES = ['btn--primary', 'btn--outline'];

const SIZES = ['btn--medium', 'btn--large'];

function Button({ children, type, to, onClick, buttonStyle, buttonSize }) {
    const checkButtonStyle = STYLES.includes(buttonStyle) ? buttonStyle : STYLES[0];

    const checkButtonSize = SIZES.includes(buttonSize) ? buttonSize : SIZES[0];

    const checkLink = LINKS.includes(to) ? to : LINKS[0];

    return (
        <Link to={checkLink} className='btn-mobile'>
            <button
                className={`btn ${checkButtonStyle} ${checkButtonSize}`}
                onClick={onClick}
                type={type}
            >
                {children}
            </button>
        </Link>
    );
};

export default Button;
