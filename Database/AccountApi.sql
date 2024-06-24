create database DBAccountUpBank;
use DBAccountUpBank;

if exists (select * from sysobjects where name='AccountTransaction' and xtype='U')
	drop table AccountTransaction;

if exists (select * from sysobjects where name='ClientAccount' and xtype='U')
	drop table ClientAccount;

if exists (select * from sysobjects where name='Account' and xtype='U')
	drop table Account;

if exists (select * from sysobjects where name='AccountHistory' and xtype='U')
	drop table AccountHistory;

if exists (select * from sysobjects where name='CreditCard' and xtype='U')
	drop table CreditCard;


CREATE TABLE CreditCard
(
	CreditCardNumber BIGINT,
	ExpirationDt DATETIME,
	CreditCardLimit float,
	Cvv CHAR(3),
	Holder VARCHAR(50),
	Flag VARCHAR(50),
	CONSTRAINT PK_CREDITCARD PRIMARY KEY (CreditCardNumber),
)

CREATE TABLE Account
(
	AccountNumber VARCHAR(10),
	AgencyNumber VARCHAR(255),
	Restriction BIT,
	CreditCardNumber BIGINT,
	Overdraft float,
	AccountProfile VARCHAR(8),
	CreatedDt DATETIME,
	Balance float,

	CONSTRAINT PK_ACCOUNT PRIMARY KEY (AccountNumber),
    CONSTRAINT FK_ACCOUNT_CREDITCARD FOREIGN KEY (CreditCardNumber) REFERENCES CreditCard(CreditCardNumber)
)

CREATE TABLE AccountHistory
(
	AccountNumber VARCHAR(10),
	AgencyNumber VARCHAR(255),
	Restriction BIT,
	CreditCardNumber BIGINT,
	Overdraft float,
	AccountProfile VARCHAR(8),
	CreatedDt DATETIME,
	Balance float,

	CONSTRAINT PK_ACCOUNTHISTORY PRIMARY KEY (AccountNumber),
    CONSTRAINT FK_ACCOUNTHISTORY_CREDITCARD FOREIGN KEY (CreditCardNumber) REFERENCES CreditCard(CreditCardNumber)
)


CREATE TABLE ClientAccount
(
    AccountNumber VARCHAR(10),
    ClientCPF VARCHAR(14),

    CONSTRAINT PK_CLIENTACCOUNT PRIMARY KEY (AccountNumber, ClientCPF),
    CONSTRAINT FK_CLIENTACCOUNT_ACCOUNT FOREIGN KEY (AccountNumber) REFERENCES Account(AccountNumber)
)


CREATE TABLE AccountTransaction
(
	Id INT IDENTITY(1,1),
    AccountNumber VARCHAR(10),
	TransactionDt DATETIME,
	TransactionType varchar(8),
	ReceiverAccount VARCHAR(10) NULL,
	TransactionValue float,

	CONSTRAINT PK_TRANSACTION PRIMARY KEY (Id),
    CONSTRAINT FK_TRANSACTION_ACCOUNT FOREIGN KEY (AccountNumber) REFERENCES Account(AccountNumber),
    CONSTRAINT FK_TRANSACTION_RECEIVER FOREIGN KEY (ReceiverAccount) REFERENCES Account(AccountNumber)
)
