CREATE TABLE Addresses(
    Id INT identity constraint PK_Addresses primary key,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    FirstName        varchar(255) not null,
    LastName         varchar(255) not null,
    Street1          varchar(255) not null,
    Street2          varchar(255),
    City             varchar(255) not null,
    State            varchar(255) not null,
    Country          varchar(255) not null,
    ZipCode          varchar(255) not null
)
GO

CREATE TABLE ServiceCategories(
    Id INT identity constraint PK_ServiceCategories primary key,
    IsEnabled BIT NOT NULL DEFAULT 1,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
)
GO

CREATE TABLE ServiceSubCategories(
    Id INT identity constraint PK_ServiceSubCategories primary key,
    IsEnabled BIT NOT NULL DEFAULT 1,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
)
GO

CREATE TABLE Businesses(
    Id INT identity constraint PK_Businesses primary key,
    RefId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
)
GO

CREATE TABLE Stores(
    Id INT identity constraint PK_Stores primary key,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    BusinessRefId UNIQUEIDENTIFIER NOT NULL,
    BusinessId INT NOT NULL,
    FOREIGN KEY (BusinessId) REFERENCES Businesses(Id),
)
GO

CREATE TABLE StoreServices(
    Id INT identity constraint PK_StoreServices primary key,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Duration INT,
    Price DECIMAL(10, 2),
    BusinessRefId UNIQUEIDENTIFIER NOT NULL,
    StoreId INT NOT NULL,
    FOREIGN KEY (StoreId) REFERENCES Stores(Id),
)
GO

CREATE TABLE CategorizeStoreServices(
    Id INT identity constraint PK_CategorizeStoreServices primary key,
    IsEnabled BIT NOT NULL DEFAULT 1,
    StoreServiceId INT NOT NULL,
    FOREIGN KEY (StoreServiceId) REFERENCES StoreServices(Id),
    ServiceCategoryId INT NOT NULL,
    FOREIGN KEY (ServiceCategoryId) REFERENCES ServiceCategories(Id),
    ServiceSubCategoryId INT NOT NULL,
    FOREIGN KEY (ServiceSubCategoryId) REFERENCES ServiceSubCategories(Id)
)
GO

CREATE TABLE StoreLocations(
    Id INT identity constraint PK_StoreLocations primary key,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    Name NVARCHAR(255) NOT NULL,
    Latitude DECIMAL(9, 6) NOT NULL,
    Longitude DECIMAL(9, 6) NOT NULL,
    StoreId INT NOT NULL,
    FOREIGN KEY (StoreId) REFERENCES Stores(Id),
    AddressId INT NOT NULL,
    FOREIGN KEY (AddressId) REFERENCES Addresses(Id),
)
GO

CREATE TABLE StoreEmployees(
    Id INT identity constraint PK_StoreEmployees primary key,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    NickName NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(25) NOT NULL,
    StoreId INT NOT NULL,
    FOREIGN KEY (StoreId) REFERENCES Stores(Id),
)
GO

CREATE TABLE Clients(
    Id INT identity constraint PK_Clients primary key,
    RefId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(25) NOT NULL,
)
GO

CREATE TABLE AppointmentStatuses(
    Id INT identity constraint PK_AppointmentStatuses primary key,
    IsEnabled BIT NOT NULL DEFAULT 1,
    Name NVARCHAR(255) NOT NULL,
)
GO

CREATE TABLE Appointments(
    Id INT identity constraint PK_Appointments primary key,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    IsEnabled BIT NOT NULL DEFAULT 1,
    Notes NVARCHAR(MAX),
    StartDate DATETIME NOT NULL,
    StatusId INT NOT NULL,
    FOREIGN KEY (StatusId) REFERENCES AppointmentStatuses(Id),
    ClientId INT NOT NULL,
    FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    StoreServiceId INT NOT NULL,
    FOREIGN KEY (StoreServiceId) REFERENCES StoreServices(Id),
    StoreLocationId INT NOT NULL,
    FOREIGN KEY (StoreLocationId) REFERENCES StoreLocations(Id),
    StoreEmployeeId INT NOT NULL,
    FOREIGN KEY (StoreEmployeeId) REFERENCES StoreEmployees(Id),
    StoreId INT NOT NULL,
    FOREIGN KEY (StoreId) REFERENCES Stores(Id)
)
GO

CREATE INDEX IDX_Appointments_ClientId ON Appointments(ClientId);
CREATE INDEX IDX_Appointments_StoreId ON Appointments(StoreId);
CREATE INDEX IDX_Appointments_StoreServiceId ON Appointments(StoreServiceId);
CREATE INDEX IDX_Appointments_StoreLocationId ON Appointments(StoreLocationId);
CREATE INDEX IDX_Appointments_StoreEmployeeId ON Appointments(StoreEmployeeId);


-- Insert Service Categories
INSERT INTO ServiceCategories (IsEnabled, Name, Description) VALUES
                                                                 (1, 'Hair', 'Hair category'),
                                                                 (1, 'Nail', 'Nail category'),
                                                                 (1, 'Massage', 'Massage category'),
                                                                 (1, 'Spa', 'Spa category'),
                                                                 (1, 'Wellness', 'Wellness category');

-- Insert Service SubCategories
INSERT INTO ServiceSubCategories (IsEnabled, Name, Description) VALUES
                                                                    (1, 'Women', 'Services provided to women'),
                                                                    (1, 'Men', 'Services provided to men'),
                                                                    (1, 'Kids', 'Services provided to kids'),
                                                                    (1, 'Dogs', 'Services provided to dogs'),
                                                                    (1, 'Cats', 'Services provided to cats');