import React, { useEffect, useState } from 'react';

function TextGenerate({ text, cooldownMilliseconds = 0, durationMilliseconds, intervalMilliseconds = 45, children }) {
    let timer;
    const [partialText, setPartialText] = useState('');

    useEffect(() => {
        const delayTimer = setTimeout(() => handleGenerate(), cooldownMilliseconds);
        return () => {
            clearTimeout(delayTimer);
            clearInterval(timer);
        }
    }, []);

    const handleGenerate = () => {
        if (partialText === text) {
            return;
        }
        let i = -1;
        const interval = durationMilliseconds ? durationMilliseconds / text.length : intervalMilliseconds;
        timer = setInterval(() => {
            i++;
            if (i === text.length - 1) clearInterval(timer);
            setPartialText(prev => prev + text[i]);
        }, interval);
    };
    return (
        <>
            {partialText}
            {partialText === text && children}
        </>
    );
}

export default TextGenerate;