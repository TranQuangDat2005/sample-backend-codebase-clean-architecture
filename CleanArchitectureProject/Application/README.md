# Application Layer

Tầng **Application** chứa các quy tắc nghiệp vụ của ứng dụng (**Application Business Rules**), thường được gọi là các **Use Cases**.

---

## 🎯 Mục đích & Trách nhiệm

- Điều phối dòng chảy dữ liệu giữa Domain và các tầng bên ngoài.
- Nhận dữ liệu đầu vào từ Presentation (dưới dạng DTO hoặc Command/Query), áp dụng logic nghiệp vụ thông qua Domain Entities/Services, và lưu trạng thái xuống cơ sở dữ liệu qua Repository Interfaces.
- Định nghĩa các **Interfaces** trừu tượng (như `IRepository`, `IEmailService`, `IUnitOfWork`) để tầng Infrastructure triển khai, tuân thủ nguyên lý **Dependency Inversion Principle (DIP)**.

---

## 📁 Cấu trúc thư mục con

| Thư mục | Mục đích |
| :--- | :--- |
| **[DTOs](./DTOs/README.md)** | Các đối tượng truyền dữ liệu (Data Transfer Objects) giữa Presentation và Application. |
| **[Interfaces](./Interfaces/README.md)** | Định nghĩa hợp đồng trừu tượng (Repositories, Services, Caching, Messaging). |
| **[Services](./Services/README.md)** | Triển khai logic nghiệp vụ ứng dụng và điều phối các use case. |

---

## ⚠️ Nguyên tắc thiết kế cần tuân thủ

1. **Chỉ phụ thuộc vào Domain**: Tầng Application chỉ được tham chiếu đến tầng Domain, tuyệt đối không tham chiếu đến Infrastructure hay Presentation.
2. **Không phụ thuộc vào công nghệ dữ liệu cụ thể**: Không dùng trực tiếp EF Core `DbContext`, SQL queries, hay các thư viện HTTP client cụ thể trong tầng này. Tất cả truy xuất tài nguyên ngoại vi đều phải thông qua Interface.
