# CRM Application

- **App**: Visit https://crm.fulfill3d.com to see the application.
- **Frontend**: Visit https://github.com/fulfill3d/CRM_Frontend to see the frontend repo.
- **Docs**: Visit https://fulfill3d.com/projects/eee2bdbd-e460-46b9-b793-5b3c9a02e98a to see the project wiki.

A comprehensive Customer Relationship Management (CRM) application designed for businesses to manage their stores, employees, and services across multiple locations. The app also enables clients to discover nearby services and book appointments.

## Table of Contents

1. [Introduction](#introduction)
2. [Microservices](#microservices)
3. [Tech Stack](#tech-stack)

## Introduction

The CRM Application provides a platform for businesses to efficiently manage their operations across multiple locations. It facilitates the addition of stores, employee management, and service offerings. Clients can easily find and book services, ensuring a seamless experience.

## Microservices

### Business
- **CRM.API.Business.Identity:** Register your business on the platform.
- **CRM.API.Business.Appointment:** Manage appointments in various stores.
- **CRM.API.Business.Employee:** Assign and manage employees for each store.
- **CRM.API.Business.Store:** Add and manage stores in various locations.
- **CRM.API.Business.Service:** Define and categorize services offered at each store.

### Client
- **CRM.API.Client.Identity:** Clients can register and create profiles.
- **CRM.API.Client.Service:** Find services based on location and other filters.
- **CRM.API.Client.Appointment:** Book appointments with businesses for various services.

## Tech Stack

- **Backend:** .NET 8 (Isolated Worker), Azure Functions v4
- **Database:** Azure SQL Server
- **Database Migration:** Fluent Migrator
- **Authentication:** Azure AD B2C
- **Configuration & Secrets Management:** Azure App Configuration, Azure Key Vault
- **API Documentation:** OpenAPI
- **Hosting:** Microsoft Azure
