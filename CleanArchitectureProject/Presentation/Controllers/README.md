# Presentation / Controllers

Thư mục chứa các **API Controllers** kế thừa từ `ControllerBase`.

---

## 📌 Khái niệm

Controllers định nghĩa các route, HTTP verbs (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`) và tiếp nhận yêu cầu từ client.

---

## 📐 Đặc điểm thiết kế (Thin Controller)

- Sử dụng thuộc tính `[ApiController]` và `[Route("api/[controller]")]`.
- **Dependency Injection**: Inject các Application Service (hoặc `IMediator`) thông qua Constructor.
- **Không chứa business logic**: Mọi logic kiểm tra nghiệp vụ và xử lý dữ liệu phải được ủy quyền cho tầng Application.
- **Chuẩn hóa mã HTTP**: Trả về `Ok()`, `Created()`, `NoContent()`, `BadRequest()`, `NotFound()` phù hợp với chuẩn REST.

---

## 💡 Ví dụ minh họa

```csharp
namespace CleanArchitectureProject.Presentation.Controllers;

using Microsoft.AspNetCore.Mvc;
using CleanArchitectureProject.Application.DTOs;
using CleanArchitectureProject.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        // Gọi Application Service để lấy dữ liệu
        return Ok();
    }
}
```
