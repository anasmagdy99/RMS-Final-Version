import apiClient from './api';

// Authentication Service
export const authService = {
    // Login
    login: async (code, password) => {
        const response = await apiClient.post('/auth/login', { code, password });
        return response.data;
    },

    // Logout
    logout: () => {
        localStorage.removeItem('user');
        window.location.href = '/login';
    },

    // Get current user
    getCurrentUser: () => {
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user) : null;
    },

    // Save user to localStorage
    saveUser: (user) => {
        localStorage.setItem('user', JSON.stringify(user));
    },

    // Check if user is authenticated
    isAuthenticated: () => {
        return !!localStorage.getItem('user');
    },
};

export default authService;
