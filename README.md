# 🚀 Service Marketplace Platform (MVP)

A fullstack Service Marketplace platform built as part of a Senior Fullstack Engineer take-home assignment.

---

## 👨‍💻 Author
**Name:** Eng : Omar Mohmed  
**Email:** omerrooali@gmail.com  
**Phone:** 0551576803  

---

## 🔗 GitHub Repository
👉 https://github.com/Omeroali/ServiceMarketplaceTask

---

## 📌 Overview

This project is a simplified Service Marketplace where:

- Customers create service requests  
- Providers view and accept nearby requests  
- Role-Based Access Control (RBAC) is enforced at API level  
- Subscription limits are applied  
- Includes a simple AI-powered feature  

---

## 🏗️ Architecture (Clean Architecture)

The solution is structured as:

- **API** → Entry point (Controllers)
- **Application** → Business logic (CQRS + MediatR)
- **Domain** → Entities & Enums
- **Infrastructure** → Database + Identity + Services
- **WebUI** → Frontend (ASP.NET MVC)

---

## 🔐 Authentication & RBAC

- JWT Authentication  
- Roles:
  - Admin
  - Provider
  - Customer  

- Dynamic Permissions (stored in DB):
  - request.create
  - request.accept
  - request.complete
  - request.view_all  

Permissions are injected into JWT and enforced at API level.

---

## ⚙️ Core Features

### 👤 Customer
- Create service request  
- View own requests  
- Limited to **3 requests (Free plan)**  

---

### 🛠️ Provider
- View nearby requests  
- Accept request  
- Complete request  

---

### 🔄 Request Lifecycle
Pending → Accepted → Completed

---

## 🌍 Geolocation

- Each request includes:
  - Latitude
  - Longitude  

- Providers can filter nearby requests using coordinates.

---

## 💳 Subscription (Simulated)

- Free users → Max 3 requests  
- Admin → Unlimited  
- Implemented via flag in database  

---

## 🤖 AI Feature

- Automatically enhances request description  
- Simulated logic (can be replaced with real AI API)

---

## 🖥️ Frontend

### 🔑 Login / Register
- Clean centered UI  
- Show/Hide password  

---

### 👤 Customer Dashboard
- Create request  
- View own requests  

---

### 🛠️ Provider Dashboard
- Load nearby requests  
- Accept / Complete requests  

---

### 🛡️ Admin Dashboard
- Assign roles to users  
- Navigate to:
  - Customer page (create requests)
  - Provider page (manage requests)

---

## 👑 Admin Credentials

Email: admin@site.com  
Password: P@ssw0rd  

---

## 🔌 API Endpoints (Short Explanation)

### 🔐 Auth

**POST /api/Auth/register**  
→ Creates new user account  

**POST /api/Auth/login**  
→ Returns JWT token with roles & permissions  

---

### 📦 Requests

**GET /api/Requests?lat=&lng=**  
→ Get requests filtered by location  

**POST /api/Requests**  
→ Create new request (Customer only)  

**POST /api/Requests/{id}/accept**  
→
