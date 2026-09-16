# Tests / Api

Kiểm thử **Presentation Layer** — tầng API Controllers thông qua HTTP request thật.

---

## 📌 Nguyên tắc

- Dùng **`WebApplicationFactory<Program>`** (built-in của .NET, không cần thư viện ngoài) để khởi động toàn bộ pipeline ASP.NET Core.
- **Có thể thay thế DB** bằng InMemory trong `ConfigureTestServices` để cô lập từng test.
- Kiểm tra: **status code**, **response body (JSON)**, **header**, **validation error**.

---

## 📦 NuGet cần cài

```xml
<!-- Api.Tests.csproj -->
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.*" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
```

> ⚠️ Project **Api** (Presentation) phải có `<InternalsVisibleTo>` hoặc thêm `public partial class Program {}` ở cuối `Program.cs` để `WebApplicationFactory` truy cập được.

---

## 📁 Cấu trúc

```
Api/
└── Controllers/
    ├── ProductsControllerTests.cs    # Test GET /api/products, POST /api/products
    └── HealthCheckTests.cs           # Test endpoint sức khỏe ứng dụng
```

---

## 💡 Ví dụ

```csharp
// Api/Controllers/ProductsControllerTests.cs
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureProject.Infrastructure.Persistence;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Thay thế DbContext thật bằng InMemory
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            })
            .CreateClient();
    }

    [Fact]
    public async Task GET_Products_ShouldReturn200()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task POST_Products_ShouldReturn201_WhenValid()
    {
        // Arrange
        var payload = new { name = "Laptop", price = 15000000 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_Products_ShouldReturn400_WhenNameIsEmpty()
    {
        // Arrange
        var payload = new { name = "", price = 100 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
```
