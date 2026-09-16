# Application / Interfaces

Thư mục chứa các **Hợp đồng giao tiếp (Interfaces)** của tầng Application.

---

## 📌 Khái niệm

Theo nguyên lý **Dependency Inversion Principle (DIP)** trong SOLID:
> *"Các module cấp cao không nên phụ thuộc vào các module cấp thấp. Cả hai nên phụ thuộc vào sự trừu tượng (abstraction)."*

Tầng Application định nghĩa các `interface` mô tả các thao tác mà nó cần thực hiện (như đọc/ghi cơ sở dữ liệu, gửi email, tích hợp cổng thanh toán). Tầng Infrastructure sẽ chịu trách nhiệm triển khai (`implement`) các interface này.

---

## 📂 Các nhóm Interface thường gặp

1. **Repository Interfaces**: 
   - Định nghĩa các thao tác truy xuất thực thể (ví dụ: `IProductRepository`, `IOrderRepository`).
2. **Unit of Work**:
   - Quản lý transaction và xác nhận lưu thay đổi (ví dụ: `IUnitOfWork`).
3. **External Service Interfaces**:
   - Dịch vụ gửi thông báo, tệp tin, thanh toán (ví dụ: `IEmailService`, `IFileStorageService`).

---

## 💡 Ví dụ minh họa

```csharp
namespace CleanArchitectureProject.Application.Interfaces;

using CleanArchitectureProject.Domain.Entities;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Delete(Product product);
}
```
