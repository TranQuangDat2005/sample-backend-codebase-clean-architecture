# Infrastructure / Persistence / Migrations

Thư mục chứa các tệp **Migrations** được tự động sinh ra bởi **Entity Framework Core**.

---

## 📌 Khái niệm

EF Core Migrations giúp theo dõi và áp dụng các thay đổi từ mô hình thực thể (C# code) lên lược đồ cơ sở dữ liệu (Database Schema) một cách có kiểm soát và an toàn.

Mỗi khi có thay đổi về Entity hoặc Configuration, một migration mới sẽ được tạo ra để ghi nhận phiên bản lược đồ cơ sở dữ liệu.

---

## 🛠️ Các lệnh thường dùng (.NET CLI)

1. **Thêm Migration mới:**
   ```bash
   dotnet ef migrations add InitialCreate --project CleanArchitectureProject/CleanArchitectureProject.csproj --output-dir Infrastructure/Persistence/Migrations
   ```

2. **Cập nhật Database:**
   ```bash
   dotnet ef database update --project CleanArchitectureProject/CleanArchitectureProject.csproj
   ```

3. **Xóa Migration cuối cùng (nếu chưa apply):**
   ```bash
   dotnet ef migrations remove --project CleanArchitectureProject/CleanArchitectureProject.csproj
   ```

4. **Tạo Script SQL migration để chạy trên Production:**
   ```bash
   dotnet ef migrations script --project CleanArchitectureProject/CleanArchitectureProject.csproj -i -o script.sql
   ```

---

## ⚠️ Lưu ý

- Không nên sửa trực tiếp các tệp migration bằng tay trừ khi thực sự cần thiết (ví dụ: viết custom SQL data seed).
- Luôn kiểm tra kỹ migration trước khi áp dụng lên môi trường Production.
