# Presentation / Filters (Module-Level Interceptors)

Thư mục chứa các **Action Filters** hoặc **Endpoint Filters** thuộc tầng Presentation của Module.

---

## 🎯 Vì sao dùng Filters thay cho Middleware tại tầng Module?

Trong kiến trúc **Modular Monolith**:
- **Cạm bẫy của Middleware**: Pipeline của ASP.NET Core chạy dọc cho toàn bộ ứng dụng. Nếu bạn định nghĩa Middleware tại Module `Catalog`, nó sẽ chặn cả các request gửi đến Module `Orders` và `Payments`. Điều này phá vỡ tính cô lập của module.
- **Giải pháp chuẩn xác**: Sử dụng **Action Filters / Endpoint Filters**. Filters chỉ được kích hoạt khi request được định tuyến (Routed) trúng vào Controller hoặc Endpoint của chính Module đó.

```
Request ---> [Global Middlewares tại Host] ---> Routing 
                                                  │
                                                  ├──> [Catalog Controller] + [Catalog Filters] ✅
                                                  └──> [Orders Controller] (Không bị ảnh hưởng) ✅
```

---

## 📂 Các dạng Filter thường dùng trong Module

1. **Validation Filter**: Kiểm tra tính hợp lệ của DTO đầu vào trước khi gọi Use Case.
2. **Resource Existence Filter**: Kiểm tra sự tồn tại của tài nguyên (ví dụ: Product có tồn tại không) trước khi thực thi action.
3. **Module Feature Flag Filter**: Bật/tắt các tính năng thử nghiệm trong module.

---

## 💡 Ví dụ Minh Họa: Action Filter nội bộ Module Catalog

```csharp
namespace CleanArchitectureProject.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Filter này CHỈ tác động lên các Controller/Action của Module Catalog được gắn attribute.
/// Hoàn toàn không ảnh hưởng đến các module khác trong hệ thống.
/// </summary>
public class ValidateProductIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is Guid id)
        {
            if (id == Guid.Empty)
            {
                context.Result = new BadRequestObjectResult(new { error = "ProductId không hợp lệ (không được để trống)." });
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
```

### Sử dụng trên Controller của Module:

```csharp
[ApiController]
[Route("api/catalog/products")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ValidateProductId] // <-- Chỉ áp dụng cho endpoint này của Catalog
    public IActionResult GetById(Guid id)
    {
        return Ok(...);
    }
}
```
