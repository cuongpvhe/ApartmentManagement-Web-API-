import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/api';

const ResetPassword = () => {
  const [email, setEmail] = useState('');
  const [emailToken, setEmailToken] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleResetPassword = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post('/reset-password', {
        email,
        emailToken,
        newPassword,
      });
      console.log('Response from API:', response); // Log response for debugging
      navigate('/login'); // Redirect to login page after successful password reset
    } catch (error) {
      console.error('Error during password reset:', error); // Log error for debugging
      setError('Password reset failed. Please check your inputs.');
    }
  };

  return (
    <div>
      <h2>Reset Password</h2>
      <form onSubmit={handleResetPassword}>
        <div>
          <label>Email:</label>
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div>
          <label>Email Token:</label>
          <input type="text" value={emailToken} onChange={(e) => setEmailToken(e.target.value)} />
        </div>
        <div>
          <label>New Password:</label>
          <input type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
        </div>
        <button type="submit">Reset Password</button>
      </form>
      {error && <p>{error}</p>}
    </div>
  );
};

export default ResetPassword;
