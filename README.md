# CoopEducation Backend

Backend API สำหรับระบบ **CoopEducation** ระบบจัดการข้อมูลและกระบวนการสหกิจศึกษา พัฒนาด้วย **ASP.NET Core Web API** โดยเชื่อมต่อฐานข้อมูล Microsoft SQL Server และใช้ Supabase Storage สำหรับจัดเก็บไฟล์เอกสาร

## 🛠️ Tech Stack

| Technology                   | Usage                          |
| ---------------------------- | ------------------------------ |
| .NET 10                      | Backend Framework              |
| ASP.NET Core Web API         | REST API                       |
| C#                           | Programming Language           |
| Microsoft SQL Server (MSSQL) | Database                       |
| Entity Framework Core        | ORM / Database Access          |
| Supabase Storage             | File Storage                   |
| JWT / Token                  | Authentication & Authorization |
| BCrypt                       | Password Hashing               |

## 📌 Features

Backend รองรับการทำงานหลักของระบบสหกิจศึกษา เช่น

* 🔐 User Authentication
* 👨‍🎓 Student Management
* 👨‍🏫 Teacher Management
* 🏢 Company Management
* 👨‍💼 Mentor Management
* 📚 Cooperative Education Placement Management
* 📅 Appointment Management
* 📄 Document & Form Management
* 👥 User & Role Management
* 📁 Document/File Upload และจัดเก็บผ่าน Supabase Storage
* 🔑 Access Token และ Refresh Token
* 📝 API Logging

## 🏗️ Project Structure

```text
CoopEducation/
│
├── .github/
│   └── workflows/
│
├── CoopEducation/
│   │
│   ├── Controllers/
│   │   ├── Appointment/
│   │   ├── Company/
│   │   ├── CoopPlacements/
│   │   ├── DocAndForm/
│   │   ├── Login/
│   │   ├── Student/
│   │   ├── Teacher/
│   │   └── User/
│   │
│   ├── Models/
│   │   ├── Constant/
│   │   ├── DTO/
│   │   ├── Request/
│   │   ├── Response/
│   │   │
│   │   ├── CoopEducationDbContext.cs
│   │   ├── Student.cs
│   │   ├── Teacher.cs
│   │   ├── Company.cs
│   │   ├── Mentor.cs
│   │   ├── CoopPlacement.cs
│   │   ├── StudentDocument.cs
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── RefreshToken.cs
│   │   └── ...
│   │
│   ├── Services/
│   │   ├── AllServices.cs
│   │   ├── DocumentService.cs
│   │   ├── StudentService.cs
│   │   └── TokenService.cs
│   │
│   ├── Properties/
│   │   └── launchSettings.json
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── CoopEducation.csproj
│
├── CoopEducation.slnx
├── .gitignore
└── .gitattributes
```

## 📂 Architecture

โปรเจกต์แบ่งโครงสร้างหลักออกเป็น 3 ส่วน

### Controllers

รับ HTTP Request จาก Client และจัดการ API Endpoint ของแต่ละ Module

```text
Controllers
├── Appointment
├── Company
├── CoopPlacements
├── DocAndForm
├── Login
├── Student
├── Teacher
└── User
```

### Models

จัดการ Entity, Database Context และ Data Transfer Objects

```text
Models
├── Constant
├── DTO
├── Request
├── Response
└── Entity Models
```

### Services

แยก Business Logic และการทำงานที่ใช้ร่วมกันออกจาก Controller

```text
Services
├── AllServices
├── DocumentService
├── StudentService
└── TokenService
```

## 🗄️ Database

ระบบใช้ **Microsoft SQL Server (MSSQL)** เป็นฐานข้อมูลหลัก และใช้ **Entity Framework Core** สำหรับติดต่อและจัดการข้อมูลผ่าน `CoopEducationDbContext`

ตัวอย่าง Entity หลัก:

```text
User
Role
Student
Teacher
Company
Mentor
CoopPlacement
StudentDocument
TeacherDocument
DocumentType
Advisorship
SupervisionAppointment
RefreshToken
...
```

## 📁 File Storage

ไฟล์เอกสารของระบบจัดเก็บผ่าน **Supabase Storage**

การทำงานโดยรวม:

```text
Client
   │
   │ Upload File
   ▼
ASP.NET Core API
   │
   │
   ▼
Supabase Storage
   │
   └── Document Files
```

Backend มี `DocumentService` สำหรับจัดการการทำงานเกี่ยวกับไฟล์และเอกสาร

## 🔐 Authentication

ระบบมีการจัดการ Authentication และ Token โดยใช้

* Access Token
* Refresh Token
* Password Hashing
* Role-based access control

ส่วนที่เกี่ยวข้องกับ Token ถูกแยกไว้ใน

```text
Services/
└── TokenService.cs
```

และมี Model สำหรับจัดการ Refresh Token:

```text
Models/
└── RefreshToken.cs
```

## 🚀 Getting Started

### Prerequisites

ก่อนเริ่มต้นใช้งาน จำเป็นต้องติดตั้ง:

* .NET 10 SDK
* Microsoft SQL Server
* Git
* Supabase Project

### 1. Clone Repository

```bash
git clone https://github.com/AlaNo778/CoopEducationBackend.git
cd CoopEducation
```

### 2. Configure Application Settings

สร้างหรือแก้ไข Configuration ให้ตรงกับ Environment ของคุณ

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<YOUR_MSSQL_CONNECTION_STRING>"
  },
  "Supabase": {
    "Url": "<YOUR_SUPABASE_URL>",
    "Key": "<YOUR_SUPABASE_KEY>"
  }
}
```

> **Important:** ห้าม Commit Connection String, Supabase Key, JWT Secret หรือข้อมูลลับอื่น ๆ ลงใน Git Repository

แนะนำให้ใช้ Environment Variables หรือ User Secrets สำหรับข้อมูลที่เป็นความลับ

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build Project

```bash
dotnet build
```

### 5. Run Backend

```bash
dotnet run
```

API จะเริ่มทำงานตาม URL ที่กำหนดไว้ใน `launchSettings.json`

## 🔄 Backend Workflow

ภาพรวมการทำงานของ Backend:

```text
Client / Frontend
       │
       │ HTTP Request
       ▼
ASP.NET Core Web API
       │
       ├── Controllers
       │
       ├── Services
       │
       ├── Entity Framework Core
       │
       ▼
   MSSQL Database
       
       │
       │ File Operations
       ▼
 Supabase Storage
```

## 📡 API Modules

API ถูกแบ่งตามหน้าที่ของระบบออกเป็น Module หลัก:

| Module         | Description                          |
| -------------- | ------------------------------------ |
| Login          | Authentication                       |
| Student        | Student information & management     |
| Teacher        | Teacher information & management     |
| Company        | Company & mentor management          |
| CoopPlacements | Cooperative education placement      |
| Appointment    | Appointment & supervision scheduling |
| DocAndForm     | Documents and forms                  |
| User           | User, role and major management      |

## 🧪 Development

สำหรับการพัฒนา Backend สามารถใช้ IDE/Editor เช่น:

* Visual Studio
* Visual Studio Code

คำสั่งที่ใช้บ่อย:

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Clean
dotnet clean
```

## 📌 Project Status

> 🚧 This project is currently under development.

## 👨‍💻 Author

**Aslan Samae**

Computer Science
Prince of Songkla University
