// Learn more about configuring Ocelot/OpenAPI at https://ocelot.readthedocs.io/
// ref : "API Gateway Ocelot .Net Core 6.1 Setup" at https://stackoverflow.com/questions/71264496

// Import library
using System;
using System.Text;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Creaate application builder
var builder = WebApplication.CreateBuilder(args);

// Generate ocelot configuration file with multiple json file
// Setting ocelot service with hostingContext and config object.
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) => {
    // Setting host server setting and default ocelot configuration
    // When this function execute, default appsettings will not working, you must addition by yourself.
    var env = hostingContext.HostingEnvironment;
    config.AddJsonFile("appsettings.json", true, true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
          .AddJsonFile("ocelot.json")
          .AddEnvironmentVariables();
    // Merge ocelot configuration come from path "/app/ocelot", and replace local ocelot.json.
    config.AddOcelot("/app/ocelot", env);
});

// Add ocelot configuration with single json file
// Declare ocelot configuration by setting json file.
/*
IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("ocelot.json")
                            .Build();
*/
// Set ocelot in services.
builder.Services.AddOcelot();

// 註冊使用 Authentication 服務
// 於專案設定 NuGet 套件引用 Microsoft.AspNetCore.Authentication.JwtBearer，並依據專案框架版本選用套件
// Ref : https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/3.0.0
// .NET Core 共有兩種驗證機制，JwtBearer、Cookie，兩者差別可參考說明文獻
var Configuration = builder.Configuration;
builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
        // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
        options.IncludeErrorDetails = true;

        // 當驗證 JwtToken 時，不進行 JSON claim 轉換，例如將 roles 轉換為 SAML 標準聲明
        // 由於 Ocelot 為字串比對檢查，當出現 roles 被轉換為 SAML 聲明的 "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"，這會觸發 JSON 解析問題導致被 Ocelot 判斷為沒有比對內容，
        // 若為保持與 .NET 的 Roles 處理相同授權字詞，則需關閉此轉換，設定 "roles" 於授權宣告中
        options.MapInboundClaims = false;

        // 設定驗證程序須執行的動作項目
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // 驗證 Issuer 資訊，若驗證失敗則表示該 JWT 非本系統發出
            ValidateIssuer = true,
            ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

            // 驗證 Audience 資訊，若驗證失敗則表示該 JWT 非本系統發出
            // 若不驗證就不需要填寫並設為 false
            ValidateAudience = true,
            ValidAudience = Configuration.GetValue<string>("JwtSettings:Issuer"),

            // 驗證 Token 的有效期間
            ValidateLifetime = true,

            // 若 Token 中包含 key 需要驗證其內容，若未提供則應避免驗證
            ValidateIssuerSigningKey = false,

            // 提供自 IConfiguration 取得的驗證碼
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSettings:SignKey")))
        };

        //
        Configuration.Bind("JwtSettings", options);
        });

// Build and setting application
var app = builder.Build();

// Sets up all the Ocelot middleware
await app.UseOcelot();

// Sets up application routing
app.MapGet("/", () => "Hello World!");

// Startup application
app.Run();
