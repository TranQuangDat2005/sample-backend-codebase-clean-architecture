# Presentation Layer

Tầng **Presentation** là điểm tiếp xúc đầu tiên (Entry Point) của ứng dụng với thế giới bên ngoài. Trong dự án này, nó đóng vai trò là **ASP.NET Core Web API**.

---

## 🎯 Mục đích & Trách nhiệm

- Tiếp nhận các HTTP Request từ client (Web frontend, Mobile app, Postman, dịch vụ khác).
- Xác thực và phân quyền người dùng (Authentication & Authorization).
- Điều hướng request đến đúng Application Service hoặc Command/Query Handler.
- Đóng gói dữ liệu trả về theo chuẩn RESTful API với các mã trạng thái HTTP tương ứng (`200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`,...).
- Cấu hình pipeline HTTP (Middleware) và Dependency Injection (DI) tại `Program.cs`.

---

## 📁 Cấu trúc thư mục & tệp quan trọng

| Tệp / Thư mục | Mục đích |
| :--- | :--- |
| **[Controllers](./Controllers/README.md)** | Chứa các Web API Controllers xử lý các endpoint HTTP. |
| **[Middlewares](./Middlewares/README.md)** | Chứa các middleware tùy biến xử lý các mối quan tâm chung (Logging, Global Exception Handling, CORS,...). |
| `Program.cs` | Tệp khởi động ứng dụng, cấu hình DI container và HTTP request pipeline. |
| `appsettings.json` | Tệp cấu hình ứng dụng (Connection strings, JWT settings, logging level). |

---

## ⚠️ Nguyên tắc thiết kế cần tuân thủ

1. **Thin Controllers**: Controller chỉ nên đóng vai trò trung chuyển dữ liệu (nhận request -> gọi service -> trả response). Không viết logic nghiệp vụ hay truy vấn database trực tiếp trong controller.
2. **Không trả về Domain Entities trực tiếp**: Luôn ánh xạ Entity sang DTO trước khi trả về client để tránh lộ thông tin nhạy cảm và vòng lặp tham chiếu JSON (cyclic reference).
