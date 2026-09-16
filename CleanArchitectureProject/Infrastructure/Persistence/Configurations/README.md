# Infrastructure / Persistence / Configurations

Thư mục chứa các lớp cấu hình **Fluent API** của Entity Framework Core.

---

## 📌 Khái niệm

Thay vì sử dụng Data Annotations (như `[Required]`, `[MaxLength]`, `[Table]`) trực tiếp lên các Domain Entities (làm vấy bẩn tầng Domain với các mối quan tâm của Database), Clean Architecture khuyến nghị sử dụng **Fluent API** thông qua `IEntityTypeConfiguration<TEntity>`.

---

## 📐 Lợi ích

- **Giữ Domain thuần khiết (POCO)**: Domain Entities hoàn toàn sạch sẽ, không phụ thuộc vào `System.ComponentModel.DataAnnotations`.
- **Tập trung và rõ ràng**: Mỗi Entity có một tệp cấu hình riêng biệt (ví dụ: `ProductConfiguration.cs`), dễ quản lý và bảo trì.
- **Tự động đăng ký**: Có thể quét tự động bằng `modelBuilder.ApplyConfigurationsFromAssembly(...)` trong `DbContext`.

---

## 💡 Ví dụ minh họa

```csharp
namespace CleanArchitectureProject.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArchitectureProject.Domain.Entities;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.StockQuantity)
            .IsRequired();
    }
}
```
