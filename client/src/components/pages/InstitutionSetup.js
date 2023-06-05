import React, { useState, useRef } from 'react';
import './InstitutionSetup.css';
import TextGenerate from '../TextGenerate';
import Button from '../Button';
import FileUpload from '../FileUpload';
import { notifyError } from '../../AuthContextProvider';

const fileStages = {2: 'Courses list', 3: 'Students list', 4: 'Lectures list', 5: 'Majors list'};

function InstitutionSetup() {
    const [currentStage, setCurrentStage] = useState(1);
    const nameRef = useRef();

    const handleFirstStageContinue = () => {
        if (finishedStage(1)) return;

        const name = nameRef.current.value;
        if (!name) {
            notifyError('Invalid name')
            return;
        }
        setCurrentStage(2);
    }

    const finishedStage = n => currentStage > n;

    return (
        <section className='account-setup'>
            <div className='blur-box'>
                <div className='box-container'>
                    <TextGenerate text='Welcome to Scheduler Pro!' > <br />
                        <TextGenerate text="Let's get started" > <br />
                            <div className='setup-input-wrapper'>
                                <TextGenerate text="Enter your institution's name" ><br />
                                    <TextGenerate text="This will be used by students to find you" ><br />
                                        <div className='setup-menu-input'>
                                            <i
                                                class={`fa-solid fa-${finishedStage(1) ? 'check' : 'arrow-right-long'}`}
                                                style={{ color: finishedStage(1) ? "#00ff00" : "#ffffff", }}
                                            />
                                            <input type='text' ref={nameRef} disabled={finishedStage(1)} autoFocus />
                                            <Button onClick={handleFirstStageContinue}>
                                                <TextGenerate intervalMilliseconds={150} text='Continue' />
                                            </Button>
                                        </div>
                                    </TextGenerate>
                                </TextGenerate>
                            </div>
                        </TextGenerate>
                    </TextGenerate>
                    {Object.entries(fileStages).map(([stage, text]) => (
                        finishedStage(stage - 1) &&
                        <div className='setup-input-wrapper'>
                            <TextGenerate text={`Upload ${text} (.txt)`} >
                                <div className='setup-menu-input'>
                                    <i
                                        class={`fa-solid fa-${finishedStage(stage) ? 'check' : 'arrow-right-long'}`}
                                        style={{ color: finishedStage(stage) ? "#00ff00" : "#ffffff", }}
                                    />
                                    <FileUpload onSuccess={() => setCurrentStage(currentStage + 1)}>
                                        <TextGenerate text='Upload' intervalMilliseconds={150} />
                                    </FileUpload>
                                </div>
                            </TextGenerate>
                        </div>
                    ))}
                </div>
            </div>
        </section>
    );
}

export default InstitutionSetup;