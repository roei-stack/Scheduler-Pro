import React, { useEffect } from 'react';
import { APP_NAME } from '../../AuthContextProvider';

function NotFound() {
    useEffect(() => {
        window.scrollTo(0, 0);
        document.title = `404 Not Found | ${APP_NAME}`;
        return () => {
            document.title = APP_NAME;
        };
    }, []);

    return (
        <div className='not-found'>
            <div>
                <h1>Error 404 page not found</h1>
                <small>Check the URL and reload</small>
            </div>
        </div>
    );
}

export default NotFound;
