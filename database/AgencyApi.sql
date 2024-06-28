CREATE DATABASE [UpBank.AgencyAPI];
use [UpBank.AgencyAPI];
-- Agency Table

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agency](
	[Number] [nvarchar](450) NOT NULL,
	[AddressId] [nvarchar](max) NOT NULL,
	[CNPJ] [nvarchar](max) NOT NULL,
	[Restriction] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[Agency] ADD  CONSTRAINT [PK_Agency] PRIMARY KEY CLUSTERED 
(
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- DeletedAgency table'

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgencyDeleted](
	[Number] [nvarchar](450) NOT NULL,
	[AddressId] [nvarchar](max) NOT NULL,
	[CNPJ] [nvarchar](max) NOT NULL,
	[Restriction] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[AgencyDeleted] ADD  CONSTRAINT [PK_AgencyDeleted] PRIMARY KEY CLUSTERED 
(
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


-- Employee Table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[CPF] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[BirthDt] [datetime2](7) NOT NULL,
	[Sex] [nvarchar](1) NOT NULL,
	[AddressId] [nvarchar](max) NOT NULL,
	[Salary] [float] NOT NULL,
	[Phone] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Manager] [bit] NOT NULL,
	[Registry] [int] NOT NULL,
	[AgencyNumber] [nvarchar](450) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[CPF] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Employee_AgencyNumber] ON [dbo].[Employee]
(
	[AgencyNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Agency_AgencyNumber] FOREIGN KEY([AgencyNumber])
REFERENCES [dbo].[Agency] ([Number])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Agency_AgencyNumber]
GO


-- DeletedEmployee Table

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeDeleted](
	[CPF] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[BirthDt] [datetime2](7) NOT NULL,
	[Sex] [nvarchar](1) NOT NULL,
	[AddressId] [nvarchar](max) NOT NULL,
	[Salary] [float] NOT NULL,
	[Phone] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Manager] [bit] NOT NULL,
	[Registry] [int] NOT NULL,
	[AgencyNumber] [nvarchar](450) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[EmployeeDeleted] ADD  CONSTRAINT [PK_EmployeeDeleted] PRIMARY KEY CLUSTERED 
(
	[CPF] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_EmployeeDeleted_AgencyNumber] ON [dbo].[EmployeeDeleted]
(
	[AgencyNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EmployeeDeleted]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDeleted_Agency_AgencyNumber] FOREIGN KEY([AgencyNumber])
REFERENCES [dbo].[Agency] ([Number])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmployeeDeleted] CHECK CONSTRAINT [FK_EmployeeDeleted_Agency_AgencyNumber]
GO