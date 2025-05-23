using System;
using Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Implementations;
using Repository.Interfaces;
using Repository.UnitOfWork;
using Service.Implementations;
using Service.Interfaces;
using Service.UnitOfWork;

namespace UIWeb.Infrastructure.Extensions
{
  public static class ServiceExtension
  {
    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<RepositoryContext>(options => // Veritabanı bağlantısı yapılıyor.
      {
        options.UseSqlServer(configuration.GetConnectionString("MsSqlConnection"), // appsettings.json dosyasındaki bağlantı bilgileri alınıyor.
          b => b.MigrationsAssembly("UIWeb")); // Migrations'ların nerde bulunacağını belirliyoruz.
        options.EnableSensitiveDataLogging(true); // Hassas bilgileri loglara yansıtmak için.
      });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
      services.AddIdentity<ApplicationUser, IdentityRole>(options => // Identity sistemini tanımlar.
      {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
      })
      .AddEntityFrameworkStores<RepositoryContext>(); // Identity verileri Entity Framework ile veri tabanına kaydedilecek.
    }

    public static void ConfigureSession(this IServiceCollection services)
    {
      services.AddDistributedMemoryCache(); // Dağıtılmış önbellek eklendi.
      services.AddSession(options => // Oturum (session) yönetimi için.
      {
        options.Cookie.Name = "NetBlog.Session"; // Çerezleri tutmak için isim.
        options.IdleTimeout = TimeSpan.FromMinutes(10); // Eğer yeni istek yoksa 10 dakika tutar.
      });
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // HTTP isteği bilgilerine erişim için.
    }

    // Repository IoC
    public static void ConfigureRepositoryRegistration(this IServiceCollection services)
    {
      services.AddScoped<IRepositoryManager, RepositoryManager>();
      services.AddScoped<IPostRepository, PostRepository>();
      services.AddScoped<ICategoryRepository, CategoryRepository>();
      services.AddScoped<ICommentRepository, CommentRepository>();
      services.AddScoped<ILikeRepository, LikeRepository>();
    }

    // Service IoC
    public static void ConfigureServiceRegistration(this IServiceCollection services)
    {
      services.AddScoped<IServiceManager, ServiceManager>();
      services.AddScoped<IPostService, PostService>();
      services.AddScoped<ICategoryService, CategoryService>();
      services.AddScoped<ICommentService, CommentService>();
      services.AddScoped<ILikeService, LikeService>();
      services.AddScoped<IAuthService, AuthService>();
    }

    public static void ConfigureApplicationCookie(this IServiceCollection services)
    {
      services.ConfigureApplicationCookie(options =>
      {
        options.LoginPath = new PathString("/Account/SignIn");
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.AccessDeniedPath = new PathString("/Account/AccessDenied");
      });
    }

    // Lowercase ve Slash için service.
    public static void ConfigureRouting(this IServiceCollection services)
    {
      services.AddRouting(options =>
      {
        options.LowercaseUrls = true; // lowercase
        options.AppendTrailingSlash = false; // slash
      }
      );
    }
  }
}