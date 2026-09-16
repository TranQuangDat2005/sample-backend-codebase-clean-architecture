# Tests / Infrastructure

Kiểm thử **Infrastructure Layer** — tầng triển khai Repository và tương tác với cơ sở dữ liệu.

---

## 📌 Nguyên tắc

- **Dùng EF Core InMemory hoặc SQLite (:memory:)** thay thế DB thật để test nhanh, độc lập.
- Kiểm tra các thao tác CRUD thực sự qua `DbContext` (không mock EF Core).
- Đây là **Integration Test** — chậm hơn Unit Test nhưng phát hiện lỗi ánh xạ (mapping), quan hệ (relation), và migration sớm hơn.

> ⚠️ **Lưu ý**: EF Core InMemory không hỗ trợ đầy đủ các ràng buộc quan hệ (foreign key, unique). Nếu cần kiểm tra constraint, dùng **SQLite in-memory** thay thế.

---

## 📦 NuGet cần cài

```xml
<!-- Infrastructure.Tests.csproj -->
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.*" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
```

---

## 📁 Cấu trúc

```
Infrastructure/
└── Persistence/
    ├── ProductRepositoryTests.cs     # Test Add / Get / Update / Delete qua DbContext
    └── ApplicationDbContextTests.cs  # Test cấu hình Fluent API (ánh xạ bảng, quan hệ)
```

---

## 💡 Ví dụ — SQLite In-Memory (khuyên dùng)

```csharp
// Infrastructure/Persistence/ProductRepositoryTests.cs
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureProject.Domain.Entities;
using CleanArchitectureProject.Infrastructure.Persistence;
using CleanArchitectureProject.Infrastructure.Persistence.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        // Dùng SQLite in-memory để hỗ trợ đầy đủ SQL constraint
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated(); // Tạo schema từ Fluent API config

        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistProduct_WhenValid()
    {
        // Arrange
        var product = new Product(id: 0, name: "Laptop", price: 15_000_000);

        // Act
        await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Laptop");
        saved.Should().NotBeNull();
        saved!.Price.Should().Be(15_000_000);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }
}
```
