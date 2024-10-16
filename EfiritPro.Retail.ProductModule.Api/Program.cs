using EfiritPro.Retail.Packages.Rabbit.Extensions;
using EfiritPro.Retail.Packages.Utils;
using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using EfiritPro.Retail.ProductModule.Persistence.Migrations;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var grafanaLokiPath = Environment.GetEnvironmentVariable("GRAFANA_LOKI_PATH") ??
                              throw new InvalidOperationException("string GRAFANA_LOKI_PATH not found.");

if (builder.Environment.IsDevelopment())
{
    DotEnvService.LoadEnvironmentFromFile("Development.env");
}

void LogResponse(int statusCode, string response)
{
    if (statusCode >= 200 && statusCode < 300)
    {
        Log.Information(response);
    }
    else if (statusCode >= 400 && statusCode < 500)
    {
        Log.Warning(response);
    }
    else if (statusCode >= 500)
    {
        Log.Error(response);
    }
    else
    {
        Log.Information(response);
    }
}

async Task<string> FormatRequest(HttpRequest request)
{
    var body = request.Body;

    request.EnableBuffering();

    var buffer = new byte[Convert.ToInt32(request.ContentLength)];
    await request.Body.ReadAsync(buffer, 0, buffer.Length);
    var bodyAsText = Encoding.UTF8.GetString(buffer);
    request.Body.Position = 0;

    return $"{request.HttpContext.Connection.RemoteIpAddress}:{request.HttpContext.Connection.RemotePort} " +
        $"- {request.Headers["X-Forwarded-For"]} " +
        $"- \"{request.Method} {request.Path}{request.QueryString} {request.Protocol}\" {bodyAsText}";
}

async Task<string> FormatResponse(HttpResponse response)
{
    response.Body.Seek(0, SeekOrigin.Begin);
    var text = response.StatusCode == 500 ? await new StreamReader(response.Body).ReadToEndAsync() : "";
    response.Body.Seek(0, SeekOrigin.Begin);

    return $"{response.HttpContext.Connection.RemoteIpAddress}:{response.HttpContext.Connection.RemotePort} - " +
        $"\"{response.HttpContext.Request.Method} {response.HttpContext.Request.Path}{response.HttpContext.Request.QueryString} " +
        $"{response.HttpContext.Request.Protocol}\" {response.StatusCode} {text}";
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(grafanaLokiPath)
    //.Filter.ByExcluding(logEvent => logEvent.Properties.ContainsKey("SourceContext")
    //        && logEvent.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore"))
    .Filter.ByExcluding(le => le.Properties.ContainsKey("SourceContext")
            && le.Properties["SourceContext"].ToString().Contains("Database.Command"))
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "EfiritPro.Retail.ProductModule.Api", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


var productDbConnectionString = Environment.GetEnvironmentVariable("PRODUCT_DB") ??
                             throw new InvalidOperationException("Connection string PRODUCT_DB not found.");

var originsString = Environment.GetEnvironmentVariable("ORIGINS") ??
                    throw new InvalidOperationException("string ORIGIN not found");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(originsString.Split(';'))
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddDbContext<ProductDbContext>(options => options
        .UseNpgsql(productDbConnectionString)
        .UseSnakeCaseNamingConvention()
        .UseLazyLoadingProxies())
    .AddScoped<VATService>()
    .AddScoped<ProductTypeService>()
    .AddScoped<UnitService>()
    .AddScoped<MarkingTypeService>()
    .AddScoped<ProductService>()
    .AddScoped<ProductSetService>()
    //.AddScoped<ProductSetTreeService>()
    .AddScoped<ProductGroupService>()
    .AddScoped<ProductGroupTreeService>()
    .AddScoped<ProductPriceService>()
    .AddScoped<FavoriteProductService>()
    .AddHostedService<ProductPriceSetterService>()
    .AddRabbit<ProductDbContext, RabbitEventHandler>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var request = await FormatRequest(context.Request);
    Log.Information(request);

    var originalBodyStream = context.Response.Body;

    using (var responseBody = new MemoryStream())
    {
        context.Response.Body = responseBody;

        await next();

        var response = await FormatResponse(context.Response);
        LogResponse(context.Response.StatusCode, response);

        await responseBody.CopyToAsync(originalBodyStream);
    }
});

app
    .UseSwagger(x =>
    {
        x.RouteTemplate = "product/swagger/{documentname}/swagger.json";
    })
    .UseSwaggerUI(x =>
    {
        x.RoutePrefix = "product/swagger";
    });

app
    .UseRouting()
    .UseCors()
    .UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<ProductDbContext>();
    dbContext?.Database.Migrate();
    Console.WriteLine(productDbConnectionString);
    Console.WriteLine("can connect: " + dbContext.Database.CanConnect());

    //  init garbage
    if (dbContext.ProductGroups.Where(x=>x.Name== "deletedProducts").ToList().Count==0)
    {
        dbContext.ProductGroups.Add(new ProductGroup() { Name = "deletedProducts" });
        dbContext.SaveChanges();
    }

    //await CreateTestProductsMany(dbContext);
}

app.Run();

async Task CreateTestProductsMany(ProductDbContext db)
{
    //    Random uuu = new();

    //    for (int i = 0; i < 1000; i++)
    //    {
    //        string j = i.ToString();
    //        j = j.Length > 5 ? j.Substring(j.Length - 5) : j = j.PadLeft(5, '0');

    //        await db.Products.AddAsync(new Product()
    //        {
    //            Id = Guid.Empty,
    //            OwnerId = Guid.Parse("10cc52f0-7ec7-4fa2-b191-02208aef298a"),
    //            OrganizationId = Guid.Parse("5b2953dc-37b3-4ad6-8a3f-eaf0e77777b2"),

    //            Name = "тестирование скорости",
    //            VendorCode = $"{j}",
    //            BarCode = null,
    //            Description = null,
    //            Excise = true,
    //            Hidden = false,

    //            PriceMayChangeInCashReceipt = uuu.Next(0, 1) % 2 == 0 ? true : false,
    //            PriceMinInCashReceipt = (ulong)i,
    //            PriceMaxInCashReceipt = (ulong)i,

    //            CompositionList = new(),

    //            ProductGroup = null,

    //            VATId = Guid.Parse("c8cf8cbd-0f4f-471f-822d-4664bc94af5d"),

    //            ProductTypeId = Guid.Parse("06efb9ca-c06b-41c4-bba6-f8a65aeee918"),
    //            //ProductType = productType,

    //            UnitId = Guid.Parse("b0f6f8f3-69ee-4898-8314-cb18239e1c2a"),
    //            //Unit = unit,

    //            MarkingTypeId = null,
    //            //MarkingType = markingType,

    //            IsAllowedToMove = uuu.Next(0, 1) % 2 == 0 ? true : false,

    //            //ParentProductId = parentProductId,
    //            //ParentProduct = parentProduct,
    //        });
    //    }
    //    await db.SaveChangesAsync();
}

void InsertIntoDatabase(ProductDbContext context)
{
    var random = new Random();

    //context.ProductTypes.Add(new EfiritPro.Retail.ProductModule.Models.ProductType() { Id = Guid.NewGuid(), Name = "testType1" });
    //context.ProductTypes.Add(new EfiritPro.Retail.ProductModule.Models.ProductType() { Id = Guid.NewGuid(), Name = "testType2" });
    //context.ProductTypes.Add(new EfiritPro.Retail.ProductModule.Models.ProductType() { Id = Guid.NewGuid(), Name = "testType3" });
    //context.ProductTypes.Add(new EfiritPro.Retail.ProductModule.Models.ProductType() { Id = Guid.NewGuid(), Name = "testType4" });
    //context.ProductTypes.Add(new EfiritPro.Retail.ProductModule.Models.ProductType() { Id = Guid.NewGuid(), Name = "testType5" });
    //context.ProductTypes.Add(new EfiritPro.Retail.ProductModule.Models.ProductType() { Id = Guid.NewGuid(), Name = "testType6" });

    //var tempProductList = new List<Product>();

    //for (int i = 0; i < 20; i++)
    //{
    //    var vat = new VAT
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = RandomString(random, 20),
    //        Percent = (ushort)random.Next(0, 100)
    //    };

    //    var unit = new Unit
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = RandomString(random, 20),
    //        Code = (ushort)random.Next(0, 10000)
    //    };

    //    tempProductList.Add(new Product
    //    {
    //        Id = Guid.NewGuid(),
    //        OwnerId = Guid.NewGuid(),
    //        OrganizationId = Guid.NewGuid(),
    //        Name = RandomString(random, 20),
    //        VendorCode = RandomString(random, 20),
    //        BarCode = null,
    //        Description = null,
    //        Excise = random.Next(2) == 0,
    //        Hidden = random.Next(2) == 0,
    //        PurchasePrice = (ulong)random.Next(1, 100000),
    //        SellingPrice = (ulong)random.Next(1, 100000),
    //        PromoPrice = (ulong)random.Next(1, 100000),
    //        ProductGroupId = null,
    //        //VATId = Guid.NewGuid(),
    //        ProductTypeId = GetRandomProductType(),
    //        //UnitId = Guid.NewGuid(),
    //        MarkingTypeId = null,
    //        PriceShouldBeSetInTime = null,
    //        PriceMayChangeInCashReceipt = random.Next(2) == 0,
    //        PriceMinInCashReceipt = (ulong)random.Next(1, 100000),
    //        PriceMaxInCashReceipt = (ulong)random.Next(1, 100000),
    //        //CompositionList = new List<Guid>(),
    //        ProductGroup = null,
    //        VAT = vat,
    //        //ProductType = new ProductType(),
    //        Unit = unit,
    //        MarkingType = null
    //    });
    //}

    //context.Products.AddRangeAsync(tempProductList);

    var tempGroupList = new List<ProductGroup>();
    //tempGroupList = GenerateProductGroups(random, , context);
    //var ps = context.Products.ToList();

    ////  I need to make 10 test groups
    //for (int i = 0; i < 10; i++)
    //{
    //    var tempGroup = new ProductGroup() { Name = RandomString(random, 10) };
    //    var tempGroupsProducts = new List<Product>();

    //    //SetProductsInGroup(ref tempGroup,context, random.Next(0, 5));
    //    var tempRandom = new Random();

    //    for (int j = 0; i < random.Next(0, 5); j++)
    //    {
    //        var tempProductInGroup = ps[tempRandom.Next(0, ps.Count - 1)];
    //        tempProductInGroup.ProductGroup = tempGroup;
    //        tempGroupsProducts.Add(tempProductInGroup);
    //    }

    //    tempGroup.Products = tempGroupsProducts;

    //    tempGroupList.Add(tempGroup);
    //}

    tempGroupList = GenerateProductGroups(random, context);

    //context.ProductGroups.AddRange(tempGroupList);
    //foreach (var item in tempGroupList)
    //{
    //    context.ProductGroups.Add(item);
    //}
    //context.ProductGroups.Add(new ProductGroup() { Name = "testGroup2" });    
    var testGroupForAddProduct = context.ProductGroups
        .Include(pg => pg.Products)
        .FirstOrDefault(x => x.Name == "testGroup1");
    var testProduct = context.Products
    .FirstOrDefault(x => x.Id == Guid.Parse("1844991a-195a-43a4-aace-2ef647a5344f")
    && x.OwnerId == Guid.Parse("be7282e6-f704-4f42-ab8e-413b822b1744")
    && x.OrganizationId == Guid.Parse("c9d655e6-c0a2-4e8d-8b60-5f8fab5a946b"));

    if (testProduct==null) throw new ArgumentNullException("null value in "+nameof(testProduct));

    //testGroupForAddProduct.Products.Add(context.Products.FirstOrDefault(x=>x.Name== "SrGcVx24kfae6189Fm1K"));

    context.SaveChanges();
}

List<ProductGroup> GenerateProductGroups(Random random, ProductDbContext context)
{
    List<ProductGroup> productGroups = new List<ProductGroup>();

    for (int i = 0; i < 10; i++)
    {
        var productGroup = new ProductGroup
        {
            Name = RandomString(random, 20)
        };

        // 50% шанс, что группа будет пустая
        if (random.Next(2) == 1)
        {
            int productCount = random.Next(0, 6); 
            var tempPs = new List<Product>();
            for (int j = 0; j < productCount; j++)
            {
                var tempPr = context.Products.ToList()[random.Next(0, context.Products.ToList().Count - 1)];

                //// Добавляем продукт в группу без изменения его ключевых полей и без отслеживания
                //context.Entry(tempPr).State = EntityState.Detached;
                //var tempItem = products[random.Next(products.Count)];
                tempPs.Add(tempPr);
            }
            productGroup.Products = tempPs;
        }

        productGroups.Add(productGroup);
    }

    return productGroups;
}

void SetProductsInGroup(ref ProductGroup g, ProductDbContext c, int count)
{
    var ps = c.Products.ToList();

    var tempRandom = new Random();

    for (int i = 0; i < count;i++)
    {
        g.Products.Add(ps[tempRandom.Next(0,ps.Count-1)]);
    }
}

static string RandomString(Random random, int length)
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    char[] stringChars = new char[length];

    for (int i = 0; i < length; i++)
    {
        stringChars[i] = chars[random.Next(chars.Length)];
    }

    return new string(stringChars);
}