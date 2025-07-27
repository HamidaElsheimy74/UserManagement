# UserManagement
The **UserManagement API** is a robust solution for managing user, role and retrieval. It supports storing users, roles, user roles, and serving them efficiently with Database integration.

## Features
- **User Management**: Adding, updating, delete, get by id & get users list.
- **Roles Management**: Adding, updating, delete, get by id & get roles list.
- **user roles Management**: Adding, updating, delete, get by id & get user role list.
- **Error Handling**: Comprehensive error responses for invalid inputs or server issues.
- **Multi-Language**: (en/hi).
- **JWT authentication**

- ## Technologies
- **.NET 8**
- **C# 12**
- **ASP.NET Core web Api**
- **MSTest** for unit testing
- **Dependency Injection** for modularity and testability.
- **Sql Server** for saving user & roles database.
- **Entity FrameWork core** as ORM with code first approach.

## Installation
1. .NET 8 SDK.
2.  Visual Studio 2022.
3.  Clone the repository: https://github.com/HamidaElsheimy74/UserManagement.
4.  Navigate to the project directory.
5.  Restore dependencies.
6.  Build the project.

 ## Configuration
1- Update database with the db objects that's made inside the migration by writting the following command in PM: update-database.

2-Press F5 to start debugging & run the code.

3-Set launch profile to Development & use these credintial to login & get Access/Refresh tokens to be able to use the apis.

	 {
		 "userName": "admin@gmail.com",
		 "password": "Admin@1234"
	 }
  
- **Note 1**: Admin role is the only one allowed to manage Users & roles.
- **Note 1**: Login, Register & RefreshTokens are allowed by Anonymous users.
- **Note 2**: DB is Seeded by both user & Admin roles to start working with the Apis.
 


