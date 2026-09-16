# Tầng Host (Composition Root) trong Kiến trúc Modular Monolith

Tầng **Host** (thường là ASP.NET Core Web API hoặc Worker Service) đóng vai trò là **Composition Root** — điểm hội tụ và khởi chạy ứng dụng.

---

## 🎯 Mục đích & Trách nhiệm của Tầng Host

Trong mô hình **Modular Monolith kết hợp Clean Architecture**, mỗi Module (Catalog, Orders, Payments,...) được thiết kế như một hệ thống con độc lập (Subsystem) tuân thủ Clean Architecture.

Tầng **Host** có các trách nhiệm cốt lõi sau:
1. **Lắp ráp các Modules (Module Assembly / Composition)**: Gọi các Extension Method đăng ký DI của từng module (`AddCatalogModule`, `AddOrdersModule`,...).
2. **Cấu hình Cross-Cutting Concerns**: Thiết lập các mối quan tâm dùng chung toàn hệ thống như Authentication (JWT), Global Exception Handling, CORS, Logging (Serilog), OpenTelemetry, Health Checks.
3. **Cấu hình HTTP Middleware Pipeline**: Sắp xếp thứ tự các Middleware xử lý request trước khi định tuyến tới Controller của các Module.
4. **Không chứa Logic Nghiệp vụ**: Tuyệt đối không viết code nghiệp vụ, không trực tiếp thao tác Database hay truy cập Entity của bất kỳ module nào tại tầng Host.

---

## 🏛️ Sơ đồ Kiến trúc Tổng thể

```
                     +---------------------------------------------+
                     |                 TẦNG HOST                   |
                     |         (Web API / Composition Root)        |
                     |                 Program.cs                  |
                     +-------+---------------+-------------+-------+
                             |               |             |
           .AddCatalogModule | .AddOrders... |             | .AddPayments...
                             v               v             v
       +-----------------------+     +---------------+   +----------------+
       |     Module CATALOG    |     | Module ORDERS |   | Module PAYMENTS|
       |  +-----------------+  |     |  (Tương tự)   |   |   (Tương tự)   |
       |  |  Presentation   |  |     +---------------+   +----------------+
       |  |  (Controllers)  |  |
       |  +--------+--------+  |
       |           |           |
       |           v           |
       |  +-----------------+  |
       |  |   Application   |  |
       |  |   (Use Cases)   |  |
       |  +--------+--------+  |
       |           |           |
       |           v           |
       |  +-----------------+  |
       |  |     Domain      |  |
       |  |    (Entities)   |  |
       |  +--------+--------+  |
       |           ^           |
       |           |           |
       |  +--------+--------+  |
       |  | Infrastructure  |  |
       |  |   (DbContext)   |  |
       |  +-----------------+  |
       +-----------------------+
```

---

## ⚖️ So sánh: Monolith Truyền thống vs Modular Monolith

| Tiêu chí | Monolith Truyền thống | Modular Monolith (Khuyến nghị) |
| :--- | :--- | :--- |
| **`Program.cs`** | Dài hàng trăm/nghìn dòng; đăng ký lẫn lộn DbContext, Repositories, Services của toàn bộ dự án. | Cực kỳ tinh gọn (chỉ 20-40 dòng); chỉ gọi phương thức lắp ráp của từng module. |
| **Tính Đóng gói (Encapsulation)** | Kém; bất kỳ tầng nào cũng có thể gọi lộn xộn sang tầng khác hoặc module khác. | Cao; mỗi module chỉ công khai các hợp đồng (Contracts/DTOs) và Extension Method `Add{Name}Module`. |
| **Xung đột mã nguồn (Git Conflict)** | Thường xuyên xung đột tại `Program.cs` khi nhiều lập trình viên cùng thêm service. | Rất hiếm khi xung đột; mỗi nhóm chỉ chỉnh sửa trong file DI riêng của module mình. |
| **Tiến hóa lên Microservices** | Rất khó khăn và rủi ro vì các thành phần bị liên kết chặt chẽ (tight coupling). | Cực kỳ dễ dàng; chỉ cần tách module thành một Web API riêng rẽ mà không cần viết lại nghiệp vụ. |

---

## 📁 Cấu trúc Tệp tin trong Thư mục `Host`

| Tệp / Thư mục | Mục đích |
| :--- | :--- |
| [Program.cs](./Program.cs) | Điểm khởi đầu ứng dụng, gọi `AddCatalogModule`, `AddOrdersModule`,... và cấu hình pipeline HTTP. |
| **[Middlewares](./Middlewares/README.md)** | Chứa các Global Middlewares (Global Exception Handling, Request Logging, CORS,...). |
| `appsettings.json` | Cấu hình tập trung chuỗi kết nối (Connection Strings) và cấu hình riêng của từng module. |
| `README.md` | Tài liệu hướng dẫn kiến trúc và cách thức tích hợp module mới vào Host. |

---

## 🚀 Cách Thức Hoạt Động Cụ Thể

### 1. Module tự đóng gói việc đăng ký DI
Tại mỗi module (ví dụ module `Catalog`), ta tạo file [CatalogModuleInjection.cs](../Presentation/CatalogModuleInjection.cs):

```csharp
public static class CatalogModuleInjection
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCatalogApplication();
        services.AddCatalogInfrastructure(configuration);
        services.AddCatalogPresentation();
        return services;
    }
}
```

### 2. Tầng Host chỉ việc "lắp ghép" (Composition)
Tại [Host/Program.cs](./Program.cs):

```csharp
var builder = WebApplication.CreateBuilder(args);

// Lắp ráp các Modules một cách độc lập
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);

// Đăng ký Controllers và các thành phần hạ tầng chung
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 💡 Giao tiếp giữa các Module (Inter-Module Communication)

Trong kiến trúc Modular Monolith, các module **tuyệt đối không tham chiếu trực tiếp DbContext hay Repository của nhau**. Thay vào đó, giao tiếp được thực hiện qua các hình thức sau:

1. **In-process Event Bus (Bất đồng bộ - Async)**:
   - Module `Orders` phát ra một Integration Event (ví dụ: `OrderCreatedIntegrationEvent`).
   - Module `Catalog` lắng nghe sự kiện này và tự động trừ số lượng tồn kho sản phẩm.
   - Triển khai thông qua `InMemoryEventBus` hoặc MediatR Notifications trong cùng một tiến trình ứng dụng.
2. **Contracts / Interfaces công khai (Đồng bộ - Sync)**:
   - Nếu module `Orders` cần kiểm tra giá sản phẩm tức thì, module `Catalog` cung cấp một Interface công khai (ví dụ `ICatalogModuleApi` hoặc `IProductQueryService`) nằm ở tầng Shared/Contracts.
   - `Orders` chỉ phụ thuộc vào Interface này, không phụ thuộc vào `CatalogDbContext`.

---

## 🗄️ Chiến lược Quản lý Database & Migrations

Khi áp dụng mô hình Modular, có 2 cách tổ chức cơ sở dữ liệu:

1. **Single Database - Separate Schemas (Phổ biến & Đơn giản nhất)**:
   - Dùng chung một cơ sở dữ liệu vật lý (ví dụ: `AppDb`).
   - Mỗi module sở hữu một **Schema** riêng:
     - Bảng của Catalog: `catalog.Products`, `catalog.Categories`
     - Bảng của Orders: `orders.Orders`, `orders.OrderItems`
   - Mỗi module có một `DbContext` và file Migration History riêng:
     ```csharp
     options.UseSqlServer(connectionString, x => 
         x.MigrationsHistoryTable("__CatalogMigrationsHistory", "catalog"));
     ```
2. **Separate Databases (Độc lập hoàn toàn)**:
   - Mỗi module kết nối tới một Database riêng biệt (`CatalogDb`, `OrdersDb`).
   - Tối đa hóa tính độc lập, sẵn sàng chuyển thành Microservices bất cứ lúc nào.
