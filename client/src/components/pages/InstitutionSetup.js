import React, { useEffect, useRef } from 'react';
import './InstitutionSetup.css';
import { Link } from 'react-router-dom';
import TextGenerate from '../TextGenerate';

let typeWriterPace = 45

function InstitutionSetup() {
    return (
        <section className='account-setup'>
            <div className='blur-box'>
                <div>
                    <TextGenerate text='Welcome to Scheduler Pro!' cooldownMilliseconds={0} intervalMilliseconds={typeWriterPace} > <br />
                        <TextGenerate text="Let's get started" intervalMilliseconds={typeWriterPace} > <br />
                            <TextGenerate text="Enter your institution's name" intervalMilliseconds={typeWriterPace} ><br />
                                <TextGenerate text="This name will be used by other users to find you" intervalMilliseconds={typeWriterPace} >
                                    <br /><input type='text' />
                                </TextGenerate>
                            </TextGenerate>
                        </TextGenerate>
                    </TextGenerate>
                </div>

            </div>
        </section>
    );
}

export default InstitutionSetup;