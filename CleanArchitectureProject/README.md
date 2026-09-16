# CleanArchitectureProject

Thư mục chứa mã nguồn chính của ứng dụng ASP.NET Core Web API theo mô hình **Clean Architecture**.

---

## 🏗️ Cấu trúc phân tầng

Dự án được tổ chức thành 4 tầng rõ ràng trong cùng một project:

1. **[Domain](./Domain/README.md)**: Chứa các quy tắc nghiệp vụ cốt lõi của doanh nghiệp (Enterprise Business Rules), bao gồm Entities, Value Objects, và Domain Exceptions. Không phụ thuộc vào bất kỳ thư viện hoặc tầng nào khác.
2. **[Application](./Application/README.md)**: Chứa các quy tắc nghiệp vụ ứng dụng (Application Business Rules), Use Cases, DTOs và các Interfaces trừu tượng. Chỉ phụ thuộc vào Domain.
3. **[Infrastructure](./Infrastructure/README.md)**: Triển khai các chi tiết kỹ thuật bên ngoài như Database, Entity Framework Core, kết nối bên thứ 3 (Email, SMS, Payment). Phụ thuộc vào Application (triển khai Interface) và Domain.
4. **[Presentation](./Presentation/README.md)**: Điểm vào của ứng dụng (Entry Point), tiếp nhận HTTP requests từ client, ánh xạ DTOs, gọi Application Services và trả về kết quả HTTP thích hợp.
5. **[Tests](./Tests/README.md)**: Toàn bộ các project kiểm thử tương ứng với từng tầng — Unit Test (Domain, Application), Integration Test (Infrastructure, Api), và E2E Test.

---


## ⚙️ Các tệp cấu hình quan trọng

- `CleanArchitectureProject.csproj`: Cấu hình target framework (`net10.0`), các package NuGet cần thiết.
- `CleanArchitectureProject.http`: Tệp gửi request HTTP để test API trực tiếp từ IDE.
- `Properties/launchSettings.json`: Thiết lập profile chạy khi debug (port, IIS Express, Kestrel, HTTPS).
