import React, { useState, useEffect, useContext } from 'react';
import { AuthContext } from '../AuthContextProvider';
import { Link } from 'react-router-dom';
import Button from './Button';
import './navbar.css';

function Navbar() {
    const { username, isSignedIn, handleSignOut } = useContext(AuthContext);

    const [click, setClick] = useState(false);
    const [button, setButton] = useState(true);

    const handleClick = () => setClick(!click);
    const closeMobileMenu = () => setClick(false);

    const showButton = () => {
        if (window.innerWidth <= 960) {
            setButton(false);
        } else {
            setButton(true);
        }
    };

    useEffect(() => { showButton() }, []); // or set button default to false

    window.addEventListener('resize', showButton);

    return (
        <nav className='navbar'>
            <div className='navbar-container'>
                <Link to='/' className='navbar-logo' onClick={closeMobileMenu}>
                    SCDLR&nbsp;<i className="fa-solid fa-calendar-days"></i>
                </Link>
                <div className='menu-icon' onClick={handleClick}>
                    <i className={click ? 'fas fa-times' : 'fas fa-bars'} />
                </div>
                <ul className={click ? 'nav-menu active' : 'nav-menu'}>
                    <li className='nav-item'>
                        <Link to='/' className='nav-links' onClick={closeMobileMenu}>
                            Home&nbsp;<i className="fa-solid fa-home"></i>
                        </Link>
                    </li>
                    <li className='nav-item'>
                        <Link to='/services' className='nav-links' onClick={closeMobileMenu}>
                            Services&nbsp;<i className="fa-solid fa-gears"></i>
                        </Link>
                    </li>
                    {/* nav-links-mobile will only show up on mobile view */}
                    {isSignedIn ? (
                        <>
                            <li className='nav-item'>
                                <Link to={`/account/${username}`} className='nav-links' onClick={closeMobileMenu}>
                                    Account&nbsp;<i className="fa-solid fa-receipt"></i>
                                </Link>
                            </li>
                            <li className='nav-item'>
                                <Link to='/' className='nav-links-mobile' onClick={() => { closeMobileMenu(); handleSignOut(); }}>
                                    Sign out
                                </Link>
                            </li>
                        </>
                    ) : (
                        <>
                            <li className='nav-item'>
                                <Link to='/sign-in' className='nav-links-mobile' onClick={closeMobileMenu}>
                                    Sign in
                                </Link>
                            </li> <li className='nav-item'>
                                <Link to='/sign-up' className='nav-links-mobile' onClick={closeMobileMenu}>
                                    Sign up
                                </Link>
                            </li>
                        </>
                    )}
                </ul>
                <div className='btn--signing'>
                    {isSignedIn ? (
                        button && <Button to='/' buttonStyle='btn--outline' onClick={handleSignOut}>Sign out</Button>
                    ) : (
                        <>
                            {button && <Button to='/sign-in' buttonStyle='btn--outline'>Sign in</Button>}
                            {button && <Button to='/sign-up' buttonStyle='btn--outline'>Sign up</Button>}
                        </>
                    )}
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
