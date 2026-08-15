# Library Management System API

A Web API project for managing a library (books, authors, members, borrowing and returning) built with ASP.NET Core.

## Tech Stack

ASP.NET Core 8 Web API
Entity Framework Core
SQL Server
JWT Authentication
Swagger

## Features

Full CRUD for Books, Authors, Publishers, Categories, and Members
User registration and login with roles (Admin, Librarian, Staff)
Borrow and return books
Search for a book by name, author, or category
Filter books by status (available or borrowed)

## How to Run

1. Open appsettings.json and update the connection string with your server info:

Server=SERVER_NAME;Database=books;Trusted_Connection=True;TrustServerCertificate=True;

2. Open Package Manager Console and run:

Update-Database

3. Run the project (F5)

4. Swagger will open automatically and you can test the endpoints from there.

## How to Use

1. Register a new account using POST /api/auth/register and choose a Role (Administrator, Librarian, or Staff)
2. Log in using POST /api/auth/login to get a Token
3. In Swagger, click Authorize and type: Bearer YOUR_TOKEN_HERE
4. Now you can test any other endpoint

## Roles and Permissions

Administrator: view, add, edit, delete
Librarian: view, add, edit
Staff: view, add/edit members and borrowing only

## Main Endpoints

/api/books
/api/authors
/api/publishers
/api/categories
/api/members
/api/borrowing/borrow
/api/borrowing/return/{id}
/api/books/search?name=
/api/books/status/{status}
