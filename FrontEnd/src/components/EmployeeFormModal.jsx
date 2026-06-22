import React, { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import employeeService from '../services/employeeService';
import CustomSelect from './CustomSelect';
import CustomDatePicker from './CustomDatePicker';
import './EmployeeFormModal.css';

const EmployeeFormModal = ({ isOpen, onClose, onSubmit, employee, mode }) => {
    const [formData, setFormData] = useState({
        code: '',
        name: '',
        departmentID: '',
        managerID: '',
        dateOfEmployment: '',
        employeeLevelID: '',
        annualLeaveEntitlement: 15
    });

    const [errors, setErrors] = useState({});
    const [managers, setManagers] = useState([]);
    const [loadingManagers, setLoadingManagers] = useState(false);

    // Fetch managers when modal opens
    useEffect(() => {
        if (isOpen) {
            fetchManagers();
        }
    }, [isOpen]);

    useEffect(() => {
        if (employee && mode === 'edit') {
            setFormData({
                code: employee.code || '',
                name: employee.name || '',
                departmentID: employee.departmentID || '',
                managerID: employee.managerID || '',
                dateOfEmployment: employee.dateOfEmployment?.split('T')[0] || '',
                employeeLevelID: employee.employeeLevelID || '',
                annualLeaveEntitlement: employee.annualLeaveEntitlement || 15
            });
        } else if (mode === 'create') {
            setFormData({
                code: '',
                name: '',
                departmentID: '',
                managerID: '',
                dateOfEmployment: '',
                employeeLevelID: '',
                annualLeaveEntitlement: 15
            });
        }
    }, [employee, mode, isOpen]);

    // Auto-calculate Annual Leave Entitlement based on Employee Level
    useEffect(() => {
        if (formData.employeeLevelID) {
            const levelId = parseInt(formData.employeeLevelID);
            let entitlement = 15; // default

            if (levelId === 1) { // Level A
                entitlement = 15;
            } else if (levelId === 2) { // Level B
                entitlement = 20;
            }

            setFormData(prev => ({
                ...prev,
                annualLeaveEntitlement: entitlement
            }));
        }
    }, [formData.employeeLevelID]);

    const fetchManagers = async () => {
        try {
            setLoadingManagers(true);
            const data = await employeeService.getAll();
            console.log('All employees:', data);

            // Filter out Board (departmentID: 11) and HR (departmentID: 10)
            const filteredEmployees = data.filter(emp =>
                emp.departmentID !== 10 && emp.departmentID !== 11
            );
            console.log('Filtered employees (no Board/HR):', filteredEmployees);

            setManagers(filteredEmployees);
        } catch (error) {
            console.error('Error fetching managers:', error);
        } finally {
            setLoadingManagers(false);
        }
    };

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
        // Clear error for this field
        if (errors[name]) {
            setErrors(prev => ({ ...prev, [name]: '' }));
        }
    };

    const handleCustomChange = (name, value) => {
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        if (errors[name]) {
            setErrors(prev => ({ ...prev, [name]: '' }));
        }
    };

    const validate = () => {
        const newErrors = {};

        if (!formData.code.trim()) newErrors.code = 'Employee code is required';
        if (!formData.name.trim()) newErrors.name = 'Name is required';
        if (!formData.departmentID) newErrors.departmentID = 'Department is required';
        if (!formData.dateOfEmployment) newErrors.dateOfEmployment = 'Date of employment is required';
        if (!formData.employeeLevelID) newErrors.employeeLevelID = 'Employee level is required';
        if (!formData.managerID) newErrors.managerID = 'Manager is required';

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        if (validate()) {
            onSubmit(formData);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="employee-form-modal-overlay" onClick={onClose}>
            <div className="employee-form-modal" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>{mode === 'create' ? 'Add New Employee' : 'Edit Employee'}</h2>
                    <button className="modal-close-btn" onClick={onClose}>
                        <X size={24} />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="employee-form">
                    <div className="form-grid">
                        {/* Employee Code */}
                        <div className="form-group">
                            <label className="label">
                                Employee Code <span className="required">*</span>
                            </label>
                            <input
                                type="text"
                                name="code"
                                className={`input ${errors.code ? 'input-error' : ''}`}
                                value={formData.code}
                                onChange={handleChange}
                                disabled={mode === 'edit'}
                                placeholder="e.g., EMP001"
                            />
                            {errors.code && <span className="error-message">{errors.code}</span>}
                        </div>

                        {/* Name */}
                        <div className="form-group">
                            <label className="label">
                                Full Name <span className="required">*</span>
                            </label>
                            <input
                                type="text"
                                name="name"
                                className={`input ${errors.name ? 'input-error' : ''}`}
                                value={formData.name}
                                onChange={handleChange}
                                placeholder="e.g., Ahmed Mohamed"
                            />
                            {errors.name && <span className="error-message">{errors.name}</span>}
                        </div>

                        {/* Department */}
                        <div className="form-group">
                            <CustomSelect
                                label="Department *"
                                value={formData.departmentID}
                                onChange={(val) => handleCustomChange('departmentID', val)}
                                error={errors.departmentID}
                                options={[
                                    { value: '', label: 'Select Department' },
                                    { value: 7, label: 'Quality' },
                                    { value: 8, label: 'Marketing' },
                                    { value: 9, label: 'Finance' },
                                    { value: 10, label: 'HR' }
                                ]}
                            />
                            {errors.departmentID && <span className="error-message">{errors.departmentID}</span>}
                        </div>

                        {/* Date of Employment */}
                        <div className="form-group">
                            <CustomDatePicker
                                label="Date of Employment *"
                                value={formData.dateOfEmployment}
                                onChange={(val) => handleCustomChange('dateOfEmployment', val)}
                                error={errors.dateOfEmployment}
                            />
                            {errors.dateOfEmployment && <span className="error-message">{errors.dateOfEmployment}</span>}
                        </div>

                        {/* Manager */}
                        <div className="form-group">
                            <CustomSelect
                                label="Manager *"
                                value={formData.managerID}
                                onChange={(val) => handleCustomChange('managerID', val)}
                                error={errors.managerID}
                                options={[
                                    { value: '', label: 'Select Manager' },
                                    ...managers.map(m => ({ value: m.id, label: `${m.name} - ${m.departmentName || 'N/A'}` }))
                                ]}
                                disabled={loadingManagers}
                            />
                            {errors.managerID && <span className="error-message">{errors.managerID}</span>}
                            {loadingManagers && <span className="info-text">Loading managers...</span>}
                        </div>

                        {/* Employee Level */}
                        <div className="form-group">
                            <CustomSelect
                                label="Employee Level *"
                                value={formData.employeeLevelID}
                                onChange={(val) => handleCustomChange('employeeLevelID', val)}
                                error={errors.employeeLevelID}
                                options={[
                                    { value: '', label: 'Select Level' },
                                    { value: 1, label: 'A (15 days)' },
                                    { value: 2, label: 'B (20 days)' }
                                ]}
                            />
                            {errors.employeeLevelID && <span className="error-message">{errors.employeeLevelID}</span>}
                        </div>

                        {/* Annual Leave Entitlement (Auto-calculated, Read-only) */}
                        <div className="form-group">
                            <label className="label">Annual Leave Entitlement</label>
                            <input
                                type="number"
                                name="annualLeaveEntitlement"
                                className="input"
                                value={formData.annualLeaveEntitlement}
                                readOnly
                                disabled
                                style={{ background: '#f0f0f0', cursor: 'not-allowed' }}
                            />
                            <span className="info-text">Auto-calculated based on level</span>
                        </div>
                    </div>

                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" onClick={onClose}>
                            Cancel
                        </button>
                        <button type="submit" className="btn btn-primary">
                            {mode === 'create' ? 'Add Employee' : 'Save Changes'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default EmployeeFormModal;
