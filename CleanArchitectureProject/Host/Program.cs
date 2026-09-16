using CleanArchitectureProject.Presentation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArchitectureProject.Host
{
    /// <summary>
    /// File Program.cs tại tầng Host đóng vai trò là "Composition Root" (Điểm hội tụ lắp ráp).
    /// <para>
    /// Trong kiến trúc <b>Modular Monolith kết hợp Clean Architecture</b>:
    /// - Tầng Host KHÔNG chứa bất kỳ logic nghiệp vụ nào.
    /// - Tầng Host KHÔNG trực tiếp đăng ký DbContext hay Service của từng module.
    /// - Tầng Host chỉ chịu trách nhiệm: Lắp ráp các Modules lại với nhau và cấu hình các mối quan tâm chung (Cross-Cutting Concerns).
    /// </para>
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================================================================
            // 1. LẮP RÁP CÁC MODULES (MODULE COMPOSITION)
            // ============================================================================
            // Mỗi module tự đóng gói việc đăng ký DI của mình thông qua Extension Method.
            // Nhờ đó, Program.cs ở Host luôn cực kỳ gọn gàng, dù hệ thống có mở rộng lên hàng chục module.

            // Module 1: Catalog (Quản lý sản phẩm, danh mục hàng hóa)
            builder.Services.AddCatalogModule(builder.Configuration);

            // Module 2: Orders (Quản lý đơn hàng, giỏ hàng - minh họa cách mở rộng thêm module)
            builder.Services.AddOrdersModule(builder.Configuration);

            // Module 3: Payments (Quản lý thanh toán, cổng thanh toán - minh họa)
            builder.Services.AddPaymentsModule(builder.Configuration);

            // ============================================================================
            // 2. CẤU HÌNH CÁC DỊCH VỤ DÙNG CHUNG CỦA TẦNG HOST (CROSS-CUTTING CONCERNS)
            // ============================================================================
            
            // Đăng ký Controllers của ASP.NET Core MVC/Web API
            builder.Services.AddControllers();

            // Cấu hình tài liệu API OpenAPI / Swagger
            builder.Services.AddOpenApi();

            // [Ví dụ] Cấu hình Shared Event Bus dùng để giao tiếp bất đồng bộ giữa các Module trong bộ nhớ (In-process):
            // builder.Services.AddSingleton<IInMemoryEventBus, InMemoryEventBus>();

            // [Ví dụ] Cấu hình Authentication & Authorization chung toàn hệ thống (JWT Bearer):
            // builder.Services.AddAuthentication(...)
            // builder.Services.AddAuthorization();

            // [Ví dụ] Cấu hình Health Checks để giám sát trạng thái của Web Host và các Module:
            // builder.Services.AddHealthChecks();

            // ============================================================================
            // 3. BUILD ỨNG DỤNG & CẤU HÌNH HTTP REQUEST PIPELINE (MIDDLEWARES)
            // ============================================================================
            var app = builder.Build();

            // Môi trường Development: Mở tài liệu API Swagger / OpenAPI
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Global Exception Handling: Xử lý ngoại lệ tập trung toàn ứng dụng
            // app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            // Kích hoạt xác thực & phân quyền (nếu có)
            // app.UseAuthentication();
            app.UseAuthorization();

            // Điều hướng các HTTP request đến Controllers của các Module đã đăng ký
            app.MapControllers();

            // Khởi chạy ứng dụng
            app.Run();
        }
    }

    #region Module Extension Stubs (Mã giả lập phục vụ mục đích minh họa & giải thích)

    /// <summary>
    /// Lớp mở rộng minh họa cho Module Orders.
    /// Giúp người đọc hiểu cách các module khác nhau được ghép vào Host tương tự như CatalogModule.
    /// </summary>
    public static class OrdersModuleExtensions
    {
        public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Tự đăng ký Application, Infrastructure (OrdersDbContext), Presentation của Orders
            // services.AddDbContext<OrdersDbContext>(...);
            // services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }
    }

    /// <summary>
    /// Lớp mở rộng minh họa cho Module Payments.
    /// </summary>
    public static class PaymentsModuleExtensions
    {
        public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Tự đăng ký các dịch vụ thanh toán (VNPAY, MoMo, Stripe Gateway adapters)
            // services.AddScoped<IPaymentGateway, VnPayGateway>();
            return services;
        }
    }

    #endregion
}
