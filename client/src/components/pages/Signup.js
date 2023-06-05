import React, { useEffect, useState, useRef, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext, SERVER, notifyError, notifySuccess, APP_NAME } from '../../AuthContextProvider';
import Button from '../Button';
import './Pages.css';
import './Sign.css';

function Signup() {
  const { isSignedIn, handleSignOut } = useContext(AuthContext);
  const navigate = useNavigate();
  const usernameRef = useRef();
  const passwordRef = useRef();
  const confirmRef = useRef();
  const isinstitutionRef = useRef();
  const [pwVisable, setPwVisable] = useState(false);

  const isMountedRef = useRef(false);
  useEffect(() => {
    // this check makes sure the useEffect will only run once
    if (!isMountedRef.current) {
      isMountedRef.current = true;
      return;
    }
    if (isSignedIn) { handleSignOut(); }
    window.scrollTo(0, 0);
    document.title = `Sign Up | ${APP_NAME}`;
    return () => document.title = APP_NAME;
  });

  const handleSubmit = e => {
    e.preventDefault();
    const username = usernameRef.current.value;
    const password = passwordRef.current.value;
    const confirmPass = confirmRef.current.value;
    const isInstitution = isinstitutionRef.current.checked;

    // validations - client side
    if (!username || !password || !confirmPass) {
      notifyError('Please fill out all feilds');
      return;
    }
    if (password !== confirmPass) {
      notifyError("The passwords don't match");
      return;
    }
    // validations - server side
    fetch(`${SERVER}/Account/signup`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password, isInstitution })
    }).then(response => {
      if (response.ok) {
        notifySuccess('Account created successfully!');
        navigate('/sign-in');
      } else if (response.status === 401) {
        throw new Error('This username already exists');
      } else {
        throw new Error('An error occurred, please try again later');
      }
    }).catch(error => notifyError(error.message));
  }

  return (
    <section className='sign-up'>
      <div className='blur-box'>
        <div>
          <form>
            <h2>Sign up</h2>
            <div className='sign-input-box'>
              <label>Username</label>
              <i class="fa-solid fa-user" />
              <input type='text' ref={usernameRef} ></input>
            </div>
            <div className='sign-input-box'>
              <label>Password</label>
              <i
                className={pwVisable ? 'fa-solid fa-eye' : 'fa-solid fa-lock'}
                onMouseDown={() => setPwVisable(true)}
                onMouseUp={() => setPwVisable(false)}
                onMouseLeave={() => setPwVisable(false)}
              />
              <input type={pwVisable ? 'text' : 'password'} ref={passwordRef} ></input>
            </div>
            <div className='sign-input-box'>
              <label>Confirm Password</label>
              <i
                className={pwVisable ? 'fa-solid fa-eye' : 'fa-solid fa-lock'}
                onMouseDown={() => setPwVisable(true)}
                onMouseUp={() => setPwVisable(false)}
                onMouseLeave={() => setPwVisable(false)}
              />
              <input type={pwVisable ? 'text' : 'password'} ref={confirmRef} ></input>
            </div>
            <div className='sign-forgot-pass'>
              <label><input type='checkbox' ref={isinstitutionRef} />Educational institution account</label>
            </div>
            <Button buttonStyle='btn--primary' buttonSize='btn--large' onClick={handleSubmit}>Sign up</Button>
            <div className='sign-register-link'>
              <p>Already registered?&nbsp;<Link to='/sign-in'>Sign in</Link></p>
            </div>
          </form>
        </div>
      </div>
    </section>
  );
}

export default Signup;

/*
old
    /*try {
      const response = await fetch(`${SERVER}/Account/signup`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password, isInstitution })
      });

      if (response.ok) {
        notifySuccess('Account created successfully!');
        navigate('/sign-in');
      } else if (response.status === 401) {
        notifyError('This username already exists');
      } else {
        notifyError('an error occured');
      }
    } catch (ex) {
      notifyError(`${ex.message}, please try again later`);
    }

older
/*const response = await fetch(`${SERVER}/Account/signup`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password, isInstitution })
    }).catch(ex => {
      notifyError(`${ex.message}, please try again later`);
      return;
    });

    if (response) {
      if (response.ok) {
        navigate('/sign-in');
      } else if (response.status == 401) {
        notifyError('This username already exists');
      } else {
        notifyError('an error occured');
      }
    }*/