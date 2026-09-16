# Application / Services

Thư mục chứa các **Dịch vụ ứng dụng (Application Services)** triển khai các Use Cases.

---

## 📌 Khái niệm

Application Services (hoặc Use Case Handlers / MediatR Handlers) chịu trách nhiệm:
1. Nhận yêu cầu từ tầng Presentation (thông qua DTO).
2. Tải các Entity hoặc Aggregate Root cần thiết từ Repositories.
3. Thực thi nghiệp vụ bằng cách gọi các phương thức trên Domain Entities / Value Objects.
4. Lưu trạng thái mới xuống thông qua Repository / Unit of Work.
5. Chuyển đổi kết quả (Entity -> DTO) và trả về cho Controller.

---

## 📐 Đặc điểm thiết kế

- **Stateless**: Các service nên là không trạng thái (stateless) và được đăng ký với vòng đời `Scoped` trong DI container.
- **Không chứa Business Logic cốt lõi**: Tránh dồn toàn bộ logic vào Service (gây ra hiện tượng Anemic Domain Model). Service chỉ làm nhiệm vụ "nhạc trưởng" (orchestrator), điều phối giữa Domain và Persistence/External Services.

---

## 💡 Ví dụ minh họa

```csharp
namespace CleanArchitectureProject.Application.Services;

using CleanArchitectureProject.Application.DTOs;
using CleanArchitectureProject.Application.Interfaces;
using CleanArchitectureProject.Domain.Entities;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product(request.Name, request.Price, request.StockQuantity);
        
        await _productRepository.AddAsync(product, cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Price, product.StockQuantity);
    }
}
```
