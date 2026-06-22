import apiClient from './api';

// Request Service
export const requestService = {
    // Get all requests (HR/Board)
    getAllRequests: async () => {
        const response = await apiClient.get('/transactions/all');
        return response.data;
    },

    // Get my requests
    getMyRequests: async () => {
        const response = await apiClient.get('/transactions/my-requests');
        return response.data;
    },

    // Get team requests (Manager)
    getTeamRequests: async () => {
        const response = await apiClient.get('/transactions/my-team-requests');
        return response.data;
    },

    // Create new request
    create: async (requestData) => {
        const response = await apiClient.post('/transactions', requestData);
        return response.data;
    },

    // Update request
    update: async (id, requestData) => {
        const response = await apiClient.put(`/transactions/${id}`, requestData);
        return response.data;
    },

    // Cancel request
    cancel: async (id) => {
        const response = await apiClient.post(`/transactions/${id}/cancel`);
        return response.data;
    },

    // Approve request (Manager/HR)
    approve: async (id) => {
        const response = await apiClient.post(`/transactions/${id}/approve`);
        return response.data;
    },

    // Reject request (Manager/HR)
    reject: async (id, reason) => {
        const response = await apiClient.post(`/transactions/${id}/reject`, { reason });
        return response.data;
    },

    // Delete request
    delete: async (id) => {
        const response = await apiClient.delete(`/transactions/${id}`);
        return response.data;
    },
};

export default requestService;
