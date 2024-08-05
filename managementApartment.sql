-- Create Database
CREATE DATABASE ApartmentManagement;
GO

-- Use the new Database
USE ApartmentManagement;
GO

-- Table Definitions

-- Managers Table
CREATE TABLE Managers (
    ManagerID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    FullName NVARCHAR(100) NULL,
	Username NVARCHAR(100) NULL,
    Password NVARCHAR(255) NULL,
	Role INT NOT NULL DEFAULT 2,
    Address NVARCHAR(255),
    PhoneNumber NVARCHAR(10) NULL,
    Email NVARCHAR(100) NULL,
	Token NVARCHAR(100) NULL,
	RefreshToken NVARCHAR(100) NULL,
	RefreshTokenExpiryTime DATE NULL,
	ResetPasswordToken NVARCHAR(255) NULL,
	ResetPasswordExpiry DATE NULL,
	CreateDate DATE NULL,
    UpdateDate DATE NULL,
	Status bit NOT NULL DEFAULT 1,
    
);

-- Buildings Table
CREATE TABLE Buildings (
    BuildingID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ManagerID INT NULL,
    BuildingName NVARCHAR(100) NULL,
	CreateDate DATE NULL,
    UpdateDate DATE NULL,
    FOREIGN KEY (ManagerID) REFERENCES Managers(ManagerID)
);

-- Floors Table
CREATE TABLE Floors (
    FloorID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    BuildingID INT NULL,
    FloorNumber INT NULL,
    FOREIGN KEY (BuildingID) REFERENCES Buildings(BuildingID)
);

-- Services Table
CREATE TABLE Services (
    ServiceID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ManagerID INT NULL,
    ServiceName NVARCHAR(100) NULL,
    ServiceFee DECIMAL(18, 2) NULL,
	FOREIGN KEY (ManagerID) REFERENCES Managers(ManagerID)
);

-- Rooms Table
CREATE TABLE Rooms (
    RoomID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    FloorID INT NULL,
    ServiceID INT NULL,
    RoomNumber NVARCHAR(10) NULL,
    FOREIGN KEY (FloorID) REFERENCES Floors(FloorID),
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID)
);

-- Residents Table
CREATE TABLE Residents (
    ResidentID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	RoomID INT NULL,
    FullName NVARCHAR(100) NULL,
    Address NVARCHAR(255) NULL,
    PhoneNumber NVARCHAR(10) NULL,
    Email NVARCHAR(100) NULL,
    CCCD NVARCHAR(20) NULL,
	Avatar [nchar](50) NULL,
	CreateDate DATE NULL,
    UpdateDate DATE NULL,
	Status bit NOT NULL DEFAULT 1,
	FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),
);

-- Vehicles Table
CREATE TABLE Vehicles (
    VehicleID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ResidentID INT NULL,
    LicensePlate NVARCHAR(20) NULL,
    VehicleType NVARCHAR(50) NULL,
	CreateDate DATE NULL,
    UpdateDate DATE NULL,
    FOREIGN KEY (ResidentID) REFERENCES Residents(ResidentID)
);

-- Contracts Table
CREATE TABLE Contracts (
    ContractID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    RoomID INT NULL,
    ResidentID INT NULL,
    StartDate DATE NULL,
    EndDate DATE NULL,
    RentPrice DECIMAL(18, 2) NULL,
    Status bit NOT NULL DEFAULT 1,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),
    FOREIGN KEY (ResidentID) REFERENCES Residents(ResidentID)
);

-- Payments Table
CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ServiceID INT NULL,
    RoomID INT NULL,
    PaymentDate DATE NULL,
    Amount DECIMAL(18, 2)  NULL,
    PaymentType NVARCHAR(50) NULL,
	CreateDate DATE NULL,
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID)
);

GO

-- Insert Initial Data

-- Insert Managers
SET IDENTITY_INSERT Managers ON 
INSERT INTO Managers (ManagerID, FullName, Username, Password, Role, Address, PhoneNumber, Email, Token, CreateDate, UpdateDate) VALUES (1, 'Nguyen Van Tung','manager_user1','123456', 2, 'Hà Nội', '0909123456', 'nguyenvantung@gmail.com', null, null, null);
INSERT INTO Managers (ManagerID, FullName, Username, Password, Role, Address, PhoneNumber, Email, Token, CreateDate, UpdateDate) VALUES (2, 'Tran Thi Dung','manager_user2','123456', 2, 'Nam Định', '0909123457', 'tranthidung@gmail.com', null, null, null);
INSERT INTO Managers (ManagerID, FullName, Username, Password, Role, Address, PhoneNumber, Email, Token, CreateDate, UpdateDate) VALUES (3, 'Pham Van Cuong', 'admin', '123456', 1, NULL, NULL, 'phamduycuongit@gmail.com', null,null, null);
SET IDENTITY_INSERT Managers OFF 
GO
-- Insert Buildings
SET IDENTITY_INSERT Buildings ON 
INSERT INTO Buildings (BuildingID, ManagerID, BuildingName, CreateDate, UpdateDate) VALUES (1, 1, 'Building 1', null, null);
INSERT INTO Buildings (BuildingID, ManagerID, BuildingName, CreateDate, UpdateDate) VALUES (2, 2, 'Building 2', null, null);
SET IDENTITY_INSERT Buildings OFF
GO
-- Insert Floors
SET IDENTITY_INSERT Floors ON
INSERT INTO Floors (FloorID, BuildingID, FloorNumber) VALUES (1, 1, 1);
INSERT INTO Floors (FloorID, BuildingID, FloorNumber) VALUES (2, 1, 2);
INSERT INTO Floors (FloorID, BuildingID, FloorNumber) VALUES (3, 2, 1);
INSERT INTO Floors (FloorID, BuildingID, FloorNumber) VALUES (4, 2, 2);
SET IDENTITY_INSERT Floors OFF
GO
-- Insert Services
SET IDENTITY_INSERT Services ON
INSERT INTO Services (ServiceID, ManagerID, ServiceName, ServiceFee) VALUES (1, 3,  'Electricity', 3500);
INSERT INTO Services (ServiceID, ManagerID, ServiceName, ServiceFee) VALUES (2, 3, 'Water', 10000);
INSERT INTO Services (ServiceID, ManagerID, ServiceName, ServiceFee) VALUES (3, 3, 'Internet', 200000);
INSERT INTO Services (ServiceID, ManagerID, ServiceName, ServiceFee) VALUES (4, 3, 'Rent', 5000000);
SET IDENTITY_INSERT Services OFF
GO
-- Insert Rooms
SET IDENTITY_INSERT Rooms ON
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (1, 1, '101', 1); -- Room 101 has Electricity
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (2, 1, '102', 2); -- Room 102 has Water
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (3, 2, '201', 3); -- Room 201 has Internet
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (4, 2, '202', 1); -- Room 202 has Electricity
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (5, 3, '101', 2); -- Room 101 has Water
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (6, 3, '102', 3); -- Room 102 has Internet
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (7, 4, '201', 1); -- Room 201 has Electricity
INSERT INTO Rooms (RoomID, FloorID, RoomNumber, ServiceID) VALUES (8, 4, '202', 2); -- Room 202 has Water
SET IDENTITY_INSERT Rooms OFF
GO
-- Insert Residents
SET IDENTITY_INSERT Residents ON
INSERT INTO Residents (ResidentID, RoomID, FullName, Address, PhoneNumber, Email, CCCD, Avatar, CreateDate, UpdateDate) VALUES (1, 1, 'Le Van Cuong', 'Hà Nội', '0909123458', 'levancuong@gmail.com', '012345678903', NULL, NULL, NULL);
INSERT INTO Residents (ResidentID, RoomID, FullName, Address, PhoneNumber, Email, CCCD, Avatar, CreateDate, UpdateDate) VALUES (2, 2, 'Pham Van Truong', 'Hà Tĩnh', '0909123459', 'phamvantruong@gmail.com', '012345678904', NULL, NULL, NULL);
SET IDENTITY_INSERT Residents OFF
GO
-- Insert Vehicles
SET IDENTITY_INSERT Vehicles ON
INSERT INTO Vehicles (VehicleID, ResidentID, LicensePlate, VehicleType, CreateDate, UpdateDate) VALUES (1, 1, '30A-12345', 'Car', null, null);
INSERT INTO Vehicles (VehicleID, ResidentID, LicensePlate, VehicleType, CreateDate, UpdateDate) VALUES (2, 2, '29B-67890', 'Motorbike', null, null);
SET IDENTITY_INSERT Vehicles OFF
GO
-- Insert Contracts
SET IDENTITY_INSERT Contracts ON
INSERT INTO Contracts (ContractID, RoomID, ResidentID, StartDate, EndDate, RentPrice) VALUES (1, 1, 1, '2023-01-01', '2023-12-31', 5000000);
INSERT INTO Contracts (ContractID, RoomID, ResidentID, StartDate, EndDate, RentPrice) VALUES (2, 2, 2, '2023-01-01', '2023-12-31', 5500000);
SET IDENTITY_INSERT Contracts OFF
GO
-- Insert Payments
SET IDENTITY_INSERT Payments ON
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (1, 4, 1, '2023-02-01', 5000000, 'Rent', null);
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (2, 4, 2, '2023-02-01', 5000000, 'Rent', null);
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (3, 1, 1, '2023-02-01', 350000, 'Electricity', null);
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (4, 1, 2, '2023-02-01', 350000, 'Electricity', null);
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (5, 2, 1, '2023-02-01', 100000, 'Water', null);
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (6, 2, 2, '2023-02-01', 100000, 'Water', null);
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (7, 3, 1, '2023-02-01', 200000, 'Internet', null);
INSERT INTO Payments (PaymentID, ServiceID, RoomID, PaymentDate, Amount, PaymentType, CreateDate) VALUES (8, 3, 2, '2023-02-01', 200000, 'Internet', null);
SET IDENTITY_INSERT Payments OFF
