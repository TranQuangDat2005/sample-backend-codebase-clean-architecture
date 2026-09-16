# Infrastructure / Persistence

Thư mục chứa mã nguồn chịu trách nhiệm **Lưu trữ và Truy xuất Dữ liệu (Data Persistence)** của hệ thống.

---

## 📌 Khái niệm

Trong kiến trúc Clean Architecture với .NET, phân vùng Persistence thường sử dụng **Entity Framework Core** làm ORM chính để ánh xạ các Domain Entities thành các bảng cơ sở dữ liệu quan hệ (hoặc tài liệu NoSQL).

Thư mục này cũng chứa lớp `ApplicationDbContext` (kế thừa từ `DbContext`).

---

## 📁 Cấu trúc thư mục con

| Thư mục | Mục đích |
| :--- | :--- |
| **[Configurations](./Configurations/README.md)** | Ánh xạ cấu hình bảng, quan hệ, kiểu dữ liệu bằng Fluent API (`IEntityTypeConfiguration<T>`). |
| **[Migrations](./Migrations/README.md)** | Quản lý lịch sử và các tệp migration của Entity Framework Core. |
| **[Repositories](./Repositories/README.md)** | Các lớp triển khai cụ thể các Repository Interfaces được định nghĩa ở tầng Application. |

*Ngoài ra thì kết nối với database qua ApplicationDbContext tận dụng EF CORE
---

## 💡 Ví dụ ApplicationDbContext

```csharp
using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using System.Reflection;

namespace StudentManagement.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Luôn gọi base trước
        base.OnModelCreating(modelBuilder);

        // CÁCH 1: Tự động quét và nạp toàn bộ các class implement IEntityTypeConfiguration<T>
        // đang nằm trong cùng thư mục/project (Assembly) với ApplicationDbContext.
        // (Khuyên dùng - Thêm 100 bảng mới cũng không cần sửa code ở đây)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // --- HOẶC ---
        
        // CÁCH 2: Khai báo thủ công từng file cấu hình một 
        // (Chỉ dùng khi dự án quá nhỏ, dưới 5 bảng)
        // modelBuilder.ApplyConfiguration(new StudentConfiguration());
        // modelBuilder.ApplyConfiguration(new CourseConfiguration());
    }
}
```
