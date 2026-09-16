# Domain / ValueObjects

Thư mục chứa các **Đối tượng giá trị (Value Objects)**.

---

## 📌 Khái niệm

Value Object là một đối tượng đại diện cho một khái niệm mô tả trong hệ thống nhưng **không có định danh (Identity) riêng**. Hai Value Object được coi là bằng nhau nếu tất cả các giá trị thuộc tính của chúng bằng nhau (**Structural Equality**).

Ví dụ phổ biến: `Address`, `Money`, `Email`, `PhoneNumber`, `DateRange`.

---

## 📐 Đặc điểm thiết kế

1. **Bất biến (Immutability)**: Khi đã được tạo ra, trạng thái của Value Object không thể thay đổi. Mọi sự thay đổi phải tạo ra một instance mới.
2. **So sánh theo giá trị (Value Equality)**: Cần ghi đè `Equals()`, `GetHashCode()`, hoặc triển khai `IEquatable<T>`, hoặc kế thừa từ lớp trừu tượng `ValueObject`, hoặc sử dụng C# `record`.
3. **Tự xác thực (Self-validation)**: Đảm bảo chỉ có thể khởi tạo đối tượng với dữ liệu hợp lệ trong hàm khởi tạo.

---

## 💡 Ví dụ minh họa

Sử dụng C# `record struct` hoặc `record`:

```csharp
namespace CleanArchitectureProject.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Số tiền không được âm.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Loại tiền tệ không được để trống.", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Không thể cộng hai loại tiền tệ khác nhau.");

        return new Money(Amount + other.Amount, Currency);
    }
}
```
