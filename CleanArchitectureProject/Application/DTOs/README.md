# Application / DTOs

Thư mục chứa các **Data Transfer Objects (DTOs)**.

---

## 📌 Khái niệm

DTO (Data Transfer Object) là một đối tượng thuần dữ liệu (chỉ chứa các thuộc tính không có logic nghiệp vụ) được dùng để truyền tải dữ liệu giữa các tầng hoặc giữa Client và Server.

DTOs giúp:
- **Tách biệt Entity khỏi API**: Tránh rò rỉ cấu trúc cơ sở dữ liệu hoặc logic nội bộ ra ngoài thế giới bên ngoài.
- **Bảo mật**: Ngăn chặn lỗi bảo mật như Over-posting / Mass-assignment (ví dụ: người dùng gửi kèm trường `IsAdmin = true`).
- **Tối ưu hóa dữ liệu**: Chỉ trả về những trường thông tin cần thiết cho Client thay vì toàn bộ Entity.

---

## 📂 Tổ chức DTOs

Nên phân tách DTOs theo Request / Response hoặc theo tính năng:

- `CreateProductRequest.cs`
- `UpdateProductRequest.cs`
- `ProductResponse.cs`

---

## 💡 Ví dụ minh họa

```csharp
namespace CleanArchitectureProject.Application.DTOs;

public record CreateProductRequest(
    string Name,
    decimal Price,
    int StockQuantity
);

public record ProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    int StockQuantity
);
```
