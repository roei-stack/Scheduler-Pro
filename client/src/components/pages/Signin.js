import React, { useEffect, useState, useRef, useContext } from 'react';
import { AuthContext, SERVER, notifyError, APP_NAME } from '../../AuthContextProvider';
import { Link } from 'react-router-dom';
import Button from '../Button';
import './Sign.css';
import './Pages.css';

function Signin() {
  const { isSignedIn, handleSignIn, handleSignOut } = useContext(AuthContext);
  const usernameRef = useRef();
  const passwordRef = useRef();
  const remembermeRef = useRef();
  const [pwVisable, setPwVisable] = useState(false);

  const isMountedRef = useRef(false);
  useEffect(() => {
    if (!isMountedRef.current) {
      isMountedRef.current = true;
      return;
    }
    if (isSignedIn) { handleSignOut(); }
    window.scrollTo(0, 0);
    document.title = `Sign In | ${APP_NAME}`;
    return () => document.title = APP_NAME;
  });

  //todo not two notifications
  const handleSubmit = e => {
    e.preventDefault();
    const username = usernameRef.current.value;
    const password = passwordRef.current.value;
    const rememberMe = remembermeRef.current.checked;

    // validations - client side
    if (!username || !password) {
      notifyError('Incorrect username or password');
      return;
    }
    // validations - server side
    fetch(`${SERVER}/Account/signin`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password, rememberMe })
    }).then(response => {
      if (response.ok) {
        return response.json();
      } else if (response.status === 401) {
        // Unauthorized
        throw new Error('Incorrect username or password');
      } else {
        throw new Error('An error occurred, please try again later');
      }
    }).then(data => handleSignIn(data))
      .catch(error => notifyError(error.message));
  };

  return (
    <section className='sign-in'>
      <div className='sign-form-box'>
        <div>
          <form>
            <h2>Sign in</h2>
            <div className='sign-input-box'>
              <label>Username</label>
              <i className="fa-solid fa-user" />
              <input type='text' ref={usernameRef}></input>
            </div>
            <div className='sign-input-box'>
              <label>Password</label>
              <i
                class={pwVisable ? 'fa-solid fa-eye' : 'fa-solid fa-lock'}
                onMouseDown={() => setPwVisable(true)}
                onMouseUp={() => setPwVisable(false)}
                onMouseLeave={() => setPwVisable(false)}
              />
              <input type={pwVisable ? 'text' : 'password'} ref={passwordRef}></input>
            </div>
            <div className='sign-forgot-pass'>
              <label><input type='checkbox' ref={remembermeRef} />Remember Me</label>
              <Link>Forgot Password?</Link>
            </div>
            <Button buttonStyle='btn--primary' buttonSize='btn--large' onClick={handleSubmit}>Sign in</Button>
            <div className='sign-register-link'>
              <p>{`New to ${APP_NAME}?`}&nbsp;<Link to='/sign-up'>Register</Link></p>
            </div>
          </form>
        </div>
      </div>
    </section>
  );
}

export default Signin;

/*
old fetch:
    /*try {
      const response = await fetch(`${SERVER}/Account/signin`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password, rememberMe })
      });

      if (response.ok) {
        // todo get token and send it to handlesignin
        const data = await response.json();
        handleSignIn(username, data.token);
        navigate(`/account/${username}`);
      } else if (response.status === 401) {
        // Unauthorized
        notifyError('Incorrect username or password');
      } else {
        notifyError('An error occured')
      }
    } catch (ex) {
      notifyError(`${ex.message}, please try again later`);
    }*/