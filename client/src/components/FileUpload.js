import React, { useState } from 'react';
import Button from './Button';
import { notifyError } from '../AuthContextProvider';

function FileUpload({ onSuccess = () => { }, parseFileContent = () => { }, children }) {
    const [selectedFile, setSelectedFile] = useState(null);
    const [isDisabled, setIsDisabled] = useState(false);

    const handleFileChange = event => setSelectedFile(event.target.files[0]);

    const handleFileUpload = () => {
        if (!selectedFile) {
            notifyError('Please select a file');
            return;
        }
        if (isDisabled) return;

        if (selectedFile.type !== 'text/plain') {
            notifyError('File type must be .txt');
            return;
        }
        const reader = new FileReader();
        reader.onload = e => {
            const fileContent = e.target.result;
            try {
                const parsedData = parseFileContent(fileContent);
                //console.log(parsedData);
                setIsDisabled(true);
                onSuccess(parsedData);
            } catch (error) {
                notifyError(`Error parsing file: ${error.message}`);
            }
        };
        reader.readAsText(selectedFile);
    }

    return (
        <>
            <input type='file' accept='.txt' onChange={handleFileChange} disabled={isDisabled} />
            <Button onClick={handleFileUpload}>
                {children}
            </Button>
        </>
    );
}

export default FileUpload;