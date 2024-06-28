CREATE DATABASE DBEmployee;

USE DBEmployee
GO

CREATE TABLE Table_Employee(
	Name varchar(50) NULL,
	CPF varchar(14) UNIQUE NULL,
	BirthDt datetime NULL,
	Sex char(1) NULL,
	IdAddress varchar(255) NULL,
	Salary float NULL,
	Phone varchar(50) NULL,
	Email varchar(50) NULL,
	Manager bit NULL,
	Registry int NOT NULL,
 CONSTRAINT PK_Table_Employee PRIMARY KEY (Registry) 
);


CREATE TABLE Table_EmployeeCanceled(
	Name varchar(50) NULL,
	CPF varchar(14) NULL,
	BirthDt datetime NULL,
	Sex char(1) NULL,
	IdAddress varchar(255) NULL,
	Salary float NULL,
	Phone varchar(50) NULL,
	Email varchar(50) NULL,
	Manager bit NULL,
	Registry int NOT NULL,
 CONSTRAINT PK_Table_EmployeeCanceled PRIMARY KEY (Registry)
);
