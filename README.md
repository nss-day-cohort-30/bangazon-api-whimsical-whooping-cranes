# Building the Bangazon Platform API


Hello, new users! Wlcome to Bangazon Platform API. 
This API displays the inner workings of Bangazon, a customer driven application to sell products. 

Set Up: 

You will need a Windows machine or a virtual Windows Machine to run this API.
You will need Visual Studio 2019 (to review the code)
You will need Azure Data Studio to create your Bangazon database
You will need PostMan to query and view the Bangazon API

1. git clone git@github.com:nss-day-cohort-30/bangazon-api-whimsical-whooping-cranes.git
2. cd into bangazon-api-whimsical-whooping-cranes
3. Start BangazonAPI.sln (this will open up your visual studio to review the code)
4. Open Azure data studio, copy and paste the first part of the script below. 
5. Change your Bangazon server connection to Bangazon
6. Copy and paste the second part of the script below (this will create the tables and insert values into the tables)
7. From this point on, you will copy and paste the delete tables and drop table constraints so your tables are recreated each time you      restart your Azure.
8. Run BangazonAPI within Visual Studio (in your terminal, you will see that your local host is port 5000
9. Open up Postman, your Postman query will be for the post listed above. Here is an example of looking up employees        http://localhost:5000/employees

Your job is to build out a .NET Web API that makes each resource in the Bangazon ERD available to application developers throughout the entire company.

1. Products
1. Product types
1. Customers
1. Orders
1. Payment types
1. Employees
1. Computers
1. Training programs
1. Departments


## Database Management

You will be using the [Official Bangazon SQL](./bangazon.sql) file to create your database. Create the database using SSMS, create a new SQL script for that database, copy the contents of the SQL file into your script, and then execute it.

## Controllers


ComputersController: Full CRUD, allows user to post, get(all and single), put (edit), and delete computers from the database.

ProductsController: Full CRUD, allows users to post a new product, get all products, get a single product, delete a product, and edit an product form the database.
na
PaymentTypeController: FULL CRUD, allows user to post, get all, get single, put, and delete PaymentTypes from the database

DepartmentController:  Get all departments, query for all departments to include employees of that department, query to see departments that budget is greater than or equal to 300000, get single department, post new department, edit existing department, and check if product exists.  

## Test Classes

ComputersTestingClass: testing class that allows user to test methods for posting, getting(all and single), putting, and deleting computers from the database. Ensures ComputersController methods are functioning properly.

TestingProductsClass: class that tests if the methods for getting all products, getting a single product, creating, editing, and deleting a product, deleting a non existing product, and getting a nonexistint product are functioning properly.

PaymentTypeTestingClass: testing class that allows user to test methods for posting, getting all, getting single, posting, and delete PaymentType from database. Also contains method to test 'get single' for non existant PaymentType. 

DepartmentTestingClass: testing class that allows user to test the get all departments method, modify a department method, post a department method, and get a nonexistant department method.

CODE TO COPY AND PASTE INTO AZURE TO CREATE BANGAZON API

*******FIRST SECTION TO COPY AND PASTE*********
 USE MASTER
GO

IF NOT EXISTS (
    SELECT [name]
    FROM sys.databases
    WHERE [name] = N'Bangazon'
)
CREATE DATABASE Bangazon
GO

USE Bangazon
GO 


** YOU WILL ONLY COPY AND PASTE THIS ONCE YOUR DATABASE HAS ALREADY BEEN CREATED 
******

DELETE FROM OrderProduct;
DELETE FROM ComputerEmployee;
DELETE FROM EmployeeTraining;
DELETE FROM Employee;
DELETE FROM TrainingProgram;
DELETE FROM Computer;
DELETE FROM Department;
DELETE FROM [Order];
DELETE FROM PaymentType;
DELETE FROM Product;
DELETE FROM ProductType;
DELETE FROM Customer;
ALTER TABLE Employee DROP CONSTRAINT [FK_EmployeeDepartment];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Employee];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Computer];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Employee];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Training];
ALTER TABLE Product DROP CONSTRAINT [FK_Product_ProductType];
ALTER TABLE Product DROP CONSTRAINT [FK_Product_Customer];
ALTER TABLE PaymentType DROP CONSTRAINT [FK_PaymentType_Customer];
ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Customer];
ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Payment];
ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Product];
ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Order];
DROP TABLE IF EXISTS OrderProduct;
DROP TABLE IF EXISTS ComputerEmployee;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Department;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS PaymentType;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS ProductType;
DROP TABLE IF EXISTS Customer;

*****SECOND SECTION TO COPY AND PASTE****

CREATE TABLE Department (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(55) NOT NULL,
    Budget  INTEGER NOT NULL
);
CREATE TABLE Employee (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(55) NOT NULL,
    LastName VARCHAR(55) NOT NULL,
    DepartmentId INTEGER NOT NULL,
    IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);
CREATE TABLE Computer (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    PurchaseDate DATETIME NOT NULL,
    DecomissionDate DATETIME,
    Make VARCHAR(55) NOT NULL,
    Manufacturer VARCHAR(55) NOT NULL
);
CREATE TABLE ComputerEmployee (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    EmployeeId INTEGER NOT NULL,
    ComputerId INTEGER NOT NULL,
    AssignDate DATETIME NOT NULL,
    UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);
CREATE TABLE TrainingProgram (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(255) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    MaxAttendees INTEGER NOT NULL
);
CREATE TABLE EmployeeTraining (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    EmployeeId INTEGER NOT NULL,
    TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);
CREATE TABLE ProductType (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(55) NOT NULL
);
CREATE TABLE Customer (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(55) NOT NULL,
    LastName VARCHAR(55) NOT NULL
);
CREATE TABLE Product (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    ProductTypeId INTEGER NOT NULL,
    CustomerId INTEGER NOT NULL,
    Price MONEY NOT NULL,
    Title VARCHAR(255) NOT NULL,
    [Description] VARCHAR(255) NOT NULL,
    Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);
CREATE TABLE PaymentType (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    AcctNumber INTEGER NOT NULL,
    [Name] VARCHAR(55) NOT NULL,
    CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);
CREATE TABLE [Order] (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    CustomerId INTEGER NOT NULL,
    PaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);
CREATE TABLE OrderProduct (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    OrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);
INSERT INTO Customer
VALUES
    ('Young', 'Thug'),
    ('Mozart', 'Mozart'),
    ('Ghostface', 'Killah'),
    ('Ryan', 'Adams'),
    ('The', 'Monkees');
INSERT INTO Department
VALUES
    ('Honky Horns', 300),
    ('Fast Bikes', 20000),
    ('Angry Pets', 5),
    ('Kanye Shoes', 900),
    ('Moon Real Estate', 12);
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('02/12/2016', '01/30/2019', 'Mojave', 'Apple');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('07/12/2015', '01/30/2019', 'Macbook Pro', 'Apple');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('08/22/2014', '11/10/2018', 'Zenbook', 'Asus');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('02/12/2013', '12/25/2017', 'G Series', 'Dell');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('04/12/2012', '05/23/2018', 'Envy', 'HP');
INSERT INTO Employee (FirstName, LastName, IsSupervisor, DepartmentId) VALUES
('Mo', 'McMo', 'False', 1),
('Abbey', 'McAbbey', 'False', 2),
('Katerina', 'McKaterina', 'False', 3),
('Janet', 'McJanet', 'True', 4),
('Ryan', 'McRyan', 'True', 5);
INSERT INTO ProductType
VALUES
    ('James Farrel Parephernalia'),
    ('Polka Rap Fusion Albums'),
    ('Musical Bears'),
    ('Blocks'),
    ('Lumps');
INSERT INTO PaymentType(AcctNumber, [Name], CustomerId) VALUES(1, 'Johns BigBank', 1)
INSERT INTO PaymentType(AcctNumber, [Name], CustomerId) VALUES(2, 'BettysCard', 2)
INSERT INTO PaymentType(AcctNumber, [Name], CustomerId) VALUES(3, 'JoannasVisa', 3)
INSERT INTO PaymentType(AcctNumber, [Name], CustomerId) VALUES(4, 'MickysMastercard', 4)
INSERT INTO PaymentType(AcctNumber, [Name], CustomerId) VALUES(5, 'MyVisa', 5)
INSERT INTO Product (ProductTypeId, Price, Title, [Description], Quantity, CustomerId) VALUES (1, 2500, 'Happy-Shirt', 'Tshirt', 1, 1 );
INSERT INTO Product (ProductTypeId, Price, Title, [Description], Quantity, CustomerId) VALUES (2, 19, 'WatchMeNow', 'New Rap Album Featuring Polka', 1, 2 );
INSERT INTO Product (ProductTypeId, Price, Title, [Description], Quantity, CustomerId) VALUES (3, 45, 'HappyBears', 'Cuty fluffy bears that sing to you', 5, 3 );
INSERT INTO Product (ProductTypeId, Price, Title, [Description], Quantity, CustomerId) VALUES (4, 80, 'Buddy-Build-A-Thing', 'Colorful blocks that your child can create things with', 2, 5 );
INSERT INTO Product (ProductTypeId, Price, Title, [Description], Quantity, CustomerId) VALUES (4, 150, 'Buddy-Build-A-Thing-Deluxe', 'Build a village!  The biggest assortment of colorful blocks yet', 1, 5 );
INSERT INTO ComputerEmployee VALUES (1, 2, '09/08/2017', '1/30/2019');
INSERT INTO ComputerEmployee VALUES (2, 3, '09/29/2015', '11/10/2018');
INSERT INTO ComputerEmployee VALUES (5, 1, '07/01/2016', '1/30/2017');
INSERT INTO ComputerEmployee VALUES (4, 2, '06/13/2018', '12/31/2018');
INSERT INTO ComputerEmployee VALUES (5, 1, '09/08/2017', '04/30/2019');
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) Values ('YeezyBreezy', 01/01/01, 12/12/99, 2);
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) Values ('How To Be a Good Person 101', 11/14/19, 01/12/20, 134);
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) Values ('Crochet Mastery', 01/01/01, 12/12/99, 54);
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) Values ('Team Relationship Building', 05/29/19, 06/12/19, 99);
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) Values ('How To Build Client Relationships', 11/01/21, 12/12/21, 776);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (1, 5);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (2, 4);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (3, 3);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (4, 2);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (5, 1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) Values
(1,1),
(2,2),
(3,3),
(4,4),
(5,5);
INSERT INTO OrderProduct
VALUEs
(1,5),
(2,4),
(3,3),
(4,2),
(5,1);
