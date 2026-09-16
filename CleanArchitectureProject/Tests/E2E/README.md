# Tests / E2E

Kiểm thử **End-to-End** — luồng hoàn chỉnh từ HTTP request → xử lý nghiệp vụ → lưu DB → trả về response.

---

## 📌 Nguyên tắc

- **KHÔNG mock bất cứ thứ gì** — dùng toàn bộ stack thật (Application + Infrastructure + DB).
- Dùng **SQLite in-memory** làm DB thật trong môi trường test (nhanh, không cần cài SQL Server).
- Kiểm tra **hành vi của hệ thống từ góc nhìn người dùng cuối** (không quan tâm đến chi tiết cài đặt bên trong).
- Đây là bộ test **chậm nhất** nhưng **đáng tin cậy nhất** — nếu E2E pass, hệ thống hoạt động đúng.

---

## 📦 NuGet cần cài

```xml
<!-- E2E.Tests.csproj -->
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.*" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
```

---

## 📁 Cấu trúc

```
E2E/
└── Flows/
    ├── ProductLifecycleTests.cs    # Create → Get → Update → Delete một Product
    └── AuthFlowTests.cs            # Register → Login → truy cập endpoint bảo mật
```

---

## 💡 Ví dụ

```csharp
// E2E/Flows/ProductLifecycleTests.cs
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureProject.Infrastructure.Persistence;

// CustomWebApplicationFactory: cài SQLite in-memory thay DB thật
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Xóa DbContext đang dùng SQL Server
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Thay bằng SQLite in-memory
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("DataSource=:memory:"));

            // Tạo schema từ Fluent API config
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.OpenConnection();
            db.Database.EnsureCreated();
        });
    }
}

public class ProductLifecycleTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductLifecycleTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProductLifecycle_ShouldCreateThenRetrieve()
    {
        // STEP 1: Tạo sản phẩm
        var createPayload = new { name = "Laptop Gaming", price = 25_000_000 };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createPayload);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // STEP 2: Lấy ID từ response
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
        created.Should().NotBeNull();

        // STEP 3: GET lại sản phẩm vừa tạo
        var getResponse = await _client.GetAsync($"/api/products/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        product!.Name.Should().Be("Laptop Gaming");
        product.Price.Should().Be(25_000_000);
    }
}

// DTO dùng để deserialize response JSON
public record ProductDto(int Id, string Name, decimal Price);
```

---

## ⚖️ So sánh các loại test

| Loại test | Tốc độ | Độ tin cậy | Mock | Phát hiện lỗi |
| :--- | :--- | :--- | :--- | :--- |
| **Domain** (Unit) | ⚡ Rất nhanh | Cao | Không | Logic Entity, Rule |
| **Application** (Unit) | ⚡ Nhanh | Cao | NSubstitute | Logic Use Case |
| **Infrastructure** (Integration) | 🕐 Vừa | Cao | Không | Ánh xạ DB, Query |
| **Api** (Integration) | 🕐 Vừa | Cao | DB (InMemory) | HTTP routing, Middleware |
| **E2E** | 🐢 Chậm nhất | Rất cao | Không | Toàn bộ luồng |
