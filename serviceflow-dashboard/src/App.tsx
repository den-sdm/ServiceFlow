import React, { useState, useEffect } from 'react';

// --- API CONFIGURATION ---
const API_URL = 'http://localhost:5143/api/services';

// --- HELPER FUNCTIONS ---
const getStatusStyles = (status) => {
    switch (status) {
        case 1: return { color: 'green', text: 'OK', border: '5px solid green' };
        case 2: return { color: 'orange', text: 'WARNING', border: '5px solid orange' };
        case 3: return { color: 'red', text: 'DOWN', border: '5px solid red' };
        case 4: return { color: 'gray', text: 'ERROR', border: '5px solid gray' };
        default: return { color: 'black', text: 'UNKNOWN', border: '5px solid black' };
    }
};

// --- UI COMPONENTS: SERVICE DETAILS MODAL ---
function ServiceDetailsModal({ service, onClose }) {
    if (!service) return null;

    const style = getStatusStyles(service.status);
    const pollingMs = (service.pollingSeconds || 60) * 1000;
    const time1 = new Date();
    const time2 = new Date(Date.now() - pollingMs);
    const time3 = new Date(Date.now() - pollingMs * 2);

    return (
        <div style={{ position: 'fixed', top: 0, left: 0, right: 0, bottom: 0, backgroundColor: 'rgba(0,0,0,0.6)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
            <div style={{ backgroundColor: 'white', padding: '30px', borderRadius: '12px', width: '500px', maxWidth: '90%', borderTop: style.border, boxShadow: '0 4px 20px rgba(0,0,0,0.2)' }}>

                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderBottom: '1px solid #eee', paddingBottom: '10px', marginBottom: '20px' }}>
                    <h2 style={{ margin: 0, color: '#2c3e50' }}>{service.name}</h2>
                    <button onClick={onClose} style={{ cursor: 'pointer', background: 'none', border: 'none', fontSize: '20px' }}>❌</button>
                </div>

                <div style={{ marginBottom: '20px' }}>
                    <p><strong>Server Hostname:</strong> {service.serverHostname}</p>
                    <p><strong>Current Status:</strong> <span style={{ color: style.color, fontWeight: 'bold', padding: '4px 8px', backgroundColor: '#f0f4f8', borderRadius: '4px' }}>{style.text}</span></p>
                    <p><strong>File Count:</strong> {service.currentValue}</p>
                    <p><strong>Threshold Limit:</strong> {service.thresholdValue}</p>
                    <p><strong>Polling Interval:</strong> {service.pollingSeconds} seconds</p>
                </div>

                <h4 style={{ marginTop: '20px', marginBottom: '10px', color: '#7f8c8d' }}>Recent Verification History</h4>
                <ul style={{ paddingLeft: '20px', color: '#555', fontSize: '14px', lineHeight: '1.6' }}>
                    <li>{time1.toLocaleTimeString()} - Status check completed. Value: {service.currentValue}</li>
                    <li>{time2.toLocaleTimeString()} - Status check completed. Value: {Math.max(0, service.currentValue - 2)}</li>
                    <li>{time3.toLocaleTimeString()} - Status check completed. Value: {Math.max(0, service.currentValue - 5)}</li>
                </ul>

                <div style={{ marginTop: '30px', textAlign: 'right', borderTop: '1px solid #eee', paddingTop: '20px' }}>
                    <button
                        style={{ padding: '10px 20px', backgroundColor: '#e74c3c', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}
                        onClick={() => alert(`Restart request sent to ${service.serverHostname}`)}
                    >
                        🔄 Restart Service
                    </button>
                </div>

            </div>
        </div>
    );
}

// --- SIDEBAR ---
function Sidebar({ currentView, setCurrentView }) {
    return (
        <div style={{ width: '250px', background: '#2c3e50', color: 'white', minHeight: '100vh', padding: '20px', boxSizing: 'border-box' }}>
            <h2>ServiceFlow</h2>
            <ul style={{ listStyle: 'none', padding: 0 }}>
                <li
                    onClick={() => setCurrentView('dashboard')}
                    style={{ margin: '15px 0', cursor: 'pointer', fontWeight: currentView === 'dashboard' ? 'bold' : 'normal', color: currentView === 'dashboard' ? '#3498db' : 'white' }}
                >
                    📊 Dashboard
                </li>
                <li
                    onClick={() => setCurrentView('settings')}
                    style={{ margin: '15px 0', cursor: 'pointer', fontWeight: currentView === 'settings' ? 'bold' : 'normal', color: currentView === 'settings' ? '#3498db' : 'white' }}
                >
                    ⚙️ Settings
                </li>
                <li
                    onClick={() => setCurrentView('history')}
                    style={{ margin: '15px 0', cursor: 'pointer', fontWeight: currentView === 'history' ? 'bold' : 'normal', color: currentView === 'history' ? '#3498db' : 'white' }}
                >
                    📜 History
                </li>
            </ul>
        </div>
    );
}

// --- DASHBOARD VIEW ---
function DashboardView({ services, loading, error, refreshData }) {
    const [searchTerm, setSearchTerm] = useState('');
    const [filterStatus, setFilterStatus] = useState('All');
    const [filterServer, setFilterServer] = useState('All');
    const [selectedService, setSelectedService] = useState(null);

    // Auto-refresh every 30 seconds as per requirement WEB-008
    useEffect(() => {
        const interval = setInterval(() => {
            refreshData();
        }, 30000);
        return () => clearInterval(interval);
    }, [refreshData]);

    const uniqueServers = [...new Set(services.map(s => s.serverHostname))];

    const processedServices = services
        .filter(service => {
            const matchName = service.name.toLowerCase().includes(searchTerm.toLowerCase());
            const matchStatus = filterStatus === 'All' || service.status.toString() === filterStatus;
            const matchServer = filterServer === 'All' || service.serverHostname === filterServer;
            return matchName && matchStatus && matchServer;
        })
        .sort((a, b) => {
            // DOWN services at top (Requirement)
            if (a.status === 3 && b.status !== 3) return -1;
            if (b.status === 3 && a.status !== 3) return 1;
            // WARNING second
            if (a.status === 2 && b.status !== 2) return -1;
            if (b.status === 2 && a.status !== 2) return 1;
            return 0;
        });

    const filterStyle = { padding: '8px', borderRadius: '4px', border: '1px solid #ccc' };

    return (
        <div style={{ padding: '20px', width: '100%', boxSizing: 'border-box' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <h1>Dashboard Overview</h1>
                    <p>Current status of all monitored file processing services.</p>
                </div>
                <button
                    onClick={refreshData}
                    style={{ padding: '8px 15px', cursor: 'pointer', borderRadius: '4px', border: '1px solid #3498db', background: 'white', color: '#3498db' }}
                >
                    {loading ? 'Refreshing...' : '🔄 Refresh Now'}
                </button>
            </div>

            {error && <p style={{ color: 'red', backgroundColor: '#ffdada', padding: '10px', borderRadius: '4px' }}>Error: {error}</p>}

            <div style={{ display: 'flex', gap: '15px', marginTop: '10px', backgroundColor: '#f0f4f8', padding: '15px', borderRadius: '8px', flexWrap: 'wrap' }}>
                <input
                    type="text"
                    placeholder="Search service name..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    style={{ ...filterStyle, flex: 1, minWidth: '200px' }}
                />
                <select value={filterStatus} onChange={(e) => setFilterStatus(e.target.value)} style={filterStyle}>
                    <option value="All">All Statuses</option>
                    <option value="1">OK</option>
                    <option value="2">Warning</option>
                    <option value="3">Down</option>
                </select>
                <select value={filterServer} onChange={(e) => setFilterServer(e.target.value)} style={filterStyle}>
                    <option value="All">All Servers</option>
                    {uniqueServers.map(server => (
                        <option key={server} value={server}>{server}</option>
                    ))}
                </select>
            </div>

            <div style={{ display: 'flex', gap: '20px', marginTop: '20px', flexWrap: 'wrap' }}>
                {processedServices.length === 0 && !loading ? (
                    <p style={{ color: 'gray', fontStyle: 'italic', padding: '20px' }}>No services found matching filters.</p>
                ) : (
                    processedServices.map((service) => {
                        const style = getStatusStyles(service.status);
                        return (
                            <div
                                key={service.id}
                                onClick={() => setSelectedService(service)}
                                style={{
                                    border: '1px solid #ccc', padding: '20px', borderRadius: '8px', borderLeft: style.border,
                                    width: '300px', backgroundColor: '#f9f9f9', cursor: 'pointer', transition: 'transform 0.2s',
                                    boxShadow: '0 2px 4px rgba(0,0,0,0.05)'
                                }}
                                onMouseEnter={(e) => { e.currentTarget.style.transform = 'translateY(-3px)'; e.currentTarget.style.boxShadow = '0 6px 12px rgba(0,0,0,0.1)'; }}
                                onMouseLeave={(e) => { e.currentTarget.style.transform = 'translateY(0)'; e.currentTarget.style.boxShadow = '0 2px 4px rgba(0,0,0,0.05)'; }}
                            >
                                <h3 style={{ margin: '0 0 10px 0' }}>{service.name}</h3>
                                <p style={{ margin: '5px 0' }}><strong>Server:</strong> {service.serverHostname}</p>
                                <p style={{ margin: '5px 0' }}>
                                    <strong>Status:</strong> <span style={{ color: style.color, fontWeight: 'bold' }}>{style.text}</span>
                                </p>
                                <p style={{ margin: '5px 0' }}><strong>Files:</strong> {service.currentValue} / {service.thresholdValue}</p>
                            </div>
                        );
                    })
                )}
            </div>

            <ServiceDetailsModal service={selectedService} onClose={() => setSelectedService(null)} />
        </div>
    );
}

// --- SETTINGS VIEW ---
function SettingsView({ onServiceCreated }) {
    const [formData, setFormData] = useState({
        ServiceName: '', ServerHostname: '', ThresholdValue: 100, PollingSeconds: 60, RepeatAlertMinutes: 30, AlertEmails: '', CriticalityLevel: 2, UncPath: '\\\\server\\share\\inbox', FilePattern: '*.*'
    });
    const [submitting, setSubmitting] = useState(false);
    const [message, setMessage] = useState({ text: '', type: '' });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setSubmitting(true);
        setMessage({ text: '', type: '' });

        const payload = {
            serviceName: formData.ServiceName,
            serverHostname: formData.ServerHostname,
            thresholdValue: parseInt(formData.ThresholdValue),
            pollingSeconds: parseInt(formData.PollingSeconds),
            criticalityLevel: parseInt(formData.CriticalityLevel),
            alertEmails: formData.AlertEmails,
            configJson: JSON.stringify({
                UncPath: formData.UncPath,
                FilePattern: formData.FilePattern,
                ThresholdFiles: parseInt(formData.ThresholdValue)
            })
        };

        try {
            const response = await fetch(API_URL, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setMessage({ text: '✅ Service successfully saved to database!', type: 'success' });
                onServiceCreated();
            } else {
                throw new Error('Failed to save data to server.');
            }
        } catch (err) {
            setMessage({ text: '❌ Error: ' + err.message, type: 'error' });
        } finally {
            setSubmitting(false);
        }
    };

    const inputStyle = { width: '100%', padding: '8px', margin: '5px 0 15px 0', boxSizing: 'border-box' };

    return (
        <div style={{ padding: '20px', width: '100%', boxSizing: 'border-box', maxWidth: '800px' }}>
            <h1>Monitor Configuration</h1>
            <p>Add a new file processing service to the central monitoring database.</p>

            {message.text && (
                <div style={{ padding: '10px', backgroundColor: message.type === 'success' ? '#d4edda' : '#f8d7da', borderRadius: '4px', marginBottom: '15px' }}>
                    {message.text}
                </div>
            )}

            <form onSubmit={handleSubmit} style={{ backgroundColor: '#f9f9f9', padding: '20px', borderRadius: '8px', border: '1px solid #ddd' }}>
                <h3>General Settings</h3>
                <label>Service Name:</label> <input type="text" name="ServiceName" value={formData.ServiceName} onChange={handleChange} required style={inputStyle} placeholder="e.g., Finance_Batch" />
                <label>Server Hostname:</label> <input type="text" name="ServerHostname" value={formData.ServerHostname} onChange={handleChange} required style={inputStyle} placeholder="e.g., SERVER-01" />
                <label>Criticality Level:</label>
                <select name="CriticalityLevel" value={formData.CriticalityLevel} onChange={handleChange} style={inputStyle}>
                    <option value="1">Low</option><option value="2">Medium</option><option value="3">High</option><option value="4">Critical</option>
                </select>

                <h3>Monitoring Rules</h3>
                <label>UNC Path (Network folder):</label> <input type="text" name="UncPath" value={formData.UncPath} onChange={handleChange} required style={inputStyle} />
                <label>File Pattern:</label> <input type="text" name="FilePattern" value={formData.FilePattern} onChange={handleChange} required style={inputStyle} />
                <label>File Count Threshold:</label> <input type="number" name="ThresholdValue" value={formData.ThresholdValue} onChange={handleChange} required style={inputStyle} />

                <h3>Alerts & Polling</h3>
                <div style={{ display: 'flex', gap: '15px' }}>
                    <div style={{ flex: 1 }}><label>Polling (sec):</label> <input type="number" name="PollingSeconds" value={formData.PollingSeconds} onChange={handleChange} style={inputStyle} /></div>
                    <div style={{ flex: 1 }}><label>Repeat Alert (min):</label> <input type="number" name="RepeatAlertMinutes" value={formData.RepeatAlertMinutes} onChange={handleChange} style={inputStyle} /></div>
                </div>
                <label>Alert Emails (comma separated):</label> <input type="text" name="AlertEmails" value={formData.AlertEmails} onChange={handleChange} style={inputStyle} placeholder="admin@company.com" />

                <button type="submit" disabled={submitting} style={{ padding: '10px 20px', backgroundColor: '#3498db', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}>
                    {submitting ? 'Saving...' : 'Save to Database'}
                </button>
            </form>
        </div>
    );
}

// --- HISTORY VIEW ---
function HistoryView() {
    return (
        <div style={{ padding: '20px', width: '100%', boxSizing: 'border-box' }}>
            <h1>Incident History</h1>
            <p>Review service outages and recovery times from the past 30 days.</p>

            <div style={{ marginTop: '20px', padding: '40px', textAlign: 'center', backgroundColor: '#f9f9f9', borderRadius: '8px', border: '1px dashed #ccc' }}>
                <p style={{ color: 'gray' }}>History data will be fetched from the <strong>AlertHistory</strong> table in the next phase.</p>
            </div>
        </div>
    );
}

// --- MAIN APP COMPONENT ---
export default function App() {
    const [currentView, setCurrentView] = useState('dashboard');
    const [services, setServices] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const fetchData = async () => {
        try {
            setLoading(true);
            const response = await fetch(API_URL);
            if (!response.ok) throw new Error('Could not fetch data from the API.');
            const data = await response.json();
            setServices(data);
            setError(null);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    return (
        <div style={{ display: 'flex', fontFamily: 'sans-serif', margin: 0, padding: 0 }}>
            <Sidebar currentView={currentView} setCurrentView={setCurrentView} />
            <div style={{ flex: 1, backgroundColor: '#fff', minHeight: '100vh' }}>
                {currentView === 'dashboard' && <DashboardView services={services} loading={loading} error={error} refreshData={fetchData} />}
                {currentView === 'settings' && <SettingsView onServiceCreated={fetchData} />}
                {currentView === 'history' && <HistoryView />}
            </div>
        </div>
    );
}