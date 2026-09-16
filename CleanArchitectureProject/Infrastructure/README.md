# Infrastructure Layer

Tầng **Infrastructure** chịu trách nhiệm xử lý các mối quan tâm kỹ thuật bên ngoài (**External Concerns**) và tương tác với các công cụ, dịch vụ ngoại vi.

---

## 🎯 Mục đích & Trách nhiệm

- Triển khai các interface được định nghĩa tại tầng Application (ví dụ: `IRepository`, `IEmailService`, `ITokenProvider`).
- Giao tiếp với Cơ sở dữ liệu thông qua ORM (như Entity Framework Core, Dapper).
- Tích hợp các dịch vụ bên thứ ba: Cổng thanh toán (Stripe, VNPay), Dịch vụ gửi email (SendGrid, SMTP), Message Queue (RabbitMQ, Kafka), Lưu trữ tệp (AWS S3, Azure Blob).

---

## 📁 Cấu trúc thư mục con

| Thư mục | Mục đích |
| :--- | :--- |
| **[Persistence](./Persistence/README.md)** | Chứa toàn bộ mã nguồn liên quan đến cơ sở dữ liệu và truy xuất dữ liệu (DbContext, Configurations, Migrations, Repositories). |

---

## ⚠️ Nguyên tắc thiết kế cần tuân thủ

1. **Phụ thuộc vào Application và Domain**: Infrastructure triển khai các interface của Application và tham chiếu các entity của Domain.
2. **Không đảo ngược phụ thuộc**: Domain và Application tuyệt đối không được tham chiếu trực tiếp đến Infrastructure.
3. **Dễ thay thế**: Việc đổi nhà cung cấp cơ sở dữ liệu (từ SQL Server sang PostgreSQL) hoặc đổi dịch vụ gửi mail chỉ làm thay đổi code trong Infrastructure mà không làm thay đổi Domain/Application.
