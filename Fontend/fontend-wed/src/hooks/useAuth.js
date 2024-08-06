import { useEffect } from 'react';
import axios from 'axios';
import api from '../api/api'; // Import the centralized API instance

const useAuth = () => {
  useEffect(() => {
    const refreshAccessToken = async () => {
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
          const response = await api.post('/refresh', { refreshToken });
          localStorage.setItem('accessToken', response.data.accessToken);
          api.defaults.headers.common['Authorization'] = `Bearer ${response.data.accessToken}`;
        }
      } catch (error) {
        console.error('Failed to refresh access token:', error);
        // Optionally: handle token refresh failure (e.g., redirect to login)
      }
    };

    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      // Add a response interceptor to handle token expiration
      const interceptor = api.interceptors.response.use(
        (response) => response,
        async (error) => {
          if (error.response && error.response.status === 401) {
            await refreshAccessToken();
            // Retry the original request with the new access token
            const originalRequest = error.config;
            const newAccessToken = localStorage.getItem('accessToken');
            if (newAccessToken) {
              originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
              return api(originalRequest);
            }
          }
          return Promise.reject(error);
        }
      );

      // Clean up the interceptor on component unmount
      return () => {
        api.interceptors.response.eject(interceptor);
      };
    }
  }, []);
};

export default useAuth;
