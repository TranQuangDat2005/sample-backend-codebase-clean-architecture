# Domain / Exceptions

Thư mục chứa các **Ngoại lệ Domain (Domain Exceptions)**.

---

## 📌 Khái niệm

Domain Exceptions là các ngoại lệ xảy ra khi một thao tác hoặc trạng thái vi phạm quy tắc nghiệp vụ bất biến của miền (Domain Invariants). 

Ví dụ: 
- Cố gắng rút tiền vượt quá số dư tài khoản khả dụng.
- Đặt trạng thái đơn hàng đã giao thành đang chuẩn bị.
- Cố tình áp dụng mã khuyến mãi đã hết hạn.

---

## 📐 Đặc điểm thiết kế

- Thường kế thừa từ một lớp cơ sở `DomainException` (kế thừa từ `Exception`).
- Giúp phân biệt rõ ràng giữa lỗi do vi phạm nghiệp vụ (Business Rule Violation) và lỗi hệ thống (Database timeout, null reference, network error,...).
- Tầng Presentation (thông qua Global Exception Middleware) có thể bắt các `DomainException` này và chuyển đổi thành HTTP status code thích hợp (ví dụ: `400 Bad Request` hoặc `422 Unprocessable Entity`).

---

## 💡 Ví dụ minh họa

```csharp
namespace CleanArchitectureProject.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int requestedQuantity, int availableQuantity)
        : base($"Sản phẩm '{productName}' không đủ tồn kho. Yêu cầu: {requestedQuantity}, khả dụng: {availableQuantity}.")
    {
    }
}
```
