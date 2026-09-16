# Tests / Domain

Kiểm thử **Domain Layer** — tầng cốt lõi chứa các quy tắc nghiệp vụ thuần túy.

---

## 📌 Nguyên tắc

- **KHÔNG mock bất cứ thứ gì** — Domain hoàn toàn độc lập, không có dependency ngoài.
- Chỉ test các **class thuần C#**: Entities, Value Objects, Domain Exceptions.
- Đây là bộ test **nhanh nhất** và **ổn định nhất** trong toàn bộ hệ thống.

---

## 📦 NuGet cần cài

```xml
<!-- Domain.Tests.csproj -->
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
```

---

## 📁 Cấu trúc

```
Domain/
├── Entities/
│   ├── ProductTests.cs      # Test constructor, guard clause, domain rule
│   └── StudentTests.cs
└── ValueObjects/
    └── AddressTests.cs
```

---

## 💡 Ví dụ

```csharp
// Domain/Entities/ProductTests.cs
using FluentAssertions;
using CleanArchitectureProject.Domain.Entities;
using CleanArchitectureProject.Domain.Exceptions;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenNameIsEmpty()
    {
        // Arrange & Act
        Action act = () => new Product(id: 1, name: "", price: 100);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*tên*");
    }

    [Fact]
    public void Constructor_ShouldCreateProduct_WhenDataIsValid()
    {
        // Act
        var product = new Product(id: 1, name: "Laptop", price: 15_000_000);

        // Assert
        product.Name.Should().Be("Laptop");
        product.Price.Should().Be(15_000_000);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldThrow_WhenPriceIsNotPositive(decimal price)
    {
        Action act = () => new Product(id: 1, name: "Laptop", price: price);

        act.Should().Throw<DomainException>();
    }
}
```
