create database UpBankClientAPI;
use UpBankClientAPI;

CREATE TABLE Clients (
    CPF VARCHAR(14) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    BirthDt DATE NOT NULL,
    Sex CHAR(1) NOT NULL,
    AddressId VARCHAR(255) NOT NULL,
    Salary float NOT NULL,
    Phone VARCHAR(40) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Restriction BIT NOT NULL,
    
    CONSTRAINT PK_Clients PRIMARY KEY (CPF)
);    

CREATE TABLE DeletedClient (
    CPF VARCHAR(14),
    Name VARCHAR(100) NOT NULL,
    BirthDt DATE NOT NULL,
    Sex CHAR(1) NOT NULL,
    AddressId VARCHAR(255) NOT NULL,
    Salary float NOT NULL,
    Phone VARCHAR(40) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Restriction BIT NOT NULL,
    
    CONSTRAINT PK_DeletedClient PRIMARY KEY (CPF)
);  
