# Clean Architecture Project (.NET 10)

Dự án mẫu triển khai theo kiến trúc **Clean Architecture** (Onion Architecture / Hexagonal Architecture) trên nền tảng **.NET 10** và **ASP.NET Core Web API**.

---

## 🏛️ Tổng quan Kiến trúc

Mục tiêu cốt lõi của Clean Architecture là tách biệt các mối quan tâm (Separation of Concerns), giúp hệ thống:
- **Độc lập với Framework**: Framework chỉ đóng vai trò công cụ, không chi phối logic nghiệp vụ cốt lõi.
- **Dễ dàng kiểm thử (Testable)**: Logic nghiệp vụ có thể được kiểm thử mà không cần UI, Database, Web Server.
- **Độc lập với UI**: Giao diện có thể thay đổi (Web API, gRPC, CLI) mà không ảnh hưởng tầng dưới.
- **Độc lập với Database**: Có thể chuyển đổi SQL Server, PostgreSQL, MongoDB mà không làm thay đổi Domain/Application.

```
       +---------------------------------------------+
       |             Presentation (UI/API)           |
       |                     +                       |
       |               Infrastructure                |
       |                     |                       |
       |                     v                       |
       |             [ Application ]                 |
       |                     |                       |
       |                     v                       |
       |                [ Domain ]                   |
       +---------------------------------------------+
```

### Quy tắc phụ thuộc (Dependency Rule)
Các tầng bên ngoài phụ thuộc vào các tầng bên trong. **Domain** không phụ thuộc vào bất kỳ tầng nào. **Application** chỉ phụ thuộc vào **Domain**.

---

## 📁 Cấu trúc thư mục

```
CleanArchitectureProject/
├── CleanArchitectureProject.slnx          # Solution file
└── CleanArchitectureProject/              # Project chính (Monolithic Layered)
    ├── Domain/                            # Tầng cốt lõi: Thực thể & Quy tắc nghiệp vụ
    │   ├── Entities/                      # Các thực thể nghiệp vụ (Entities)
    │   ├── Exceptions/                    # Ngoại lệ đặc thù của Domain
    │   └── ValueObjects/                  # Các đối tượng giá trị (Value Objects)
    ├── Application/                       # Tầng ứng dụng: Use cases & Interfaces
    │   ├── DTOs/                          # Data Transfer Objects
    │   ├── Interfaces/                    # Hợp đồng giao tiếp (Repository, Services)
    │   └── Services/                      # Triển khai Use Cases & Logic điều phối
    ├── Infrastructure/                    # Tầng hạ tầng: Database, External Services
    │   └── Persistence/                   # Truy xuất dữ liệu (EF Core)
    │       ├── Configurations/            # Fluent API Entity configurations
    │       ├── Migrations/                # EF Core migrations
    │       └── Repositories/              # Triển khai Repository
    ├── Presentation/                      # Tầng hiển thị: API Controllers & Pipeline
    │   ├── Controllers/                   # Web API Controllers
    │   ├── Middlewares/                   # Custom middlewares (Xử lý lỗi, Logging,...)
    │   ├── Program.cs                     # Cấu hình Dependency Injection & Pipeline
    │   └── appsettings.json               # Cấu hình ứng dụng
    └── Properties/                        # Thiết lập môi trường chạy (launchSettings.json)
```

---

## 📋 Cấu trúc Mẫu Dự Án Thực Tế (StudentManagement)

Dưới đây là cấu trúc mẫu **tách riêng từng project** (khuyến nghị cho dự án thực tế), có chú thích chi tiết về **NuGet packages**, **tham chiếu project** và **vai trò của từng file**:

```
StudentManagement/
│
├── src/
│   │
│   ├── 1. StudentManagement.Domain/                 # 🟢 LÕI NGHIỆP VỤ
│   │   │   # (KHÔNG cài bất kỳ NuGet package nào. Code C# thuần)
│   │   ├── Entities/
│   │   │   └── Student.cs                           # POCO Class, không có [Table], [Key]
│   │   ├── ValueObjects/
│   │   │   └── Address.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs                   # Lỗi liên quan đến quy tắc nghiệp vụ
│   │
│   ├── 2. StudentManagement.Application/            # 🟡 USE CASES & HỢP ĐỒNG
│   │   │   # (Tham chiếu: Project Domain)
│   │   │   # (Tuyệt đối KHÔNG cài EF Core ở đây)
│   │   ├── Interfaces/
│   │   │   ├── IStudentRepository.cs                # Port: Giao tiếp với Database
│   │   │   └── IUnitOfWork.cs                       # Port: Quản lý transaction (Commit/Rollback)
│   │   ├── DTOs/
│   │   │   └── StudentDto.cs                        # Object trả về cho API
│   │   └── UseCases/
│   │       └── CreateStudentUseCase.cs              # Chứa logic: Validate -> Add -> Save
│   │
│   ├── 3. StudentManagement.Infrastructure/         # 🔵 CÔNG CỤ & DATABASE (EF CORE)
│   │   │   # (Tham chiếu: Project Application, Domain)
│   │   │   # Cài NuGet: Microsoft.EntityFrameworkCore.SqlServer
│   │   │   # Cài NuGet: Microsoft.EntityFrameworkCore.Tools (để chạy Migration)
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs              # Kế thừa DbContext của EF Core
│   │   │   ├── Configurations/
│   │   │   │   └── StudentConfiguration.cs          # Fluent API (ToTable, HasKey, IsRequired...)
│   │   │   ├── Repositories/
│   │   │   │   └── StudentRepository.cs             # Implements IStudentRepository bằng _context
│   │   │   └── Migrations/                          # Thư mục EF Core tự sinh ra khi Add-Migration
│   │   └── DependencyInjection.cs                   # Extension Method đăng ký EF Core vào DI Container
│   │
│   └── 4. StudentManagement.Api/                    # 🔴 GIAO DIỆN (ASP.NET CORE WEB API)
│       │   # (Tham chiếu: Project Application, Infrastructure)
│       │   # Cài NuGet: Microsoft.EntityFrameworkCore.Design (Bắt buộc để CLI đọc được DbContext)
│       ├── Controllers/
│       │   └── StudentsController.cs                # HTTP POST, GET, PUT... gọi UseCase
│       ├── Middlewares/
│       │   └── ExceptionMiddleware.cs               # Bắt lỗi toàn cục (Global Error Handling)
│       ├── appsettings.json                         # Chứa Chuỗi kết nối DB (Connection String)
│       └── Program.cs                               # Nơi gắn kết (Wiring) tất cả lại với nhau
```

### So sánh hai cách tổ chức

| Tiêu chí | Dự án hiện tại (Monolithic Layered) | Mẫu StudentManagement (Multi-Project) |
| :--- | :--- | :--- |
| **Số lượng project** | 1 project duy nhất, tách tầng bằng folder | 4 projects riêng biệt trong 1 solution |
| **Kiểm soát phụ thuộc** | Quy ước (Convention) — dựa vào kỷ luật dev | Bắt buộc (Enforced) — compiler từ chối nếu sai |
| **Phù hợp với** | Dự án nhỏ, học tập, prototype nhanh | Dự án thực tế, làm việc nhóm, dài hạn |
| **NuGet EF Core** | Cài chung trong 1 project | Chỉ cài ở Infrastructure + Api (Design) |

> 💡 **Gợi ý**: Khi chuyển sang multi-project, mỗi project tự có file `.csproj` riêng với `<ProjectReference>` khai báo rõ ràng project nào phụ thuộc project nào. Compiler sẽ **từ chối biên dịch** nếu Domain cố `using` namespace của Infrastructure.

---

## 🧪 Cấu trúc Thư mục Tests (C# / xUnit)

> Mẫu tổ chức test cho dự án **StudentManagement** (Multi-Project). Với .NET, mỗi loại test thường là một **project `.csproj` riêng** nằm trong thư mục `tests/`.

### Stack kiểm thử

| Công cụ | Vai trò |
| :--- | :--- |
| **xUnit** | Framework test chính (phổ biến nhất trong .NET ecosystem) |
| **NSubstitute** | Tạo mock/stub cho interfaces (thay thế Moq, cú pháp tự nhiên hơn) |
| **FluentAssertions** | Viết assertion dễ đọc như ngôn ngữ tự nhiên |
| **EF Core InMemory / SQLite** | Thay DB thật khi test Infrastructure layer |
| **WebApplicationFactory** | Spin up toàn bộ API để chạy Integration & E2E test (built-in .NET) |

### Cấu trúc thư mục

```
StudentManagement/
│
├── src/                                          # Source code (như trên)
│
└── tests/                                        # 🧪 Tất cả test projects
    │
    ├── 1. StudentManagement.Domain.Tests/        # Unit Test — Entities & Business Rules
    │   │   # Tham chiếu: Project Domain
    │   │   # NuGet: xunit, FluentAssertions
    │   │   # KHÔNG mock gì cả — Domain phải test được mà không cần dependency
    │   ├── Entities/
    │   │   ├── StudentTests.cs                   # Kiểm thử constructor, guard clause, domain rule
    │   │   └── OrderTests.cs
    │   └── ValueObjects/
    │       └── AddressTests.cs
    │
    ├── 2. StudentManagement.Application.Tests/   # Unit Test — Use Cases (Logic nghiệp vụ)
    │   │   # Tham chiếu: Project Application, Domain
    │   │   # NuGet: xunit, NSubstitute, FluentAssertions
    │   │   # Mock toàn bộ IRepository, IUnitOfWork bằng NSubstitute
    │   └── UseCases/
    │       ├── CreateStudentUseCaseTests.cs      # Test flow: validate → add → save
    │       └── EnrollCourseUseCaseTests.cs
    │
    ├── 3. StudentManagement.Infrastructure.Tests/ # Integration Test — Repository & DB
    │   │   # Tham chiếu: Project Infrastructure, Application, Domain
    │   │   # NuGet: xunit, FluentAssertions, Microsoft.EntityFrameworkCore.InMemory
    │   │   # Dùng EF Core InMemory hoặc SQLite (:memory:) thay DB thật
    │   └── Persistence/
    │       ├── StudentRepositoryTests.cs         # Test Add/Get/Delete qua DbContext thật
    │       └── ApplicationDbContextTests.cs
    │
    ├── 4. StudentManagement.Api.Tests/           # Integration Test — Controllers & Endpoints
    │   │   # Tham chiếu: Project Api, Application
    │   │   # NuGet: xunit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing
    │   │   # Dùng WebApplicationFactory để spin up toàn bộ API pipeline
    │   └── Controllers/
    │       ├── StudentsControllerTests.cs        # Test HTTP POST /students trả về 201
    │       └── CoursesControllerTests.cs
    │
    └── 5. StudentManagement.E2E.Tests/           # End-to-End Test — Luồng hoàn chỉnh
        │   # NuGet: xunit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing
        │   # Test toàn bộ flow từ HTTP request → DB → HTTP response (không mock)
        └── Flows/
            ├── StudentEnrollmentFlowTests.cs     # Signup → Enroll → GetResult
            └── LoginFlowTests.cs
```

### Ví dụ Unit Test — Domain Layer

```csharp
// StudentManagement.Domain.Tests/Entities/StudentTests.cs
using FluentAssertions;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Exceptions;

public class StudentTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenNameIsEmpty()
    {
        // Arrange & Act
        Action act = () => new Student(id: 1, name: "", email: "test@test.com");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*tên*");
    }

    [Fact]
    public void Student_ShouldCreate_WhenDataIsValid()
    {
        // Act
        var student = new Student(id: 1, name: "Nguyễn Văn A", email: "a@test.com");

        // Assert
        student.Name.Should().Be("Nguyễn Văn A");
        student.IsActive.Should().BeTrue();
    }
}
```

### Ví dụ Unit Test — Application Layer (có Mock)

```csharp
// StudentManagement.Application.Tests/UseCases/CreateStudentUseCaseTests.cs
using FluentAssertions;
using NSubstitute;
using StudentManagement.Application.Interfaces;
using StudentManagement.Application.UseCases;
using StudentManagement.Domain.Entities;

public class CreateStudentUseCaseTests
{
    private readonly IStudentRepository _repository = Substitute.For<IStudentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Execute_ShouldAddStudent_WhenInputIsValid()
    {
        // Arrange
        var useCase = new CreateStudentUseCase(_repository, _unitOfWork);

        // Act
        await useCase.ExecuteAsync("Nguyễn Văn A", "a@test.com");

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<Student>(s => s.Name == "Nguyễn Văn A"));
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
```

### Ví dụ Integration Test — API Layer (WebApplicationFactory)

```csharp
// StudentManagement.Api.Tests/Controllers/StudentsControllerTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

public class StudentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StudentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_Students_ShouldReturn201_WhenValid()
    {
        // Arrange
        var payload = new { name = "Nguyễn Văn A", email = "a@test.com" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/students", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

---

## 🚀 Hướng dẫn Chạy Dự Án

### Yêu cầu tiên quyết
- **.NET 10 SDK** trở lên.
- Visual Studio 2026 / Visual Studio Code / JetBrains Rider.

### Lệnh thực thi (CLI)

1. **Khôi phục dependencies:**
   ```bash
   dotnet restore
   ```

2. **Build dự án:**
   ```bash
   dotnet build
   ```

3. **Chạy ứng dụng:**
   ```bash
   dotnet run --project CleanArchitectureProject/CleanArchitectureProject.csproj
   ```

4. **Kiểm tra API:**
   - OpenAPI / Scalar / Swagger (theo cấu hình): truy cập `https://localhost:<port>/openapi/v1.json`
   - Tệp test HTTP: `CleanArchitectureProject/CleanArchitectureProject.http`
