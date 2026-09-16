# Domain / Entities

Thư mục chứa các **Thực thể (Entities)** của hệ thống.

---

## 📌 Khái niệm

Entity là một đối tượng nghiệp vụ được định danh bởi một khóa duy nhất (**Identity** / `Id`), chứ không phải bởi giá trị các thuộc tính của nó. Hai entity có cùng thuộc tính nhưng khác `Id` thì vẫn là hai thực thể khác biệt.

---

## 📐 Đặc điểm thiết kế

- **Có khóa định danh (Id)**: Thường là `Guid`, `int` hoặc `long`.
- **Đóng gói logic (Rich Domain Model)**: Tránh mô hình Anemic Domain Model (chỉ có getter/setter công khai). Các thay đổi trạng thái phải đi kèm logic nghiệp vụ và kiểm tra tính hợp lệ. (optional)
- **Auditable & Tracking**: Có thể kế thừa từ một lớp cơ sở chung (`BaseEntity`) chứa các thuộc tính như `Id`, `CreatedAt`, `UpdatedAt`, `IsDeleted`.

---

## 💡 Ví dụ minh họa
- Đây là ví dụ nâng cao, còn lại vẫn có thể dùng Anemic Domain Model
```csharp
namespace CleanArchitectureProject.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }

    private Product() { } // Dành cho EF Core

    public Product(string name, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên sản phẩm không được để trống.", nameof(name));

        if (price < 0)
            throw new ArgumentException("Giá không thể âm.", nameof(price));

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Giá mới không thể âm.", nameof(newPrice));

        Price = newPrice;
    }
}
```
