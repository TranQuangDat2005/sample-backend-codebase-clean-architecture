# Host / Middlewares (Global Middlewares)

Thư mục chứa các **Custom Middlewares toàn cục (Global)** tham gia vào đường ống xử lý HTTP Request Pipeline của tầng Host.

---

## 📌 Khái niệm & Vị trí trong Modular Monolith

Trong kiến trúc Modular Monolith, tầng **Host** đóng vai trò là cửa ngõ tiếp nhận toàn bộ HTTP Request trước khi định tuyến về các Module.

Các Middleware đặt tại tầng Host có đặc điểm:
- **Phạm vi toàn cục (Global Scope)**: Tác động lên **mọi request** gửi tới hệ thống, không phân biệt module nào (`Catalog`, `Orders`, `Payments`).
- **Xử lý Cross-Cutting Concerns**: Tập trung các mối quan tâm chung của toàn hệ thống như Bắt lỗi tập trung (Global Exception Handling), Logging, CORS, Security Headers, Rate Limiting.

> [!WARNING]
> **Không đặt Middleware bên trong Presentation của từng Module!**  
> Vì Middleware trong ASP.NET Core chạy dọc pipeline cho toàn bộ ứng dụng, nếu đặt middleware bên trong module thì request của các module khác cũng sẽ bị ảnh hưởng, phá vỡ tính cô lập (Module Boundary).

---

## 📂 Các Middleware Toàn Cục Cần Có

1. **ExceptionHandlingMiddleware (Xử lý lỗi tập trung)**:
   - Bắt tất cả các exception chưa được xử lý trong toàn bộ ứng dụng.
   - Bắt các lỗi nghiệp vụ chung (`DomainException`) và trả về định dạng chuẩn ProblemDetails (RFC 7807) với mã `400 Bad Request`.
   - Bắt các lỗi không xác định và trả về `500 Internal Server Error`, ẩn thông tin nhạy cảm.
2. **RequestLoggingMiddleware**:
   - Ghi nhận thông tin request/response (Method, Path, Query, Execution Time, Status Code).

---

## 💡 Ví dụ Minh Họa: Exception Handling Middleware

```csharp
namespace CleanArchitectureProject.Host.Middlewares;

using System.Net;
using System.Text.Json;
using CleanArchitectureProject.Domain.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain Exception caught: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Đã xảy ra lỗi hệ thống." }));
        }
    }
}
```

---

## 🚀 Kích hoạt Middleware tại Program.cs

Tại file [Host/Program.cs](../Program.cs):

```csharp
var app = builder.Build();

// Đăng ký Global Middleware vào đầu pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```
