-- =====================
-- Team Tasks Dashboard - Database Setup Script
-- Author: Roimer
-- Description: Creates the TeamTasksSample database with all required tables,
--              sample data, and queries for the Team Tasks Dashboard application.
-- =====================

-- ++++++++++++++++++++++++++++++++++++++++++
-- SECTION 1: DATABASE AND SCHEMA CREATION
-- ++++++++++++++++++++++++++++++++++++++++

USE master;
GO

-- WARNING: This script recreate the database. Intended for local/dev only.

-- Drop database if exists (for clean setup)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'TeamTasksSample')
BEGIN
    ALTER DATABASE TeamTasksSample SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TeamTasksSample;
END
GO

-- Create database
CREATE DATABASE TeamTasksSample;
GO

USE TeamTasksSample;
GO

-- Create schema for better organization
CREATE SCHEMA TaskManagement;
GO

-- ++++++++++++++++++++++++
-- SECTION 2: TABLE´S CREATION
-- +++++++++++++++++++++++

-- Developers table
CREATE TABLE TaskManagement.Developers (
    DeveloperId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Indexes
    INDEX IX_Developers_Email NONCLUSTERED (Email),
    INDEX IX_Developers_IsActive NONCLUSTERED (IsActive)
);
GO

-- Projects table
CREATE TABLE TaskManagement.Projects (
    ProjectId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    ClientName NVARCHAR(200) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Planned',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Constraints
    CONSTRAINT CHK_Projects_Status CHECK (Status IN ('Planned', 'InProgress', 'Completed')),
    CONSTRAINT CHK_Projects_Dates CHECK (EndDate IS NULL OR EndDate >= StartDate),

    -- Indexes
    INDEX IX_Projects_Status NONCLUSTERED (Status)
);
GO

-- Tasks table
CREATE TABLE TaskManagement.Tasks (
    TaskId INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    Title NVARCHAR(300) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    AssigneeId INT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'ToDo',
    Priority NVARCHAR(10) NOT NULL DEFAULT 'Medium',
    EstimatedComplexity INT NOT NULL DEFAULT 3,
    DueDate DATE NULL,
    CompletionDate DATE NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Foreign Keys
    CONSTRAINT FK_Tasks_Projects FOREIGN KEY (ProjectId)
        REFERENCES TaskManagement.Projects(ProjectId) ON DELETE CASCADE,
    CONSTRAINT FK_Tasks_Developers FOREIGN KEY (AssigneeId)
        REFERENCES TaskManagement.Developers(DeveloperId) ON DELETE SET NULL,

    -- Constraints
    CONSTRAINT CHK_Tasks_Status CHECK (Status IN ('ToDo', 'InProgress', 'Blocked', 'Completed')),
    CONSTRAINT CHK_Tasks_Priority CHECK (Priority IN ('Low', 'Medium', 'High')),
    CONSTRAINT CHK_Tasks_Complexity CHECK (EstimatedComplexity BETWEEN 1 AND 5),

    -- Indexes
    INDEX IX_Tasks_ProjectId NONCLUSTERED (ProjectId),
    INDEX IX_Tasks_AssigneeId NONCLUSTERED (AssigneeId),
    INDEX IX_Tasks_Status NONCLUSTERED (Status),
    INDEX IX_Tasks_DueDate NONCLUSTERED (DueDate)
);
GO

-- +++++++++++++++++++++++++++++++++
-- SECTION 3: SAMPLE DATA INSERTION
-- +++++++++++++++++++++++++++++++

-- Insert 5 active developers
INSERT INTO TaskManagement.Developers (FirstName, LastName, Email, IsActive, CreatedAt)
VALUES
    ('Esteban', 'Ortiz', 'esteban.ortiz@nserio-dev.com', 1, DATEADD(MONTH, -6, GETUTCDATE())),
    ('Maria', 'Rodriguez', 'maria.rodriguez@nserio-dev.com', 1, DATEADD(MONTH, -5, GETUTCDATE())),
    ('Juan', 'Garcia', 'juan.garcia@nserio-dev.com', 1, DATEADD(MONTH, -4, GETUTCDATE())),
    ('Ana', 'Ceballos', 'ana.ceballos@nserio-dev.com', 1, DATEADD(MONTH, -3, GETUTCDATE())),
    ('Pedro', 'Sanchez', 'pedro.sanchez@nserio-dev.com', 1, DATEADD(MONTH, -2, GETUTCDATE()));
GO

-- Insert 3 projects
INSERT INTO TaskManagement.Projects (Name, ClientName, StartDate, EndDate, Status, CreatedAt)
VALUES
    ('E-Commerce Platform', 'TechRetail Corp', '2024-01-15', '2024-06-30', 'InProgress', DATEADD(MONTH, -5, GETUTCDATE())),
    ('Mobile Banking App', 'FinanceBank SA', '2024-03-01', '2024-09-30', 'InProgress', DATEADD(MONTH, -3, GETUTCDATE())),
    ('Healthcare Portal', 'MediCare Solutions', '2024-05-01', NULL, 'Planned', DATEADD(MONTH, -1, GETUTCDATE()));
GO

-- Insert 20 tasks distributed among projects and developers
-- Project 1: E-Commerce Platform (8 tasks)
INSERT INTO TaskManagement.Tasks (ProjectId, Title, Description, AssigneeId, Status, Priority, EstimatedComplexity, DueDate, CompletionDate, CreatedAt)
VALUES
    -- Completed tasks (for delay calculation)
    (1, 'Setup project architecture', 'Configure Clean Architecture with all layers', 1, 'Completed', 'High', 4, '2024-01-25', '2024-01-24', DATEADD(DAY, -90, GETUTCDATE())),
    (1, 'Implement user authentication', 'JWT-based authentication system', 1, 'Completed', 'High', 5, '2024-02-10', '2024-02-15', DATEADD(DAY, -85, GETUTCDATE())), -- 5 days late
    (1, 'Create product catalog API', 'CRUD operations for products', 2, 'Completed', 'Medium', 3, '2024-02-20', '2024-02-18', DATEADD(DAY, -80, GETUTCDATE())),
    (1, 'Shopping cart functionality', 'Add/remove items, calculate totals', 2, 'Completed', 'High', 4, '2024-03-01', '2024-03-05', DATEADD(DAY, -70, GETUTCDATE())), -- 4 days late
    -- Open tasks
    (1, 'Payment gateway integration', 'Integrate with Stripe and PayPal', 1, 'InProgress', 'High', 5, DATEADD(DAY, 5, GETUTCDATE()), NULL, DATEADD(DAY, -30, GETUTCDATE())),
    (1, 'Order management system', 'Order tracking and history', 3, 'ToDo', 'Medium', 3, DATEADD(DAY, 10, GETUTCDATE()), NULL, DATEADD(DAY, -25, GETUTCDATE())),
    (1, 'Email notifications', 'Order confirmation and shipping updates', 4, 'Blocked', 'Low', 2, DATEADD(DAY, 3, GETUTCDATE()), NULL, DATEADD(DAY, -20, GETUTCDATE())),
    (1, 'Performance optimization', 'Caching and query optimization', 1, 'ToDo', 'Medium', 4, DATEADD(DAY, 15, GETUTCDATE()), NULL, DATEADD(DAY, -15, GETUTCDATE()));

-- Project 2: Mobile Banking App (7 tasks)
INSERT INTO TaskManagement.Tasks (ProjectId, Title, Description, AssigneeId, Status, Priority, EstimatedComplexity, DueDate, CompletionDate, CreatedAt)
VALUES
    -- Completed tasks
    (2, 'Mobile app scaffolding', 'Initial Flutter project setup', 3, 'Completed', 'Medium', 2, '2024-03-10', '2024-03-08', DATEADD(DAY, -60, GETUTCDATE())),
    (2, 'Account dashboard UI', 'Display balance and recent transactions', 3, 'Completed', 'High', 3, '2024-03-20', '2024-03-25', DATEADD(DAY, -55, GETUTCDATE())), -- 5 days late
    (2, 'Biometric authentication', 'Fingerprint and Face ID support', 4, 'Completed', 'High', 4, '2024-04-01', '2024-04-01', DATEADD(DAY, -50, GETUTCDATE())), -- On time
    -- Open tasks
    (2, 'Money transfer feature', 'Internal and external transfers', 3, 'InProgress', 'High', 5, DATEADD(DAY, 4, GETUTCDATE()), NULL, DATEADD(DAY, -20, GETUTCDATE())),
    (2, 'Bill payment module', 'Utility and service bill payments', 4, 'InProgress', 'Medium', 3, DATEADD(DAY, 8, GETUTCDATE()), NULL, DATEADD(DAY, -18, GETUTCDATE())),
    (2, 'Transaction history export', 'PDF and CSV export functionality', 5, 'ToDo', 'Low', 2, DATEADD(DAY, 20, GETUTCDATE()), NULL, DATEADD(DAY, -10, GETUTCDATE())),
    (2, 'Push notifications', 'Real-time transaction alerts', 5, 'Blocked', 'Medium', 3, DATEADD(DAY, 2, GETUTCDATE()), NULL, DATEADD(DAY, -8, GETUTCDATE()));

-- Project 3: Healthcare Portal (5 tasks)
INSERT INTO TaskManagement.Tasks (ProjectId, Title, Description, AssigneeId, Status, Priority, EstimatedComplexity, DueDate, CompletionDate, CreatedAt)
VALUES
    (3, 'Patient registration system', 'CRUD for patient records', 2, 'InProgress', 'High', 4, DATEADD(DAY, 6, GETUTCDATE()), NULL, DATEADD(DAY, -15, GETUTCDATE())),
    (3, 'Appointment scheduling', 'Calendar-based appointment booking', 5, 'ToDo', 'High', 4, DATEADD(DAY, 12, GETUTCDATE()), NULL, DATEADD(DAY, -12, GETUTCDATE())),
    (3, 'Medical records viewer', 'Secure document viewing system', 2, 'ToDo', 'Medium', 3, DATEADD(DAY, 18, GETUTCDATE()), NULL, DATEADD(DAY, -10, GETUTCDATE())),
    (3, 'Doctor availability API', 'Real-time availability checking', 4, 'ToDo', 'Medium', 2, DATEADD(DAY, 25, GETUTCDATE()), NULL, DATEADD(DAY, -5, GETUTCDATE())),
    (3, 'Insurance verification', 'Integration with insurance providers', 1, 'ToDo', 'Low', 5, DATEADD(DAY, 30, GETUTCDATE()), NULL, DATEADD(DAY, -3, GETUTCDATE()));
GO

-- ++++++++++++++++++++++++++++++++++
-- SECTION 4: REQUIRED QUERIES (DML)
-- ++++++++++++++++++++++++++++++++++


-- 2.2.1 Developer Workload Summary (Active developers only)
-- Returns: DeveloperName, OpenTasksCount, AverageEstimatedComplexity
-- =====================
CREATE OR ALTER VIEW TaskManagement.vw_DeveloperWorkload
AS
SELECT
    d.DeveloperId,
    CONCAT(d.FirstName, ' ', d.LastName) AS DeveloperName,
    d.Email,
    COUNT(t.TaskId) AS OpenTasksCount,
    ISNULL(CAST(AVG(CAST(t.EstimatedComplexity AS DECIMAL(10,2))) AS DECIMAL(10,2)), 0) AS AverageEstimatedComplexity
FROM TaskManagement.Developers d
LEFT JOIN TaskManagement.Tasks t ON d.DeveloperId = t.AssigneeId AND t.Status <> 'Completed'
WHERE d.IsActive = 1
GROUP BY d.DeveloperId, d.FirstName, d.LastName, d.Email;
GO


-- 2.2.2 Project Health Summary
-- Returns: ProjectName, TotalTasks, OpenTasks, CompletedTasks
-- =====================
CREATE OR ALTER VIEW TaskManagement.vw_ProjectHealth
AS
SELECT
    p.ProjectId,
    p.Name AS ProjectName,
    p.ClientName,
    p.Status AS ProjectStatus,
    COUNT(t.TaskId) AS TotalTasks,
    SUM(CASE WHEN t.Status <> 'Completed' THEN 1 ELSE 0 END) AS OpenTasks,
    SUM(CASE WHEN t.Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedTasks
FROM TaskManagement.Projects p
LEFT JOIN TaskManagement.Tasks t ON p.ProjectId = t.ProjectId
GROUP BY p.ProjectId, p.Name, p.ClientName, p.Status;
GO


-- 2.2.3 Tasks Due Soon (Next 7 days, not completed)
-- =====================
CREATE OR ALTER VIEW TaskManagement.vw_TasksDueSoon
AS
SELECT
    t.TaskId,
    t.Title,
    t.Description,
    p.Name AS ProjectName,
    CONCAT(d.FirstName, ' ', d.LastName) AS AssigneeName,
    t.Status,
    t.Priority,
    t.EstimatedComplexity,
    t.DueDate,
    DATEDIFF(DAY, GETUTCDATE(), t.DueDate) AS DaysUntilDue
FROM TaskManagement.Tasks t
INNER JOIN TaskManagement.Projects p ON t.ProjectId = p.ProjectId
LEFT JOIN TaskManagement.Developers d ON t.AssigneeId = d.DeveloperId
WHERE t.Status <> 'Completed'
    AND t.DueDate IS NOT NULL
    AND t.DueDate BETWEEN CAST(GETUTCDATE() AS DATE) AND DATEADD(DAY, 7, CAST(GETUTCDATE() AS DATE));
GO


-- 2.2.4 Stored Procedure: Insert New Task
-- Validates ProjectId and AssigneeId existence
-- Returns descriptive error messages on validation failure
-- =====================
CREATE OR ALTER PROCEDURE TaskManagement.sp_InsertTask
    @ProjectId INT,
    @Title NVARCHAR(300),
    @Description NVARCHAR(MAX) = NULL,
    @AssigneeId INT = NULL,
    @Status NVARCHAR(20) = 'ToDo',
    @Priority NVARCHAR(10) = 'Medium',
    @EstimatedComplexity INT = 3,
    @DueDate DATE = NULL,
    @NewTaskId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validate ProjectId exists
        IF NOT EXISTS (SELECT 1 FROM TaskManagement.Projects WHERE ProjectId = @ProjectId)
        BEGIN
            RAISERROR('Error: ProjectId %d does not exist.', 16, 1, @ProjectId);
            RETURN;
        END

        -- Validate AssigneeId exists (if provided)
        IF @AssigneeId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TaskManagement.Developers WHERE DeveloperId = @AssigneeId)
        BEGIN
            RAISERROR('Error: AssigneeId %d does not exist.', 16, 1, @AssigneeId);
            RETURN;
        END

        -- Validate AssigneeId is active (if provided)
        IF @AssigneeId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TaskManagement.Developers WHERE DeveloperId = @AssigneeId AND IsActive = 1)
        BEGIN
            RAISERROR('Error: AssigneeId %d is not an active developer.', 16, 1, @AssigneeId);
            RETURN;
        END

        -- Validate Title is not empty
        IF LTRIM(RTRIM(@Title)) = ''
        BEGIN
            RAISERROR('Error: Title cannot be empty.', 16, 1);
            RETURN;
        END

        -- Validate Status
        IF @Status NOT IN ('ToDo', 'InProgress', 'Blocked', 'Completed')
        BEGIN
            RAISERROR('Error: Invalid Status. Must be one of: ToDo, InProgress, Blocked, Completed.', 16, 1);
            RETURN;
        END

        -- Validate Priority
        IF @Priority NOT IN ('Low', 'Medium', 'High')
        BEGIN
            RAISERROR('Error: Invalid Priority. Must be one of: Low, Medium, High.', 16, 1);
            RETURN;
        END

        -- Validate EstimatedComplexity
        IF @EstimatedComplexity < 1 OR @EstimatedComplexity > 5
        BEGIN
            RAISERROR('Error: EstimatedComplexity must be between 1 and 5.', 16, 1);
            RETURN;
        END

        -- Insert the task
        INSERT INTO TaskManagement.Tasks (
            ProjectId, Title, Description, AssigneeId,
            Status, Priority, EstimatedComplexity, DueDate
        )
        VALUES (
            @ProjectId, @Title, @Description, @AssigneeId,
            @Status, @Priority, @EstimatedComplexity, @DueDate
        );

        SET @NewTaskId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        -- Return the created task
        SELECT * FROM TaskManagement.Tasks WHERE TaskId = @NewTaskId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO


-- =============================================
-- Stored Procedure: sp_UpdateTaskStatus
-- Author:      Roimer
-- Create date: 2026-01-26
-- Description: Updates task status with optional priority/complexity
--              and handles CompletionDate logic automatically.
-- =============================================
CREATE OR ALTER PROCEDURE TaskManagement.sp_UpdateTaskStatus
    @TaskId INT,
    @Status NVARCHAR(20),
    @Priority NVARCHAR(10) = NULL,
    @EstimatedComplexity INT = NULL,
    @UpdatedTaskId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Validate Task Existence
        IF NOT EXISTS (SELECT 1 FROM TaskManagement.Tasks WHERE TaskId = @TaskId)
        BEGIN
            RAISERROR('Error: TaskId %d not found.', 16, 1, @TaskId);
            RETURN;
        END

        -- 2. Validate Status
        IF @Status NOT IN ('ToDo', 'InProgress', 'Blocked', 'Completed')
        BEGIN
            RAISERROR('Error: Invalid Status. Must be one of: ToDo, InProgress, Blocked, Completed.', 16, 1);
            RETURN;
        END

        -- 3. Validate Priority (if provided)
        IF @Priority IS NOT NULL AND @Priority NOT IN ('Low', 'Medium', 'High')
        BEGIN
            RAISERROR('Error: Invalid Priority. Must be one of: Low, Medium, High.', 16, 1);
            RETURN;
        END

        -- 4. Validate Complexity (if provided)
        IF @EstimatedComplexity IS NOT NULL AND (@EstimatedComplexity < 1 OR @EstimatedComplexity > 5)
        BEGIN
            RAISERROR('Error: EstimatedComplexity must be between 1 and 5.', 16, 1);
            RETURN;
        END

        -- 5. Handle CompletionDate logic
        DECLARE @CompletionDate DATE = NULL;
        DECLARE @OldCompletionDate DATE;

        SELECT @OldCompletionDate = CompletionDate
        FROM TaskManagement.Tasks WHERE TaskId = @TaskId;

        -- If moving TO Completed -> Set CompletionDate (keep existing if already set)
        -- If moving FROM Completed -> Clear CompletionDate
        IF @Status = 'Completed'
            SET @CompletionDate = ISNULL(@OldCompletionDate, GETUTCDATE());
        ELSE
            SET @CompletionDate = NULL;

        -- 6. Update
        UPDATE TaskManagement.Tasks
        SET
            Status = @Status,
            Priority = ISNULL(@Priority, Priority),
            EstimatedComplexity = ISNULL(@EstimatedComplexity, EstimatedComplexity),
            CompletionDate = @CompletionDate
        WHERE TaskId = @TaskId;

        SET @UpdatedTaskId = @TaskId;

        COMMIT TRANSACTION;

        -- Return the updated task
        SELECT * FROM TaskManagement.Tasks WHERE TaskId = @TaskId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO


-- SECTION 5: DEVELOPER DELAY RISK PREDICTION (2.3)
-- Advanced SQL query to estimate delay risk per developer
-- =====================


-- Assumptions documented:
-- 1. AvgDelayDays is calculated only from completed tasks with a DueDate
-- 2. If a task was completed before or on DueDate, delay = 0
-- 3. PredictedCompletionDate = LatestDueDate + AvgDelayDays (rounded up)
-- 4. HighRiskFlag = 1 if AvgDelayDays > 0 (any delay history marks as risk)
-- 5. Developers with no completed tasks default to AvgDelayDays = 0
-- =====================

CREATE OR ALTER VIEW TaskManagement.vw_DeveloperDelayRisk
AS
WITH CompletedTasksStats AS (
    -- Calculate delay statistics from completed tasks
    SELECT
        t.AssigneeId,
        COUNT(*) AS CompletedTasksCount,
        AVG(
            CASE
                WHEN t.CompletionDate > t.DueDate
                THEN CAST(DATEDIFF(DAY, t.DueDate, t.CompletionDate) AS DECIMAL(10,2))
                ELSE 0
            END
        ) AS AvgDelayDays
    FROM TaskManagement.Tasks t
    WHERE t.Status = 'Completed'
        AND t.DueDate IS NOT NULL
        AND t.CompletionDate IS NOT NULL
        AND t.AssigneeId IS NOT NULL
    GROUP BY t.AssigneeId
),
OpenTasksStats AS (
    -- Calculate open tasks statistics
    SELECT
        t.AssigneeId,
        COUNT(*) AS OpenTasksCount,
        MIN(t.DueDate) AS NearestDueDate,
        MAX(t.DueDate) AS LatestDueDate
    FROM TaskManagement.Tasks t
    WHERE t.Status <> 'Completed'
        AND t.AssigneeId IS NOT NULL
    GROUP BY t.AssigneeId
)
SELECT
    d.DeveloperId,
    CONCAT(d.FirstName, ' ', d.LastName) AS DeveloperName,
    d.Email,
    ISNULL(ot.OpenTasksCount, 0) AS OpenTasksCount,
    ISNULL(CAST(ct.AvgDelayDays AS DECIMAL(10,2)), 0) AS AvgDelayDays,
    ot.NearestDueDate,
    ot.LatestDueDate,
    -- PredictedCompletionDate: LatestDueDate + AvgDelayDays
    CASE
        WHEN ot.LatestDueDate IS NOT NULL
        THEN DATEADD(DAY, CEILING(ISNULL(ct.AvgDelayDays, 0)), ot.LatestDueDate)
        ELSE NULL
    END AS PredictedCompletionDate,
    -- HighRiskFlag: 1 if developer has any delay history
    CASE WHEN ISNULL(ct.AvgDelayDays, 0) > 0 THEN 1 ELSE 0 END AS HighRiskFlag
FROM TaskManagement.Developers d
LEFT JOIN CompletedTasksStats ct ON d.DeveloperId = ct.AssigneeId
LEFT JOIN OpenTasksStats ot ON d.DeveloperId = ot.AssigneeId
WHERE d.IsActive = 1;
GO


-- SECTION 6: UTILITY QUERIES FOR TESTING
-- =====================

-- Test query: View all data
PRINT '### DEVELOPERS ###';
SELECT * FROM TaskManagement.Developers;

PRINT '### PROJECTS ###';
SELECT * FROM TaskManagement.Projects;

PRINT '### TASKS ###';
SELECT * FROM TaskManagement.Tasks;

PRINT '### DEVELOPER WORKLOAD (2.2.1) ###';
SELECT * FROM TaskManagement.vw_DeveloperWorkload ORDER BY OpenTasksCount DESC;

PRINT '### PROJECT HEALTH (2.2.2) ###';
SELECT * FROM TaskManagement.vw_ProjectHealth;

PRINT '### TASKS DUE SOON (2.2.3) ###';
SELECT * FROM TaskManagement.vw_TasksDueSoon ORDER BY DueDate;

PRINT '### DEVELOPER DELAY RISK (2.3) ###';
SELECT * FROM TaskManagement.vw_DeveloperDelayRisk ORDER BY HighRiskFlag DESC, AvgDelayDays DESC;

-- Test sp_InsertTask
PRINT '### TEST INSERT TASK ###';
DECLARE @NewId INT;
EXEC TaskManagement.sp_InsertTask
    @ProjectId = 1,
    @Title = 'Test Task from SP',
    @Description = 'This is a test task created via stored procedure',
    @AssigneeId = 1,
    @Status = 'ToDo',
    @Priority = 'High',
    @EstimatedComplexity = 3,
    @DueDate = NULL,
    @NewTaskId = @NewId OUTPUT;

PRINT 'New Task ID: ' + CAST(@NewId AS VARCHAR(10));
GO

PRINT '### DATABASE SETUP COMPLETE ###';
GO
