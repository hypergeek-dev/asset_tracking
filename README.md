# Asset Tracker

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Database Configuration](#database-configuration)
- [Enums](#enums)
- [Classes](#classes)
- [License](#license)

## Overview

Asset Tracker is a console application written in C# that allows users to manage a list of products. The application uses Entity Framework Core to interact with a SQL Server database. Users can add new products, list all existing products, and exit the application.

## Features

- **Add a Product**: Users can add a new product by providing details like the type, brand, model, office location, purchase date, and price.
  
- **List All Products**: Lists all the products stored in the database, sorted by office location and purchase date.

- **Exit**: Exits the application.

## Prerequisites

- .NET SDK
- SQL Server

## Installation

1. Clone the repository or download the source code.
2. Open the solution in Visual Studio.
3. Build the solution to restore NuGet packages.
4. Update the SQL Server connection string in `AssetContext.cs` if needed.
5. Run the application.

## Usage

1. Run the application.
2. Follow the on-screen instructions to add a product or list all products.

## Database Configuration

The application uses Entity Framework Core to create a SQL Server database named `AssetTrackingDB`. The database contains a table called `Products`.

### Connection String

The connection string is configured in the `AssetContext` class in `AssetContext.cs`. Update the `Server` and `Database` fields as needed.

```csharp
optionsBuilder.UseSqlServer(@"Server=MAINCORE\\SQLEXPRESS;Database=AssetTrackingDB;Trusted_Connection=True;TrustServerCertificate=true;");

## Enums

- **OfficeLocation**: Specifies the office location where the product is located. Options are `Unknown`, `Spain`, `Sweden`, and `USA`.

- **ProductStatus**: Specifies the status of the product based on its age. Options are `Normal`, `Yellow`, and `Red`.

## Classes

- **Product**: Represents a product with properties like `Id`, `TypeOfProduct`, `Brand`, `Model`, `Office`, `Date`, and `Price`.

- **AssetContext**: Inherits from `DbContext` and is responsible for interacting with the database.

## License

This project is open source and available under the MIT License.
