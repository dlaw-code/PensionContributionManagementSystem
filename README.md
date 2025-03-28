Pension Contribution Management System

Overview
A  .NET Core solution for managing pension contributions, member data, and benefit calculations following Clean Architecture and Domain-Driven Design principles.

Key Features

Member Management: Registration, updates, retrieval, and soft-delete functionality

Contribution Processing:

Monthly and voluntary contribution handling

Automated statement generation

Business rule enforcement

Background Processing:

Contribution validation

Benefit eligibility updates

Interest calculations

Comprehensive Validation:

Member details verification

Contribution validation

Employer registration checks

Technical Stack
Framework: .NET 7+

Database: SQL Server with Entity Framework Core

Architecture: Clean Architecture + DDD

Background Jobs: Hangfire


Getting Started

Prerequisites
.NET 7 SDK

SQL Server 2019+

Hangfire server (for background jobs)

Setup
Clone the repository

Configure connection strings in appsettings.json

Run database migrations


Design Decisions
Clean Architecture: Separation of concerns with clear layer boundaries

DDD Patterns:

Aggregates for Member/Contribution relationships

Domain events for business process triggers

Repository Pattern: Abstracted data access for testability

