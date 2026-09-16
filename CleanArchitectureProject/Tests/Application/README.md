# Tests / Application

Kiểm thử **Application Layer** — tầng chứa Use Cases và logic điều phối nghiệp vụ.

---

## 📌 Nguyên tắc

- **Mock toàn bộ dependencies** (IRepository, IUnitOfWork, external services) bằng **NSubstitute**.
- Chỉ kiểm tra **luồng logic** của Use Case: validate → gọi repository → commit → trả về kết quả.
- **Không** chạm vào DB thật, EF Core, hay HTTP.

---

## 📦 NuGet cần cài

```xml
<!-- Application.Tests.csproj -->
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="NSubstitute" Version="5.*" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
```

---

## 📁 Cấu trúc

```
Application/
└── Services/
    ├── ProductServiceTests.cs     # Test CreateProduct, UpdateProduct, DeleteProduct
    └── OrderServiceTests.cs
```

---

## 💡 Ví dụ

```csharp
// Application/Services/ProductServiceTests.cs
using FluentAssertions;
using NSubstitute;
using CleanArchitectureProject.Application.Interfaces;
using CleanArchitectureProject.Application.Services;
using CleanArchitectureProject.Domain.Entities;

public class ProductServiceTests
{
    // Tạo mock bằng NSubstitute
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task CreateAsync_ShouldAddProduct_WhenInputIsValid()
    {
        // Arrange
        var service = new ProductService(_repository, _unitOfWork);

        // Act
        await service.CreateAsync("Laptop", 15_000_000);

        // Assert — Kiểm tra repository.AddAsync được gọi đúng 1 lần
        await _repository.Received(1).AddAsync(
            Arg.Is<Product>(p => p.Name == "Laptop" && p.Price == 15_000_000)
        );

        // Assert — Kiểm tra SaveChanges được gọi sau đó
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNameIsEmpty()
    {
        // Arrange
        var service = new ProductService(_repository, _unitOfWork);

        // Act
        Func<Task> act = () => service.CreateAsync("", 100);

        // Assert
        await act.Should().ThrowAsync<Exception>();

        // Đảm bảo không gọi DB khi input sai
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>());
    }
}
```

---

## 🔍 NSubstitute — Cheat sheet nhanh

| Cú pháp | Ý nghĩa |
| :--- | :--- |
| `Substitute.For<IFoo>()` | Tạo mock |
| `mock.Received(1).Method()` | Kiểm tra method được gọi đúng 1 lần |
| `mock.DidNotReceive().Method()` | Kiểm tra method KHÔNG được gọi |
| `mock.Method().Returns(value)` | Cài giá trị trả về cho mock |
| `Arg.Is<T>(x => ...)` | Kiểm tra argument được truyền vào |
| `Arg.Any<T>()` | Chấp nhận mọi argument kiểu T |
