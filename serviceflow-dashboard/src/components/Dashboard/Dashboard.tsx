import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { servicesApi } from '../../api/services';
import ServiceCard from './ServiceCard';
import { AlertTriangle, CheckCircle2, Server as ServerIcon } from 'lucide-react';

const Dashboard: React.FC = () => {
    const { data: dashboard, isLoading, error, refetch } = useQuery({
        queryKey: ['dashboard'],
        queryFn: servicesApi.getDashboard,
        refetchInterval: 10000, // Refresh every 10 seconds
    });

    if (isLoading) {
        return (
            <div className="flex items-center justify-center h-96">
                <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
                    <p className="mt-4 text-gray-600">Loading dashboard...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="bg-red-50 border border-red-200 rounded-lg p-6">
                <h3 className="text-lg font-semibold text-red-800 mb-2">Error Loading Dashboard</h3>
                <p className="text-red-600">{(error as Error).message}</p>
                <button
                    onClick={() => refetch()}
                    className="mt-4 px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
                >
                    Retry
                </button>
            </div>
        );
    }

    if (!dashboard) {
        return <div>No data available</div>;
    }

    return (
        <div className="space-y-6">
            {/* Summary Cards */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div className="bg-white rounded-lg shadow p-6">
                    <div className="flex items-center">
                        <ServerIcon className="w-8 h-8 text-blue-600 mr-3" />
                        <div>
                            <p className="text-sm text-gray-600">Total Services</p>
                            <p className="text-2xl font-bold">{dashboard.totalServices}</p>
                        </div>
                    </div>
                </div>

                <div className="bg-green-50 rounded-lg shadow p-6">
                    <div className="flex items-center">
                        <CheckCircle2 className="w-8 h-8 text-green-600 mr-3" />
                        <div>
                            <p className="text-sm text-gray-600">Services Up</p>
                            <p className="text-2xl font-bold text-green-700">{dashboard.servicesUp}</p>
                        </div>
                    </div>
                </div>

                <div className="bg-red-50 rounded-lg shadow p-6">
                    <div className="flex items-center">
                        <AlertTriangle className="w-8 h-8 text-red-600 mr-3" />
                        <div>
                            <p className="text-sm text-gray-600">Services Down</p>
                            <p className="text-2xl font-bold text-red-700">{dashboard.servicesDown}</p>
                        </div>
                    </div>
                </div>

                <div className="bg-orange-50 rounded-lg shadow p-6">
                    <div className="flex items-center">
                        <AlertTriangle className="w-8 h-8 text-orange-600 mr-3" />
                        <div>
                            <p className="text-sm text-gray-600">Critical Down</p>
                            <p className="text-2xl font-bold text-orange-700">{dashboard.criticalServicesDown}</p>
                        </div>
                    </div>
                </div>
            </div>

            {/* Down Services Section */}
            {dashboard.downServices.length > 0 && (
                <div>
                    <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                        <AlertTriangle className="w-6 h-6 text-red-600 mr-2" />
                        Services Down ({dashboard.downServices.length})
                    </h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {dashboard.downServices.map((service) => (
                            <ServiceCard key={service.serviceID} service={service} onClick={() => { }} />
                        ))}
                    </div>
                </div>
            )}

            {/* All Services Section */}
            <div>
                <h2 className="text-xl font-bold text-gray-900 mb-4">
                    All Services ({dashboard.allServices.length})
                </h2>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    {dashboard.allServices.map((service) => (
                        <ServiceCard key={service.serviceID} service={service} onClick={() => { }} />
                    ))}
                </div>
            </div>
        </div>
    );
};

export default Dashboard;