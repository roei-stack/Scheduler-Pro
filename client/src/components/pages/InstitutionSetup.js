import React, { useEffect, useRef } from 'react';
import './InstitutionSetup.css';
import { Link } from 'react-router-dom';

function InstitutionSetup() {
    return (
        <section className='account-setup'>
            <div className='blur-box'>
                <div>
                    <h1>Welcome to Scheduler Pro!<br /></h1>
                    <h2>Lets get started</h2>
                </div>
            </div>
        </section>
    );
}

export default InstitutionSetup;