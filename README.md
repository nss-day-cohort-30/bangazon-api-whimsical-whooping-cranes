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
7. From this point on, you will copy and paste the delete tables and drop table constraints so your tables are recreated each time you restart your Azure.
8. Run BangazonAPI within Visual Studio (in your terminal, you will see that your local host is port 5000
9. Open up Postman, your Postman query will be for the post listed above. Here is an example of looking up employees        http://localhost:5000/employees

Your job is to build out a .NET Web API that makes each resource in the Bangazon ERD available to application developers throughout the entire company.

1. Products
2. Product types
3. Customers
4. Orders
5. Payment types
6. Employees
7. Computers
8. Training programs
9. Departments


## Database Management

You will be using the [Official Bangazon SQL](./bangazon.sql) file to create your database. Create the database using SSMS, create a new SQL script for that database, copy the contents of the SQL file into your script, and then execute it.

## Controllers

ComputersController: Full CRUD, allows user to post, get(all and single), put (edit), and delete computers from the database.

CustomerController: Get allows users to get all customers. Include "?_include=products" with fetch call to get products customer is selling.

ProductsController: Full CRUD, allows users to post a new product, get all products, get a single product, delete a product, and edit an product form the database.
na
PaymentTypeController: FULL CRUD, allows user to post, get all, get single, put, and delete PaymentTypes from the database

## Test Classes

ComputersTestingClass: testing class that allows user to test methods for posting, getting(all and single), putting, and deleting computers from the database. Ensures ComputersController methods are functioning properly.

CustomersTestingClass: allows user to test methods for getting customers.

TestingProductsClass: class that tests if the methods for getting all products, getting a single product, creating, editing, and deleting a product, deleting a non existing product, and getting a nonexistint product are functioning properly.

## Customers:

