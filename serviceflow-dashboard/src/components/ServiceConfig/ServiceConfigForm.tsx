import React, { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { servicesApi } from '../../api/services';
import type { CreateServiceRequest, FileProcessingConfig } from '../../types';

const ServiceConfigForm: React.FC = () => {
    const queryClient = useQueryClient();

    const [formData, setFormData] = useState<CreateServiceRequest>({
        hostname: '',
        serviceName: '',
        friendlyName: '',
        description: '',
        criticalityLevel: 2,
        verifications: [],
        distributionEmails: [],
    });

    const [fileConfig, setFileConfig] = useState<FileProcessingConfig>({
        folderPath: '',
        filePattern: '*.xml',
        checkType: 'FileCount',
    });

    const [pollingInterval, setPollingInterval] = useState(30);
    const [threshold, setThreshold] = useState(10);
    const [alertRepeat, setAlertRepeat] = useState(15);
    const [emailInput, setEmailInput] = useState('');

    const createMutation = useMutation({
        mutationFn: servicesApi.createService,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['dashboard'] });
            alert('Service created successfully!');
            // Reset form
            setFormData({
                hostname: '',
                serviceName: '',
                friendlyName: '',
                description: '',
                criticalityLevel: 2,
                verifications: [],
                distributionEmails: [],
            });
            setFileConfig({ folderPath: '', filePattern: '*.xml', checkType: 'FileCount' });
        },
        onError: (error) => {
            alert(`Error creating service: ${(error as Error).message}`);
        },
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        const verification = {
            verificationType: 'FileProcessing',
            configurationJSON: JSON.stringify(fileConfig),
            pollingIntervalSeconds: pollingInterval,
            thresholdValue: threshold,
            alertRepeatMinutes: alertRepeat,
        };

        createMutation.mutate({
            ...formData,
            verifications: [verification],
        });
    };

    const addEmail = () => {
        if (emailInput && !formData.distributionEmails.includes(emailInput)) {
            setFormData({
                ...formData,
                distributionEmails: [...formData.distributionEmails, emailInput],
            });
            setEmailInput('');
        }
    };

    const removeEmail = (email: string) => {
        setFormData({
            ...formData,
            distributionEmails: formData.distributionEmails.filter(e => e !== email),
        });
    };

    return (
        <div className="max-w-4xl mx-auto p-6">
            <h2 className="text-2xl font-bold text-gray-900 mb-6">Configure New Service</h2>

            <form onSubmit={handleSubmit} className="space-y-6 bg-white shadow rounded-lg p-6">
                {/* Basic Information */}
                <div>
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">Basic Information</h3>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Hostname
                            </label>
                            <input
                                type="text"
                                required
                                value={formData.hostname}
                                onChange={(e) => setFormData({ ...formData, hostname: e.target.value })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="SERVER01"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Service Name
                            </label>
                            <input
                                type="text"
                                required
                                value={formData.serviceName}
                                onChange={(e) => setFormData({ ...formData, serviceName: e.target.value })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="InvoiceProcessorService"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Friendly Name
                            </label>
                            <input
                                type="text"
                                required
                                value={formData.friendlyName}
                                onChange={(e) => setFormData({ ...formData, friendlyName: e.target.value })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="Invoice File Processor"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Criticality Level
                            </label>
                            <select
                                value={formData.criticalityLevel}
                                onChange={(e) => setFormData({ ...formData, criticalityLevel: Number(e.target.value) })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            >
                                <option value={1}>Critical</option>
                                <option value={2}>High</option>
                                <option value={3}>Medium</option>
                                <option value={4}>Low</option>
                            </select>
                        </div>
                    </div>

                    <div className="mt-4">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Description
                        </label>
                        <textarea
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            rows={3}
                            placeholder="Description of what this service does..."
                        />
                    </div>
                </div>

                {/* File Processing Configuration */}
                <div>
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">File Processing Configuration</h3>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Folder Path
                            </label>
                            <input
                                type="text"
                                required
                                value={fileConfig.folderPath}
                                onChange={(e) => setFileConfig({ ...fileConfig, folderPath: e.target.value })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="C:\ProcessingQueue\Invoices"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                File Pattern
                            </label>
                            <input
                                type="text"
                                required
                                value={fileConfig.filePattern}
                                onChange={(e) => setFileConfig({ ...fileConfig, filePattern: e.target.value })}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="IT*.xml"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Polling Interval (seconds)
                            </label>
                            <input
                                type="number"
                                required
                                min={10}
                                value={pollingInterval}
                                onChange={(e) => setPollingInterval(Number(e.target.value))}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Threshold (file count)
                            </label>
                            <input
                                type="number"
                                required
                                min={0}
                                value={threshold}
                                onChange={(e) => setThreshold(Number(e.target.value))}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Alert Repeat (minutes)
                            </label>
                            <input
                                type="number"
                                required
                                min={5}
                                value={alertRepeat}
                                onChange={(e) => setAlertRepeat(Number(e.target.value))}
                                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                    </div>
                </div>

                {/* Distribution List */}
                <div>
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">Alert Distribution List</h3>

                    <div className="flex gap-2 mb-3">
                        <input
                            type="email"
                            value={emailInput}
                            onChange={(e) => setEmailInput(e.target.value)}
                            onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), addEmail())}
                            className="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="email@example.com"
                        />
                        <button
                            type="button"
                            onClick={addEmail}
                            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
                        >
                            Add
                        </button>
                    </div>

                    <div className="space-y-2">
                        {formData.distributionEmails.map((email) => (
                            <div key={email} className="flex items-center justify-between bg-gray-50 px-3 py-2 rounded">
                                <span className="text-sm">{email}</span>
                                <button
                                    type="button"
                                    onClick={() => removeEmail(email)}
                                    className="text-red-600 hover:text-red-800 text-sm"
                                >
                                    Remove
                                </button>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Submit Button */}
                <div className="flex justify-end">
                    <button
                        type="submit"
                        disabled={createMutation.isPending}
                        className="px-6 py-3 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
                    >
                        {createMutation.isPending ? 'Creating...' : 'Create Service'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default ServiceConfigForm;