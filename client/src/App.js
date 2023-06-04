import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import AuthContextProvider from './AuthContextProvider';
import Navbar from './components/navbar';
import Home from './components/pages/Home';
import Services from './components/pages/Services';
import Signin from './components/pages/Signin';
import Signup from './components/pages/Signup';
import NotFound from './components/pages/NotFound';
import Footer from './components/Footer';
import Account from './components/pages/Account';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <AuthContextProvider>
        <Navbar />
        <Routes>
          <Route exact path='/' element={<Home />} />
          <Route path='/services' element={<Services />} />
          <Route path='/sign-in' element={<Signin />} />
          <Route path='/sign-up' element={<Signup />} />
          <Route path='/account/:username' element={<Account />} />
          <Route path='*' element={<NotFound />} />
        </Routes>
        <Footer />
      </AuthContextProvider>
    </BrowserRouter>
  );
}

export default App;
