# Presentation Layer

Tầng **Presentation** là điểm tiếp xúc đầu tiên (Entry Point) của ứng dụng với thế giới bên ngoài. Trong dự án này, nó đóng vai trò là **ASP.NET Core Web API**.

---

## 🎯 Mục đích & Trách nhiệm

- Tiếp nhận các HTTP Request từ client (Web frontend, Mobile app, Postman, dịch vụ khác).
- Xác thực và phân quyền người dùng (Authentication & Authorization).
- Điều hướng request đến đúng Application Service hoặc Command/Query Handler.
- Đóng gói dữ liệu trả về theo chuẩn RESTful API với các mã trạng thái HTTP tương ứng (`200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`,...).
- Tự đóng gói và đăng ký Dependency Injection (DI) nội bộ qua `CatalogModuleInjection.cs` để tầng Host lắp ráp.

---

## 📁 Cấu trúc thư mục & tệp quan trọng

| Tệp / Thư mục | Mục đích |
| :--- | :--- |
| **[Controllers](./Controllers/README.md)** | Chứa các Web API Controllers xử lý các endpoint HTTP của module. |
| **[Filters](./Filters/README.md)** | Chứa các Action/Endpoint Filters can thiệp request riêng của module (thay thế cho Middleware). |
| `CatalogModuleInjection.cs` | Điểm đăng ký DI (Extension Method) khép kín của Module Catalog (`AddCatalogModule`). |
| `appsettings.json` | Tệp cấu hình ứng dụng (Connection strings, JWT settings, logging level). |
| *(Tầng Host)* | `Middlewares` và `Program.cs` toàn cục đã được chuyển sang tầng **[Host](../Host/README.md)**. |

---

## 🧩 Đăng ký Dependency Injection theo hướng Modular Monolith

Thay vì cấu hình toàn bộ các dịch vụ (DbContext, Repository, MediatR,...) tập trung tại `Program.cs`, mỗi Module sẽ tự đóng gói các thành phần của mình thông qua một **Extension Method** riêng (ví dụ: `CatalogModuleInjection.cs`):

```csharp
public static class CatalogModuleInjection
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Đăng ký tầng Application (Use Cases, Handlers, Validators)
        services.AddCatalogApplication();

        // 2. Đăng ký tầng Infrastructure (DbContext, Repositories)
        services.AddCatalogInfrastructure(configuration);

        // 3. Đăng ký tầng Presentation (Controllers, Module Filters)
        services.AddCatalogPresentation();

        return services;
    }
}
```

Lợi ích:
- **Tính đóng gói (Encapsulation)**: Tầng Host không cần biết bên trong Catalog sử dụng công nghệ hay thư viện gì (EF Core, Dapper, MediatR,...).
- **Phân tách trách nhiệm (Separation of Concerns)**: Mỗi team phát triển module có thể tự quản lý DI của mình mà không gây xung đột git merge tại `Program.cs`.
- **Dễ dàng bảo trì & kiểm thử**: Khi cần gỡ bỏ, thay thế hoặc test riêng module Catalog, chỉ cần thao tác trên method này.

---

## ⚠️ Nguyên tắc thiết kế cần tuân thủ

1. **Thin Controllers**: Controller chỉ nên đóng vai trò trung chuyển dữ liệu (nhận request -> gọi service -> trả response). Không viết logic nghiệp vụ hay truy vấn database trực tiếp trong controller.
2. **Không trả về Domain Entities trực tiếp**: Luôn ánh xạ Entity sang DTO trước khi trả về client để tránh lộ thông tin nhạy cảm và vòng lặp tham chiếu JSON (cyclic reference).
3. **Module Encapsulation**: Presentation của module chỉ expose các endpoint HTTP hoặc hợp đồng giao tiếp cần thiết ra bên ngoài, giấu kín các implementation nội bộ.

