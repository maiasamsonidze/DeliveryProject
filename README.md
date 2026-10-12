Courier Delivery Management System

A backend RESTful API developed with C# and ASP.NET Core 8, designed to simplify and centralize courier delivery operations.

The system provides tools for managing customer orders, importing Excel files, assigning parcels to couriers, tracking delivery statuses, and communicating with customers through SMS notifications.

Key Features

Authentication & User Management

JWT-based authentication and authorization.

Refresh token functionality.

Role-based access control.

User registration and login.

Password hashing using BCrypt.

Password management and user administration.

Order Management

Import orders from Excel (.xlsx) files using ClosedXML.

Retrieve all orders or user-specific orders.

Manage parcel information and delivery addresses.

Update delivery addresses and parcel codes.

Delete orders and parcels.

Store geographic coordinates for delivery locations.

Courier & Parcel Management

Assign parcels to individual couriers.

Remove parcel assignments.

Retrieve courier-specific parcel information.

Update parcel delivery statuses.

Access customer contact information.

Retrieve parcel locations for map integration.

SMS Notifications

Send individual SMS notifications.

Send bulk SMS notifications.

Send delivery-related customer notifications.

Integrate with an external SMS service provider.

Real-Time Communication

SignalR integration for real-time communication.

Notify connected clients when delivery addresses are updated.

Technology Stack

Language: C#

Framework: ASP.NET Core 8 Web API

Database: Microsoft SQL Server

ORM: Entity Framework Core 8

Authentication: JWT & Refresh Tokens

Password Security: BCrypt

Excel Processing: ClosedXML

Real-Time Communication: SignalR

Object Mapping: AutoMapper

API Documentation: Swagger / OpenAPI

Architecture: Controller-Service pattern with dependency injection

Project Architecture

The application separates responsibilities into controllers, services, interfaces, DTOs, database models, and supporting components.

The API includes dedicated controllers for user management, order processing, courier assignments, and SMS notifications.

Project Purpose

The goal of this project is to improve courier delivery efficiency by reducing manual operations, centralizing order and parcel information, simplifying courier assignments, and supporting automated customer communication.
