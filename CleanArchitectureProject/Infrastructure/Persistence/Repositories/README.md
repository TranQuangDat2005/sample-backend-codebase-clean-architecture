# Infrastructure / Persistence / Repositories

Thư mục chứa các lớp triển khai cụ thể của **Repository Pattern** sử dụng Entity Framework Core.

---

## 📌 Khái niệm

Các lớp repository trong thư mục này thực thi các interface được định nghĩa tại `Application/Interfaces/` (ví dụ: `IProductRepository`).

Repository đóng vai trò như một tập hợp đối tượng trong bộ nhớ, che giấu các chi tiết truy vấn cơ sở dữ liệu (`DbContext`, `DbSet`, LINQ, SQL) khỏi tầng Application.

---

## 📐 Đặc điểm thiết kế

- Có thể tạo một lớp cơ sở `Repository<TEntity>` để tái sử dụng các thao tác CRUD cơ bản (`GetByIdAsync`, `AddAsync`, `Update`, `Delete`).
- Với các truy vấn phức tạp hoặc đặc thù của Entity, mở rộng bằng các Repository riêng (ví dụ: `ProductRepository : Repository<Product>, IProductRepository`).
- Sử dụng `AsNoTracking()` cho các truy vấn chỉ đọc (read-only queries) để tối ưu hiệu năng.

---

## 💡 Ví dụ minh họa

```csharp
namespace CleanArchitectureProject.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using CleanArchitectureProject.Application.Interfaces;
using CleanArchitectureProject.Domain.Entities;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }
}
```
