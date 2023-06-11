import React, { useState, useRef, useContext } from 'react';
import './InstitutionSetup.css';
import TextGenerate from '../TextGenerate';
import Button from '../Button';
import FileUpload from '../FileUpload';
import { AuthContext, SERVER, notifyError } from '../../AuthContextProvider';
import { parseCoursesFileContent } from '../../fileUtils';

const STAGES = 5;

// parse function accepts a string, and it returns data/throws an error when it fails
const fileStages = {
    2: { text: 'Courses list', parseFunction: parseCoursesFileContent },
    3: { text: 'Students list', parseFunction: () => { } },
    4: { text: 'Lectures list', parseFunction: () => { } },
    5: { text: 'Majors list', parseFunction: () => { } },
};

function InstitutionSetup() {
    const { username, token } = useContext(AuthContext);
    const [currentStage, setCurrentStage] = useState(1);
    const [dataLists, setDataLists] = useState({});
    const nameRef = useRef();

    const handleInstitutionNameCheck = () => {
        if (finishedStage(1)) return;
        const name = nameRef.current.value;
        // client side
        if (!name) {
            notifyError('Invalid name');
            return;
        }
        // server side
        fetch(`${SERVER}/Account/checkInstitutionName`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(name)
        }).then(response => {
            if (response.ok) {
                setCurrentStage(2);
            } else {
                throw new Error('Institution name is not available');
            }
        }).catch(error => notifyError(error.message));
    }

    const saveData = (index, data) => {
        setDataLists(prevData => ({
            ...prevData,
            [index]: data,
        }));
    };

    const onSuccessfulParse = data => {
        // file upload stages start from stage 2, and array indices start from 0
        const index = currentStage - 2;
        saveData(index, data);
        if (isFinishedAllStages()) {
            // Send the data to the server
            console.log('DONE');
        }
        setCurrentStage(currentStage + 1);
    }

    const finishedStage = n => currentStage > n;

    const isFinishedAllStages = () => currentStage === STAGES + 1;

    return (
        <section className='account-setup'>
            <div className='blur-box'>
                <div className='box-container'>
                    <TextGenerate text={`Hello ${username}, welcome to Scheduler Pro!`} ><br />
                        <TextGenerate text="Let's get started" > <br />
                            <div className='setup-input-wrapper'>
                                <TextGenerate text="Enter your institution's name" ><br />
                                    <TextGenerate text="This will be used by students to find you" ><br />
                                        <div className='setup-menu-input'>
                                            <i
                                                className={`fa-solid fa-${finishedStage(1) ? 'check' : 'arrow-right-long'}`}
                                                style={{ color: finishedStage(1) ? "#00ff00" : "#ffffff", }}
                                            />
                                            {/*<i class="fa-solid fa-spinner fa-spin-pulse" />*/}
                                            <input type='text' ref={nameRef} disabled={finishedStage(1)} autoFocus />
                                            <Button onClick={handleInstitutionNameCheck}>
                                                <TextGenerate intervalMilliseconds={150} text='Continue' />
                                            </Button>
                                        </div>
                                    </TextGenerate>
                                </TextGenerate>
                            </div>
                        </TextGenerate>
                    </TextGenerate>
                    {Object.entries(fileStages).map(([stage, { text, parseFunction }]) => (
                        finishedStage(stage - 1) &&
                        <div className='setup-input-wrapper' key={stage}>
                            <TextGenerate text={`Upload ${text}`} >
                                <div className='setup-menu-input'>
                                    <i
                                        className={`fa-solid fa-${finishedStage(stage) ? 'check' : 'arrow-right-long'}`}
                                        style={{ color: finishedStage(stage) ? "#00ff00" : "#ffffff", }}
                                    />
                                    <FileUpload
                                        parseFileContent={content => parseFunction(content)}
                                        onSuccess={data => onSuccessfulParse(data)}>
                                        <TextGenerate text='Upload' intervalMilliseconds={150} />
                                    </FileUpload>
                                </div>
                            </TextGenerate>
                        </div>
                    ))}
                    {isFinishedAllStages() &&
                        <div className='setup-input-wrapper'>
                            <i
                                className={`fa-solid ${false ? 'fa-spinner fa-spin-pulse' : 'fa-check'}`}
                                style={{ color: false ? '#ffffff' : '#00ff00', }}
                            />
                            <TextGenerate text={false ? ' Just one moment' : " You're good to go!"} />
                        </div>
                    }
                </div>
            </div>
        </section>
    );
}

export default InstitutionSetup;
