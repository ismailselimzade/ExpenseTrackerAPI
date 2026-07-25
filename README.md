# Expense Tracker API

A RESTful API for tracking personal expenses with JWT authentication.  
JWT autentifikasiyası ilə şəxsi xərcləri izləmək üçün RESTful API.

---

## Features / Xüsusiyyətlər

- User registration and login with JWT authentication.  
  JWT ilə istifadəçi qeydiyyatı və girişi.

- Add, update, and delete expenses.  
  Xərc əlavə etmə, yeniləmə və silmə.

- List expenses with filters: last month, last 3 months, custom date range.  
  Xərcləri filtrlə: son ay, son 3 ay, xüsusi tarix aralığı.

- Expense categories: Groceries, Leisure, Electronics, Utilities, Clothing, Health, Others.  
  Xərc kateqoriyaları: Groceries, Leisure, Electronics, Utilities, Clothing, Health, Others.

- Each user can only access their own expenses.  
  Hər istifadəçi yalnız öz xərclərini görə bilər.

---

## What I Learned / Öyrəndiklərim

- How to build multiple optional query filters that combine into one dynamic database query.  
  Bir-birinə uyğunlaşan bir neçə optional sorğu filtrini necə bir dinamik veritabanı sorğusunda birləşdirmək olar.

---

## Tech Stack / Texnologiyalar

C#, ASP.NET Core, Entity Framework Core, SQL Server, JWT, BCrypt.Net

---

## Endpoints

### Auth

| Method | URL | Description |
|--------|-----|-------------|
| POST | `/api/auth/register` | Registers a new user. / Yeni istifadəçi qeydiyyatı. |
| POST | `/api/auth/login` | Returns a JWT token. / JWT token qaytarır. |

### Expenses

| Method | URL | Description |
|--------|-----|-------------|
| POST | `/api/expense` | Adds a new expense. / Yeni xərc əlavə edir. |
| PUT | `/api/expense` | Updates an expense. / Xərci yeniləyir. |
| DELETE | `/api/expense/{id}` | Deletes an expense. / Xərci silir. |
| GET | `/api/expense` | Returns all expenses. / Bütün xərcləri qaytarır. |
| GET | `/api/expense?filter=lastMonth` | Returns last month's expenses. / Son ayın xərclərini qaytarır. |
| GET | `/api/expense?filter=last3Month` | Returns last 3 months' expenses. / Son 3 ayın xərclərini qaytarır. |
| GET | `/api/expense?startDate=&endDate=` | Returns expenses in date range. / Tarix aralığındakı xərcləri qaytarır. |

---

## Setup / Quraşdırma

1. Clone the repository.  
   Repozitorini kopyalayın.

2. Add the connection string and JWT settings to `appsettings.json`.  
   `appsettings.json` faylına connection string və JWT parametrlərini əlavə edin.

3. Apply the database migrations.  
   Veritabanı miqrasiyalarını tətbiq edin.

    `dotnet ef database update`

4. Run the project.  
   Proyekti işə salın.

   `dotnet run`
