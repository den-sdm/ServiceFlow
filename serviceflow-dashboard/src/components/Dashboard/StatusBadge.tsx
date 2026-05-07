import React from 'react';
import { AlertCircle, CheckCircle } from 'lucide-react';

interface StatusBadgeProps {
    isDown: boolean;
    criticalityLevel: number;
}

const StatusBadge: React.FC<StatusBadgeProps> = ({ isDown, criticalityLevel }) => {
    if (!isDown) {
        return (
            <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800">
                <CheckCircle className="w-4 h-4 mr-1" />
                Up
            </span>
        );
    }

    const levelColors = {
        1: 'bg-red-100 text-red-800',
        2: 'bg-orange-100 text-orange-800',
        3: 'bg-yellow-100 text-yellow-800',
        4: 'bg-blue-100 text-blue-800',
    };

    const colorClass = levelColors[criticalityLevel as keyof typeof levelColors] || levelColors[2];

    return (
        <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${colorClass}`}>
            <AlertCircle className="w-4 h-4 mr-1" />
            Down
        </span>
    );
};

export default StatusBadge;