import React, { createContext, useState, useEffect } from 'react';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import jwt_decode from 'jwt-decode';
import { useNavigate } from 'react-router-dom';

// global objects/variables
export const APP_NAME = 'Scheduler Pro';
export const SERVER = 'https://localhost:7147/api';
// null means the button does not link anywhere (default option), '/' is the home page
export const LINKS = [null, '/', '/sign-in', '/sign-up', '/services', '/account']; // todo last item fix
const CLAIM_NAMES = {
  USERNAME: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
  ROLE: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
  EXPIRE: 'exp'
};
// create context for authentication
export const AuthContext = createContext({
  username: '',
  isSignedIn: false,
  handleSignIn: () => { },
  handleSignOut: () => { }
});
// create alerting functions, more info: https://fkhadra.github.io/react-toastify/introduction/
export const notifyError = text => toast.error(text, { position: "bottom-left", theme: "dark" });
export const notifySuccess = text => toast.success(text, { position: "bottom-left", theme: "dark" });
export const notifyInfo = text => toast.info(text, { position: "bottom-left", theme: "dark" });

function AuthContextProvider({ children }) {
  const [username, setUsername] = useState('');
  const [isSignedIn, setIsSignedIn] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  // load token from local storage and check if it is valid on component mount
  useEffect(() => {
    const token = localStorage.getItem('token');
    const decodedToken = token ? decodeToken(token) : null;
    if (decodedToken) {
      const isValid = decodedToken.expire ? Date.now() <= decodedToken.expire * 1000 : false;
      if (isValid) {
        setUsername(decodedToken.username);
        setIsSignedIn(true);
      } else {
        localStorage.removeItem('token');
      }
    }
    setIsLoading(false);
  }, [setUsername, setIsSignedIn]);

  // handle and change states when user signes in or out
  const handleSignIn = (username, token) => {
    notifySuccess(`Welcome, ${username}!`)
    localStorage.setItem('token', token);
    setUsername(username);
    setIsSignedIn(true);
    navigate(`/account/${username}`);
  };

  const handleSignOut = () => {
    notifyInfo('You have been signed out');
    localStorage.removeItem('token');
    setUsername('');
    setIsSignedIn(false);
  };

  // attempt to decode the token with jwt-decode()
  const decodeToken = token => {
    try {
      const decodedToken = jwt_decode(token);
      const username = decodedToken[CLAIM_NAMES.USERNAME];
      const isInstitution = decodedToken[CLAIM_NAMES.ROLE] === 'Institution';
      const expire = decodedToken[CLAIM_NAMES.EXPIRE];
      return { username, isInstitution, expire };
    } catch (ex) {
      console.error(`Error decoding token: ${ex.message}`);
      return null;
    }
  }

  return (
    <AuthContext.Provider value={{ username, isSignedIn, handleSignIn, handleSignOut }}>
      {/* only render the app once the token has been checked */}
      {!isLoading && children}
      {/* render toast notifications/alerts */}
      <ToastContainer />
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;

/*const token = localStorage.getItem('token');
    if (token) {
      const decodedToken = jwt_decode(token);
      const decodedUsername = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      verifyToken(token).then(response => {
        if (response.ok) {
          setUsername(decodedUsername);
          setIsSignedIn(true);
        } else {
          localStorage.removeItem('token');
        }
      }).catch(ex => console.error(`Token validation failed: ${ex}`)).finally(() => setIsLoading(false));
    } else {
      setIsLoading(false);
    }*/
/*
  // server side token verification request
  const verifyToken = async token => {
    const response = await fetch(`${SERVER}/Account/verifyToken`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    return response;
  }
  */