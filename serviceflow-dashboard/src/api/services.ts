import { apiClient } from './client';
import type {
    DashboardSummary,
    ServiceDetail,
    CreateServiceRequest,
} from '../types';

export const servicesApi = {
    getDashboard: async (): Promise<DashboardSummary> => {
        const { data } = await apiClient.get('/services/dashboard');
        return data;
    },

    getService: async (id: number): Promise<ServiceDetail> => {
        const { data } = await apiClient.get(`/services/${id}`);
        return data;
    },

    createService: async (request: CreateServiceRequest): Promise<ServiceDetail> => {
        const { data } = await apiClient.post('/services', request);
        return data;
    },

    updateService: async (id: number, request: CreateServiceRequest): Promise<void> => {
        await apiClient.put(`/services/${id}`, request);
    },

    deleteService: async (id: number): Promise<void> => {
        await apiClient.delete(`/services/${id}`);
    },
};