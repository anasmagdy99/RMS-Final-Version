import React from 'react';
import { useNavigate } from 'react-router-dom';
import { LogOut, User, Hash, Building2, Shield, Users, Key, Network } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useNotification } from '../context/NotificationContext';
import Card from '../components/Card';
import './Profile.css';

const Profile = () => {
    const { user, logout } = useAuth();
    const { showConfirm } = useNotification();
    const navigate = useNavigate();

    const handleLogout = async () => {
        const confirmed = await showConfirm('Are you sure you want to logout?');
        if (confirmed) {
            logout();
            navigate('/login');
        }
    };

    return (
        <div className="profile-page">
            <div className="page-header">
                <div>
                    <h1>Profile</h1>
                    <p className="page-subtitle">View your account information</p>
                </div>
                <button className="btn btn-danger" onClick={handleLogout}>
                    <LogOut size={18} style={{ marginRight: '0.5rem' }} />
                    Logout
                </button>
            </div>

            <div className="profile-grid">
                <Card title="Personal Information" className="profile-card">
                    <div className="profile-avatar-section">
                        <div className="profile-avatar-large">
                            {user?.name?.charAt(0) || 'U'}
                        </div>
                        <div className="profile-name-section">
                            <h2 className="profile-name">{user?.name}</h2>
                            <p className="profile-code">Employee #{user?.code}</p>
                        </div>
                    </div>

                    <div className="profile-details">
                        <div className="profile-detail-item">
                            <span className="profile-detail-label">Employee Code</span>
                            <span className="profile-detail-value">{user?.code}</span>
                        </div>
                        <div className="profile-detail-item">
                            <span className="profile-detail-label">Full Name</span>
                            <span className="profile-detail-value">{user?.name}</span>
                        </div>
                        <div className="profile-detail-item">
                            <span className="profile-detail-label">Department</span>
                            <span className="profile-detail-value">{user?.departmentName}</span>
                        </div>
                        <div className="profile-detail-item">
                            <span className="profile-detail-label">Role</span>
                            <span className="badge badge-role">{user?.role}</span>
                        </div>
                        {user?.isManager && (
                            <div className="profile-detail-item">
                                <span className="profile-detail-label">Manager Status</span>
                                <span className="badge badge-manager">Manager</span>
                            </div>
                        )}
                    </div>
                </Card>

                <Card title="System Information" className="profile-card">
                    <div className="profile-details">
                        <div className="profile-detail-item">
                            <span className="profile-detail-label">User ID</span>
                            <span className="profile-detail-value">{user?.id}</span>
                        </div>
                        <div className="profile-detail-item">
                            <span className="profile-detail-label">Department ID</span>
                            <span className="profile-detail-value">{user?.departmentID}</span>
                        </div>
                        {user?.managerId && (
                            <div className="profile-detail-item">
                                <span className="profile-detail-label">Reports To</span>
                                <span className="profile-detail-value">Manager ID: {user?.managerId}</span>
                            </div>
                        )}
                    </div>
                </Card>
            </div>
        </div>
    );
};

export default Profile;
