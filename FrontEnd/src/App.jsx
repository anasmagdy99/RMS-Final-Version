import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { NotificationProvider } from './context/NotificationContext';
import ProtectedRoute from './components/ProtectedRoute';
import BlockBoardRoute from './components/BlockBoardRoute';
import Sidebar from './components/Sidebar';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import MyRequests from './pages/MyRequests';
import TeamRequests from './pages/TeamRequests';
import AllRequests from './pages/AllRequests';
import Employees from './pages/Employees';
import LeaveBalance from './pages/LeaveBalance';
import HRRequests from './pages/HRRequests';
import History from './pages/History';
import Profile from './pages/Profile';
import './App.css';

function App() {
  return (
    <AuthProvider>
      <NotificationProvider>
        <Router>
          <Routes>
            {/* Public Route */}
            <Route path="/login" element={<Login />} />

            {/* Protected Routes */}
            <Route
              path="/*"
              element={
                <ProtectedRoute>
                  <AppLayout />
                </ProtectedRoute>
              }
            />
          </Routes>
        </Router>
      </NotificationProvider>
    </AuthProvider>
  );
}

// Layout component with sidebar
function AppLayout() {
  return (
    <div className="app-layout">
      <Sidebar />
      <main className="main-content">
        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/my-requests" element={<MyRequests />} />
          <Route path="/leave-balance" element={
            <BlockBoardRoute>
              <LeaveBalance />
            </BlockBoardRoute>
          } />
          <Route path="/profile" element={<Profile />} />

          {/* Manager Routes */}
          <Route
            path="/team-requests"
            element={
              <ProtectedRoute>
                <TeamRequests />
              </ProtectedRoute>
            }
          />

          {/* HR Routes */}
          <Route
            path="/all-requests"
            element={
              <ProtectedRoute requiredRole="HR">
                <AllRequests />
              </ProtectedRoute>
            }
          />
          <Route
            path="/employees"
            element={
              <ProtectedRoute requiredRole="HR">
                <Employees />
              </ProtectedRoute>
            }
          />

          {/* Board Routes */}
          <Route
            path="/history"
            element={
              <ProtectedRoute requiredRole="Board">
                <History />
              </ProtectedRoute>
            }
          />
          <Route
            path="/hr-requests"
            element={
              <ProtectedRoute requiredRole="Board">
                <HRRequests />
              </ProtectedRoute>
            }
          />

          {/* 404 */}
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;
