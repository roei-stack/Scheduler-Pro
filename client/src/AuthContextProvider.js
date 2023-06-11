import React, { createContext, useState, useEffect } from 'react';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { useNavigate } from 'react-router-dom';
import jwt_decode from 'jwt-decode';

// global objects/variables
export const APP_NAME = 'Scheduler Pro';
export const SERVER = 'https://localhost:7147/api';
// null means the button does not link anywhere (default option), '/' is the home page
export const LINKS = [null, '/', '/sign-in', '/sign-up', '/services', '/account']; // todo last item fix
// create context for authentication
export const AuthContext = createContext({
  token: null,
  username: '',
  isInstitution: null,
  isSetupNeeded: null,
  isSignedIn: false,
  handleSignIn: () => { },
  handleSignOut: () => { }
});
// create alerting functions, more info: https://fkhadra.github.io/react-toastify/introduction/
export const notifyError = text => toast.error(text, { position: "bottom-left", theme: "dark" });
export const notifySuccess = text => toast.success(text, { position: "bottom-left", theme: "dark" });
export const notifyInfo = text => toast.info(text, { position: "bottom-left", theme: "dark" });

function AuthContextProvider({ children }) {
  const [token, setToken] = useState(null);
  const [username, setUsername] = useState('');
  const [isInstitution, setIsInstitution] = useState(null);
  const [isSetupNeeded, setIsSetupNeeded] = useState(null);
  const [isSignedIn, setIsSignedIn] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  // load token from local storage and check if it is valid on component mount
  // todo maybe empty dependancy array
  useEffect(() => {
    const token = localStorage.getItem('token');
    const decodedToken = token ? decodeToken(token) : null;
    if (decodedToken) {
      const isValid = Date.now() <= decodedToken.expire * 1000;
      if (isValid) {
        setToken(token);
        setUsername(decodedToken.username);
        setIsInstitution(decodedToken.isInstitution);
        setIsSetupNeeded(decodedToken.isSetupNeeded);
        setIsSignedIn(true);
      } else {
        localStorage.removeItem('token');
      }
    }
    setIsLoading(false);
  }, [setToken, setUsername, setIsInstitution, setIsSetupNeeded, setIsSignedIn]);

  // handle and change states when user signes in or out
  const handleSignIn = data => {
    // greeting user
    if (!data.needSetup) notifySuccess(`Welcome, ${data.username}!`);
    // saving nessasary data and invoking ui updates
    localStorage.setItem('token', data.token);
    setToken(data.token);
    setUsername(data.username);
    setIsInstitution(data.isInstitution);
    setIsSetupNeeded(data.needSetup);
    setIsSignedIn(true);
    // redirecting (institutions must go through first time setups)
    navigate(`/account/${data.username}`);
  };

  const handleSignOut = () => {
    notifyInfo('You have been signed out');
    // removing user's data and invoking ui updates
    localStorage.removeItem('token');
    setToken(null);
    setUsername('');
    setIsInstitution(null);
    setIsSetupNeeded(null);
    setIsSignedIn(false);
  };

  // attempt to decode the token with jwt-decode()
  const decodeToken = token => {
    try {
      const decodedToken = jwt_decode(token);
      const username = decodedToken['username'];
      const isInstitution = decodedToken['isInstitution'] === 'true';
      const isSetupNeeded = decodedToken['isSetupNeeded'] === 'true';
      const expire = decodedToken['exp'];
      return { username, isInstitution, isSetupNeeded, expire };
    } catch (ex) {
      console.error(`Error decoding token: ${ex.message}`);
      return null;
    }
  }

  return (
    <AuthContext.Provider value={{ token, username, isInstitution, isSetupNeeded, isSignedIn, handleSignIn, handleSignOut }}>
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