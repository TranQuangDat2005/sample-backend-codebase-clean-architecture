# Domain Layer

Tầng **Domain** là trung tâm (trái tim) của hệ thống Clean Architecture. Tầng này đại diện cho các quy tắc nghiệp vụ cốt lõi của doanh nghiệp (**Enterprise Business Rules**).

---

## 🎯 Mục đích & Trách nhiệm

- Định nghĩa các khái niệm, quy tắc nghiệp vụ bất biến không phụ thuộc vào công nghệ hoặc hạ tầng.
- Hoàn toàn **độc lập**, không phụ thuộc vào bất kỳ tầng nào khác (không phụ thuộc vào Application, Infrastructure, Presentation hay các thư viện bên thứ ba như Entity Framework Core).
- Mã nguồn trong tầng này thuần C# (Plain Old C# Objects - POCO).

---

## 📁 Cấu trúc thư mục con

| Thư mục | Mục đích |
| :--- | :--- |
| **[Entities](./Entities/README.md)** | Chứa các thực thể có định danh (`Id`) riêng biệt và vòng đời độc lập. |
| **[Exceptions](./Exceptions/README.md)** | Chứa các ngoại lệ đặc thù của miền nghiệp vụ khi vi phạm quy tắc domain. |
| **[ValueObjects](./ValueObjects/README.md)** | Chứa các đối tượng giá trị, định nghĩa bằng các thuộc tính của nó thay vì ID, có tính bất biến (`Immutable`). |

---

## ⚠️ Nguyên tắc thiết kế cần tuân thủ

1. **Không phụ thuộc tầng ngoài**: Tuyệt đối không `using` namespace từ Application, Infrastructure, hay Presentation.
2. **Encapsulation (Đóng gói)**: Các thuộc tính nên sử dụng `private init` hoặc `private set` để đảm bảo dữ liệu chỉ được thay đổi thông qua các phương thức nghiệp vụ hợp lệ của Domain.
3. **No Database Concerns**: Không đặt các annotation hay thuộc tính liên quan đến cơ sở dữ liệu (ví dụ: `[Table]`, `[ForeignKey]`) trong tầng này. Việc cấu hình bảng sẽ do tầng Infrastructure đảm nhiệm (thông qua Fluent API).
