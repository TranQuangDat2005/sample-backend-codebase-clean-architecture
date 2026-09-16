# Presentation / Middlewares

Thư mục chứa các **Custom Middlewares** tham gia vào đường ống xử lý HTTP Request (Pipeline).

---

## 📌 Khái niệm

Middleware trong ASP.NET Core là các khối mã được lắp ghép thành pipeline để xử lý các request và response. Mỗi thành phần có thể:
- Chọn chuyển tiếp request cho thành phần tiếp theo trong pipeline.
- Thực hiện một số công việc trước và sau thành phần tiếp theo trong pipeline.

---

## 📂 Các Middleware phổ biến cần có

1. **ExceptionHandlingMiddleware (Xử lý lỗi tập trung)**:
   - Bắt tất cả các exception chưa được xử lý trong ứng dụng.
   - Bắt các `DomainException` và trả về `400 Bad Request` hoặc `422 Unprocessable Entity` với định dạng ProblemDetails (RFC 7807).
   - Bắt các lỗi hệ thống không mong muốn và ghi log, trả về `500 Internal Server Error`.
2. **RequestLoggingMiddleware**:
   - Ghi nhận thông tin request/response (Method, Path, Query, Execution Time, Status Code).

---

## 💡 Ví dụ minh họa (Exception Handling)

```csharp
namespace CleanArchitectureProject.Presentation.Middlewares;

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
