import React, { useEffect, useRef } from 'react';
import './InstitutionSetup.css';
import { Link } from 'react-router-dom';
import TextGenerate from '../TextGenerate';

function InstitutionSetup() {
    return (
        <section className='account-setup'>
            <div className='blur-box'>
                <div>
                    <h1><TextGenerate text='Welcome to Scheduler Pro!' cooldownMilliseconds={1000} durationMilliseconds={1500}/><br /></h1>
                    <h2><TextGenerate text="Let's get started" cooldownMilliseconds={3000} durationMilliseconds={1000}/><br /></h2>
                </div>
            </div>
        </section>
    );
}

export default InstitutionSetup;