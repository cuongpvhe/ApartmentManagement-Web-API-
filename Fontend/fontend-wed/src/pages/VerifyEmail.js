import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/api';

const VerifyEmail = () => {
  const [email, setEmail] = useState('');
  const [emailToken, setEmailToken] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleVerifyEmail = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post('/send-reset-email', {
        email,
      });
      console.log('Response from API:', response); // Log response for debugging
      navigate('/resetpassword'); // Redirect to reset password page after email verification
    } catch (error) {
      console.error('Error during email verification:', error); // Log error for debugging
      setError('Email verification failed. Please check your email.');
    }
  };

  return (
    <div>
      <h2>Verify Email</h2>
      <form onSubmit={handleVerifyEmail}>
        <div>
          <label>Email:</label>
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div>
          <label>Email Token:</label>
          <input type="text" value={emailToken} onChange={(e) => setEmailToken(e.target.value)} />
        </div>
        <button type="submit">Verify Email</button>
      </form>
      {error && <p>{error}</p>}
    </div>
  );
};

export default VerifyEmail;
