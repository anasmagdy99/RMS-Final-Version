-- RMS Database Setup Script
-- Run this script in SQL Server Management Studio or Azure Data Studio

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RMS')
BEGIN
    CREATE DATABASE RMS;
END
GO

USE RMS;
GO

-- Create tables (if they don't exist)

-- EmployeeLevels Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeLevels')
BEGIN
    CREATE TABLE EmployeeLevels (
        Id INT PRIMARY KEY,
        LevelName NVARCHAR(50) NOT NULL,
        AnnualLeaveEntitlement INT NOT NULL
    );
END
GO

-- Statuses Table (for both Transaction Statuses and Departments)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Statuses')
BEGIN
    CREATE TABLE Statuses (
        Id INT PRIMARY KEY IDENTITY(1,1),
        StatusName NVARCHAR(100) NOT NULL,
        StatusType NVARCHAR(50) NOT NULL -- 'TransactionStatus' or 'Department'
    );
END
GO

-- TransactionTypes Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TransactionTypes')
BEGIN
    CREATE TABLE TransactionTypes (
        Id INT PRIMARY KEY,
        TransactionTypeName NVARCHAR(100) NOT NULL,
        Unit DECIMAL(5,2) NOT NULL,
        Sign INT NOT NULL
    );
END
GO

-- Employees Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE Employees (
        Id INT PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Password NVARCHAR(200) NOT NULL,
        DepartmentID INT NOT NULL,
        EmployeeLevelId INT NOT NULL,
        ManagerId INT NULL,
        EmployeeRole INT NOT NULL DEFAULT 0, -- 0 = Employee, 1 = Manager
        DateOfEmployment DATETIME2 NOT NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (DepartmentID) REFERENCES Statuses(Id),
        FOREIGN KEY (EmployeeLevelId) REFERENCES EmployeeLevels(Id),
        FOREIGN KEY (ManagerId) REFERENCES Employees(Id)
    );
END
GO

-- Transactions Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions')
BEGIN
    CREATE TABLE Transactions (
        Id INT PRIMARY KEY,
        EmployeeId INT NOT NULL,
        TransactionTypesID INT NOT NULL,
        StartDate DATETIME2 NOT NULL,
        EndDate DATETIME2 NOT NULL,
        SubstituteEmployeeId INT NULL,
        LeaveRationale NVARCHAR(500),
        StatusID INT NOT NULL,
        ResponseMessage NVARCHAR(500),
        CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        ResponseDate DATETIME2 NULL,
        FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
        FOREIGN KEY (TransactionTypesID) REFERENCES TransactionTypes(Id),
        FOREIGN KEY (SubstituteEmployeeId) REFERENCES Employees(Id),
        FOREIGN KEY (StatusID) REFERENCES Statuses(Id)
    );
END
GO

-- Insert Employee Levels
IF NOT EXISTS (SELECT * FROM EmployeeLevels WHERE Id = 1)
BEGIN
    INSERT INTO EmployeeLevels (Id, LevelName, AnnualLeaveEntitlement) VALUES
    (1, 'A', 15),
    (2, 'B', 24);
END
GO

-- Insert Statuses (Transaction Statuses)
IF NOT EXISTS (SELECT * FROM Statuses WHERE StatusType = 'TransactionStatus')
BEGIN
    SET IDENTITY_INSERT Statuses ON;
    
    INSERT INTO Statuses (Id, StatusName, StatusType) VALUES
    (1, 'Pending', 'TransactionStatus'),
    (2, 'Pending HR', 'TransactionStatus'),
    (3, 'Approved by HR', 'TransactionStatus'),
    (4, 'Rejected by Manager', 'TransactionStatus'),
    (5, 'Rejected by HR', 'TransactionStatus'),
    (6, 'Cancelled by Employee', 'TransactionStatus');
    
    SET IDENTITY_INSERT Statuses OFF;
END
GO

-- Insert Departments
IF NOT EXISTS (SELECT * FROM Statuses WHERE StatusType = 'Department')
BEGIN
    SET IDENTITY_INSERT Statuses ON;
    
    INSERT INTO Statuses (Id, StatusName, StatusType) VALUES
    (7, 'Quality', 'Department'),
    (8, 'Production', 'Department'),
    (9, 'IT', 'Department'),
    (10, 'HR', 'Department'),
    (11, 'Board', 'Department');
    
    SET IDENTITY_INSERT Statuses OFF;
END
GO

-- Insert Transaction Types
IF NOT EXISTS (SELECT * FROM TransactionTypes WHERE Id = 1)
BEGIN
    INSERT INTO TransactionTypes (Id, TransactionTypeName, Unit, Sign) VALUES
    (1, 'Sick Leave', 1.0, 0),
    (2, 'Annual Leave', 1.0, -1),
    (3, 'Half Day', 0.5, -1),
    (4, 'Bonus Leave', 1.0, 1),
    (5, 'Unpaid Leave', 1.0, -1);
END
GO

-- Insert Test Employees
IF NOT EXISTS (SELECT * FROM Employees WHERE Code = '1980009')
BEGIN
    -- HR Manager
    INSERT INTO Employees (Id, Code, Name, Password, DepartmentID, EmployeeLevelId, ManagerId, EmployeeRole, DateOfEmployment, IsDeleted)
    VALUES (1, '1980009', 'Ayman Mohamed', 'Pass#123', 10, 2, NULL, 1, '2020-01-01', 0);
    
    -- Quality Employee 1
    INSERT INTO Employees (Id, Code, Name, Password, DepartmentID, EmployeeLevelId, ManagerId, EmployeeRole, DateOfEmployment, IsDeleted)
    VALUES (2, '1990027', 'Ahmed Hassan', 'Pass#123', 7, 1, 1, 0, '2021-06-15', 0);
    
    -- Quality Employee 2
    INSERT INTO Employees (Id, Code, Name, Password, DepartmentID, EmployeeLevelId, ManagerId, EmployeeRole, DateOfEmployment, IsDeleted)
    VALUES (3, '1990055', 'Sara Ali', 'Pass#123', 7, 1, 1, 0, '2022-03-20', 0);
    
    -- Production Manager
    INSERT INTO Employees (Id, Code, Name, Password, DepartmentID, EmployeeLevelId, ManagerId, EmployeeRole, DateOfEmployment, IsDeleted)
    VALUES (4, '1985012', 'Mohamed Salah', 'Pass#123', 8, 2, 1, 1, '2019-05-10', 0);
    
    -- Board Member
    INSERT INTO Employees (Id, Code, Name, Password, DepartmentID, EmployeeLevelId, ManagerId, EmployeeRole, DateOfEmployment, IsDeleted)
    VALUES (5, '1975001', 'Omar Khaled', 'Pass#123', 11, 2, NULL, 0, '2015-01-01', 0);
END
GO

-- Insert Sample Transactions
IF NOT EXISTS (SELECT * FROM Transactions WHERE Id = 1)
BEGIN
    INSERT INTO Transactions (Id, EmployeeId, TransactionTypesID, StartDate, EndDate, SubstituteEmployeeId, LeaveRationale, StatusID, ResponseMessage, CreationDate, ResponseDate)
    VALUES 
    (1, 2, 2, '2026-03-01', '2026-03-05', 3, 'Family vacation', 1, NULL, '2026-02-01', NULL),
    (2, 3, 2, '2026-04-10', '2026-04-15', 2, 'Personal matters', 3, 'Approved', '2026-02-02', '2026-02-03'),
    (3, 4, 2, '2026-05-01', '2026-05-03', NULL, 'Medical appointment', 2, NULL, '2026-02-01', NULL);
END
GO

PRINT 'Database setup completed successfully!';
PRINT 'Test credentials:';
PRINT '  HR Manager - Code: 1980009, Password: Pass#123';
PRINT '  Employee - Code: 1990027, Password: Pass#123';
PRINT '  Employee - Code: 1990055, Password: Pass#123';
GO
