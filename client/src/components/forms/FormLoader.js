import React, { useEffect, useRef, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom';
import { SERVER, notifyError } from '../../AuthContextProvider';
import StaffForm from './StaffForm';
import StudentForm from './StudentForm';
import '../pages/Pages.css';
import './forms.css';
import '../pages/InstitutionSetup.css'

function FormLoader() {
    const { formId: urlFormId } = useParams();
    const navigate = useNavigate();
    const isMountedRef = useRef(false);
    const [formType, setFormType] = useState(null);
    const [courseIds, setCourseIds] = useState([]);

    useEffect(() => {
        if (!isMountedRef.current) {
            isMountedRef.current = true;
            return;
        }
        checkFormId();
    }, []);

    const checkFormId = () => {
        fetch(`${SERVER}/checkFormId/${urlFormId}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        }).then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Form not found');
            }
        }).then(data => {
            setFormType(data.formType);
            setCourseIds(data.courseIds);
        }).catch(error => {
                notifyError(error.message);
                navigate('/404-form-not-found');
            });
    }

    return (
        <div className='form'>
            <div className='blur-box'>
                <div className='box-container'>
                    {formType === 'staff' ? <StaffForm /> : <StudentForm courseIds={courseIds}/>}
                </div>
            </div>
        </div>
    );
}

export default FormLoader;
