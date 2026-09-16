# Properties

Thư mục chứa các tệp thiết lập môi trường phát triển và thực thi của Visual Studio / .NET CLI.

---

## 📌 Các tệp trong thư mục

- **`launchSettings.json`**:
  - Xác định các hồ sơ khởi chạy (launch profiles) khi chạy ứng dụng trên máy cục bộ (localhost).
  - Cấu hình các biến môi trường như `ASPNETCORE_ENVIRONMENT` (ví dụ: `Development`).
  - Thiết lập cổng lắng nghe HTTP và HTTPS cho Kestrel hoặc IIS Express.

---

## ⚠️ Lưu ý

- Tệp `launchSettings.json` chỉ được sử dụng trong môi trường phát triển cục bộ và không được đóng gói khi deploy sản phẩm (Publish/Production).
