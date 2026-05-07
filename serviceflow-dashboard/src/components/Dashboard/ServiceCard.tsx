import React from 'react';
import { Server, Clock, TrendingUp } from 'lucide-react';
import StatusBadge from './StatusBadge';
import type { ServiceStatus } from '../../types';

interface ServiceCardProps {
    service: ServiceStatus;
    onClick: () => void;
}

const ServiceCard: React.FC<ServiceCardProps> = ({ service, onClick }) => {
    return (
        <div
            onClick={onClick}
            className={`p-6 rounded-lg border-2 cursor-pointer transition-all hover:shadow-lg ${service.isDown ? 'border-red-300 bg-red-50' : 'border-gray-200 bg-white'
                }`}
        >
            <div className="flex items-start justify-between mb-4">
                <div className="flex-1">
                    <h3 className="text-lg font-semibold text-gray-900">{service.friendlyName}</h3>
                    <p className="text-sm text-gray-600">{service.serviceName}</p>
                </div>
                <StatusBadge isDown={service.isDown} criticalityLevel={service.criticalityLevel} />
            </div>

            <div className="space-y-2">
                <div className="flex items-center text-sm text-gray-600">
                    <Server className="w-4 h-4 mr-2" />
                    {service.hostname}
                </div>

                <div className="flex items-center text-sm text-gray-600">
                    <TrendingUp className="w-4 h-4 mr-2" />
                    Value: {service.currentValue} / {service.thresholdValue}
                </div>

                <div className="flex items-center text-sm text-gray-600">
                    <Clock className="w-4 h-4 mr-2" />
                    Last check: {new Date(service.lastCheckTime).toLocaleString()}
                </div>

                {service.isDown && service.timeSinceDown && (
                    <div className="mt-3 p-2 bg-red-100 rounded">
                        <p className="text-sm font-medium text-red-800">
                            Down for: {service.timeSinceDown}
                        </p>
                        {service.errorMessage && (
                            <p className="text-xs text-red-600 mt-1">{service.errorMessage}</p>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ServiceCard;