using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureProject.Presentation
{
    /// <summary>
    /// File cấu hình Dependency Injection (DI) riêng biệt cho Module Catalog.
    /// <para>
    /// Áp dụng mô hình <b>Modular Monolith kết hợp Clean Architecture</b>:
    /// Thay vì để tầng Host (Program.cs) trực tiếp biết và đăng ký từng Service, Repository hay DbContext
    /// của Catalog, Module Catalog tự chịu trách nhiệm đóng gói (Encapsulation) và công khai duy nhất
    /// một điểm mở rộng (Facade Extension Method) là <see cref="AddCatalogModule"/>.
    /// </para>
    /// </summary>
    public static class CatalogModuleInjection
    {
        /// <summary>
        /// Điểm vào duy nhất (Facade) để đăng ký toàn bộ dịch vụ của Module Catalog vào DI Container.
        /// Tầng Host (Program.cs) chỉ cần gọi đúng method này.
        /// </summary>
        /// <param name="services">DI Service Collection của ứng dụng.</param>
        /// <param name="configuration">Cấu hình từ appsettings.json hoặc biến môi trường.</param>
        /// <returns>IServiceCollection để hỗ trợ Method Chaining (gọi nối tiếp).</returns>
        public static IServiceCollection AddCatalogModule(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // 1. Đăng ký các dịch vụ thuộc tầng Application (Logic nghiệp vụ, Use Cases, Handlers)
            services.AddCatalogApplication();

            // 2. Đăng ký các dịch vụ thuộc tầng Infrastructure (Database, DbContext, Repository, External Services)
            services.AddCatalogInfrastructure(configuration);

            // 3. Đăng ký các dịch vụ thuộc tầng Presentation (Controllers, Filters, Endpoints nếu có)
            services.AddCatalogPresentation();

            return services;
        }

        /// <summary>
        /// Đăng ký DI cho tầng Application của Module Catalog.
        /// <para>
        /// Tầng này chứa:
        /// - Command/Query Handlers (MediatR / CQRS)
        /// - Use Case Services
        /// - Data Validators (FluentValidation)
        /// - Object Mappers (AutoMapper, Mapster)
        /// </para>
        /// </summary>
        private static IServiceCollection AddCatalogApplication(this IServiceCollection services)
        {
            // [Ví dụ] Đăng ký MediatR quét qua Assembly của Catalog Application:
            // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CatalogModuleInjection).Assembly));

            // [Ví dụ] Đăng ký AutoMapper / Mapster:
            // services.AddAutoMapper(typeof(CatalogModuleInjection).Assembly);

            // [Ví dụ] Đăng ký FluentValidation:
            // services.AddValidatorsFromAssembly(typeof(CatalogModuleInjection).Assembly);

            // [Ví dụ] Đăng ký các Application Services trực tiếp:
            // services.AddScoped<IProductService, ProductService>();

            return services;
        }

        /// <summary>
        /// Đăng ký DI cho tầng Infrastructure của Module Catalog.
        /// <para>
        /// Tầng này chứa:
        /// - Database Context (EF Core DbContext với Connection String riêng hoặc chung)
        /// - Repository implementations (IProductRepository -> ProductRepository)
        /// - Tương tác hệ thống tệp tin, Cache, Email, Message Broker
        /// </para>
        /// </summary>
        private static IServiceCollection AddCatalogInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // [Ví dụ] Lấy chuỗi kết nối riêng cho Catalog (hỗ trợ tách database độc lập sau này):
            var connectionString = configuration.GetConnectionString("CatalogDatabase") 
                                   ?? configuration.GetConnectionString("DefaultConnection");

            // [Ví dụ] Đăng ký DbContext với EF Core:
            // services.AddDbContext<CatalogDbContext>(options =>
            //     options.UseSqlServer(connectionString, b => 
            //         b.MigrationsHistoryTable("__CatalogMigrationsHistory", "catalog")));

            // [Ví dụ] Đăng ký Repositories:
            // services.AddScoped<IProductRepository, ProductRepository>();
            // services.AddScoped<ICategoryRepository, CategoryRepository>();

            // [Ví dụ] Đăng ký UnitOfWork nếu sử dụng:
            // services.AddScoped<ICatalogUnitOfWork, CatalogUnitOfWork>();

            return services;
        }

        /// <summary>
        /// Đăng ký DI cho tầng Presentation của Module Catalog.
        /// <para>
        /// Đối với ASP.NET Core Web API:
        /// - Đăng ký Controllers của module nếu các module nằm ở các Assembly/Project tách biệt
        ///   (thông qua AddApplicationPart).
        /// - Cấu hình Action Filters, Model Binders riêng cho module.
        /// </para>
        /// </summary>
        private static IServiceCollection AddCatalogPresentation(this IServiceCollection services)
        {
            // [Ví dụ] Nếu module nằm trong Class Library riêng, cần báo cho ASP.NET Core biết
            // để nhận diện các Controller thuộc Assembly này:
            // services.AddControllers()
            //         .AddApplicationPart(typeof(CatalogModuleInjection).Assembly);

            return services;
        }
    }
}
