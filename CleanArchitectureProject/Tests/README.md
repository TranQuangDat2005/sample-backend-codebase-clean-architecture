# Tests

Thư mục chứa toàn bộ các project kiểm thử cho ứng dụng, được tổ chức tương ứng với từng tầng của **Clean Architecture**.

---

## 🧪 Stack kiểm thử

| Công cụ | Vai trò |
| :--- | :--- |
| **xUnit** | Framework test chính (phổ biến nhất trong .NET ecosystem) |
| **NSubstitute** | Tạo mock/stub cho interfaces — cú pháp tự nhiên, dễ đọc hơn Moq |
| **FluentAssertions** | Viết assertion rõ ràng như ngôn ngữ tự nhiên (`x.Should().Be(y)`) |
| **EF Core InMemory** | Thay thế DB thật khi test tầng Infrastructure |
| **WebApplicationFactory** | Spin up toàn bộ API pipeline khi chạy Integration / E2E test |

---

## 📁 Cấu trúc thư mục

```
Tests/
├── Domain/            # Unit Test — Entities & Business Rules (không mock gì)
├── Application/       # Unit Test — Use Cases (mock IRepository bằng NSubstitute)
├── Infrastructure/    # Integration Test — Repository & DbContext (EF InMemory)
├── Api/               # Integration Test — Controllers & Endpoints (WebApplicationFactory)
└── E2E/               # End-to-End Test — Luồng hoàn chỉnh (không mock)
```

---

## 🔗 Cài NuGet cho mỗi project test

```bash
# Framework + assertion (dùng ở tất cả các layer)
dotnet add package xunit
dotnet add package FluentAssertions

# Mock (dùng ở Application.Tests)
dotnet add package NSubstitute

# DB in-memory (dùng ở Infrastructure.Tests)
dotnet add package Microsoft.EntityFrameworkCore.InMemory

# API test (dùng ở Api.Tests và E2E.Tests)
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

---

## ⚡ Chạy toàn bộ test

```bash
dotnet test
```

## ⚡ Chạy test theo thư mục cụ thể

```bash
dotnet test Tests/Domain/
dotnet test Tests/Application/
```
