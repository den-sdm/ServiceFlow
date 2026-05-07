export interface Service {
    serviceID: number;
    serviceName: string;
    friendlyName: string;
    description?: string;
    hostname: string;
    criticalityLevel: number;
    criticalityLevelName: string;
    isActive: boolean;
}

export interface ServiceDetail extends Service {
    verifications: ServiceVerification[];
    distributionList: string[];
    currentStatus?: ServiceStatus;
}

export interface ServiceVerification {
    verificationID: number;
    verificationType: string;
    configurationJSON: string;
    pollingIntervalSeconds: number;
    thresholdValue: number;
    alertRepeatMinutes: number;
    isActive: boolean;
}

export interface ServiceStatus {
    serviceID: number;
    serviceName: string;
    friendlyName: string;
    hostname: string;
    criticalityLevel: number;
    isDown: boolean;
    currentValue: number;
    thresholdValue: number;
    lastCheckTime: string;
    downSince?: string;
    timeSinceDown?: string;
    errorMessage?: string;
}

export interface DashboardSummary {
    totalServices: number;
    servicesDown: number;
    servicesUp: number;
    criticalServicesDown: number;
    downServices: ServiceStatus[];
    allServices: ServiceStatus[];
}

export interface CreateServiceRequest {
    hostname: string;
    serviceName: string;
    friendlyName: string;
    description?: string;
    criticalityLevel: number;
    verifications: VerificationRequest[];
    distributionEmails: string[];
}

export interface VerificationRequest {
    verificationType: string;
    configurationJSON: string;
    pollingIntervalSeconds: number;
    thresholdValue: number;
    alertRepeatMinutes: number;
}

export interface FileProcessingConfig {
    folderPath: string;
    filePattern: string;
    checkType: string;
}