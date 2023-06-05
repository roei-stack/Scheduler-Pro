import React from 'react';
import { Link } from 'react-router-dom';
import Button from './Button';
import './Footer.css';
import RatingStars from './RatingStars';

const FACEBOOK = 'https://www.facebook.com';

const YOUTUBE = 'https://www.youtube.com/watch?v=dQw4w9WgXcQ';

const LINKEDIN = 'https://www.linkedin.com';

const YEAR = new Date().getFullYear();

function Footer() {
    return (
        <div className='footer-container'>
            <section className='footer-subscription'>
                <p className='footer-subscription-heading'>Your opinion matters! Rate us and let us know how we can improve together</p>
                <p className='footer-subscription-text'>Your rating will be anonymous</p>
                <div className='input-areas'>
                    <form>
                        <RatingStars starColor='#ffef00'/>
                        <input className='footer-input' name='rating' type='text' placeholder='Additional information' />
                        <Button buttonStyle='btn--outline'>Submit</Button>
                    </form>
                </div>
            </section>
            <div class='footer-links'>
                <div className='footer-link-wrapper'>
                    <div class='footer-link-items'>
                        <h2>About Us</h2>
                        <Link to='/'>How it works</Link>
                        <Link to='/'>About</Link>
                        <Link to='/'>Terms of Service</Link>
                    </div>
                </div>
                <div className='footer-link-wrapper'>
                    <div class='footer-link-items'>
                        <h2>Contact Us</h2>
                        <Link to='/'>Contact</Link>
                        <Link to='/'>Support</Link>
                        <Link to='/'>Report bugs</Link>
                    </div>
                </div>
                {/* each 'footer-links-wrapper' can support up to 2 'footer-link-items*/}
            </div>
            <section class='social-media'>
                <div class='social-media-wrap'>
                    <div class='footer-logo'>
                        <Link to='/' className='social-logo'>SCDLR&nbsp;<i class="fa-solid fa-calendar-days"></i></Link>
                    </div>
                    <small class='website-rights'>SCDLR PRO © {YEAR}</small>
                    <div class='social-icons'>
                        <Link class='social-icon-link facebook' to={FACEBOOK} target='_blank'>
                            <i class='fab fa-facebook-f' />
                        </Link>
                        <Link class='social-icon-link youtube' to={YOUTUBE} target='_blank'>
                            <i class='fab fa-youtube' />
                        </Link>
                        <Link class='social-icon-link twitter' to={LINKEDIN} target='_blank'>
                            <i class='fab fa-linkedin' />
                        </Link>
                    </div>
                </div>
            </section>
        </div>
    );
}

export default Footer;
