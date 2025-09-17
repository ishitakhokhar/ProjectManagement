--Roles
CREATE TABLE SEC_Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName VARCHAR(50) NOT NULL UNIQUE
)

select * from SEC_Roles

--User
CREATE TABLE SEC_Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    RoleID INT NOT NULL,
	IsActive bit Not Null,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES SEC_Roles(RoleID)
);	



select * from SEC_Users
SELECT UserId, FullName, RoleID FROM SEC_Users;


--Projects
CREATE TABLE PRJ_Project (
    ProjectID INT PRIMARY KEY IDENTITY(1,1),
    ProjectName VARCHAR(100) NOT NULL,
    --ProjectKey VARCHAR(10) UNIQUE, -- e.g., JIRA
    ProjectDescription TEXT,
    ClientName VARCHAR(100),
	ProjectManagerName VARCHAR(100),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME,
    ProjectStatus VARCHAR(50) DEFAULT 'Active', -- Active/Completed/On Hold
    Visibility VARCHAR(20) DEFAULT 'Private', -- Public/Private
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    FOREIGN KEY (CreatedBy) REFERENCES SEC_Users(UserID)
);

select * from PRJ_Project

--ProjectMember
CREATE TABLE PRJ_ProjectMembers (
    ProjectMemberID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT NOT NULL,
    UserID INT NOT NULL,
    RoleInProject VARCHAR(50) NOT NULL,
    FOREIGN KEY (ProjectID) REFERENCES PRJ_Project(ProjectID),
    FOREIGN KEY (UserID) REFERENCES SEC_Users(UserID)
);

select * from PRJ_ProjectMembers

--IssueStatus
CREATE TABLE MST_Status (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName VARCHAR(50) NOT NULL UNIQUE
);

select * from MST_Status

--Tasks 
CREATE TABLE PRJ_Issues (
    IssueID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT NOT NULL,
	RaisedOn Datetime Not Null,
    Title VARCHAR(200) NOT NULL,
    Description TEXT,
    StatusID INT NOT NULL,
    Priority VARCHAR(20) NOT NULL CHECK (Priority IN ('Low', 'Medium', 'High', 'Critical')),
    CreatedBy INT NOT NULL,
    AssignedTo INT,
	Attachment1 varchar(250),
	Attachment2 varchar(250),
    CreatedAt DATETIME DEFAULT GETDATE(),
    DueDate DATETIME,
    FOREIGN KEY (ProjectID) REFERENCES PRJ_Project(ProjectID),
    FOREIGN KEY (StatusID) REFERENCES MST_Status(StatusID),
    FOREIGN KEY (CreatedBy) REFERENCES SEC_Users(UserID),
    FOREIGN KEY (AssignedTo) REFERENCES SEC_Users(UserID)
);

--IssueComments
CREATE TABLE PRJ_IssueComments (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    IssueID INT NOT NULL,
    CommentText TEXT NOT NULL,
	IsShow bit not null,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IssueID) REFERENCES PRJ_Issues(IssueID),
    FOREIGN KEY (CreatedBy) REFERENCES SEC_Users(UserID)
);

-- Sprints
CREATE TABLE SPR_Sprint (
    SprintID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT NOT NULL,
    SprintName VARCHAR(100) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    IsCompleted BIT DEFAULT 0,
    TotalTasks INT NULL,
    FOREIGN KEY (ProjectID) REFERENCES PRJ_Project(ProjectID)
);

select * from SPR_Sprint

DELETE FROM PRJ_IssueComments;
DELETE FROM PRJ_Issues;
DELETE FROM SPR_Sprint;
DELETE FROM PRJ_ProjectMembers;
DELETE FROM PRJ_Project;

DELETE FROM SEC_Users;
DELETE FROM MST_Status;
DELETE FROM SEC_Roles;

INSERT INTO SEC_Roles (RoleName) VALUES ('Admin');
INSERT INTO SEC_Roles (RoleName) VALUES ('ProjectManager');
INSERT INTO SEC_Roles (RoleName) VALUES ('User');

select * from SEC_Roles

INSERT INTO SEC_Users (FullName, Email, PasswordHash, RoleID, IsActive)
VALUES
('Admin', 'admin@test.com', '$2a$12$tkIErRx2c2MijwU1FWmscu5cBaK0lNS.g9QKYrMOs4a9Hw5PNPDDG', 36, 1),
('PM User', 'pm@test.com', '$2a$11$FjQx6yGfW0Bwl0oYcGFwfeOzpT0bZa6qEuwg1q5c8uQ6XHsjc7g1O', 37, 1),
('Normal User', 'user@test.com', '$2a$11$g6LjXrR9oGQZsVjzS6/2Cu3BK0cEbE8lRyvXW3F2H7pqlFsZ4z3Ca', 38, 1);

select * from SEC_Users


-- Check inserted ProjectID
SELECT * FROM PRJ_Project;









