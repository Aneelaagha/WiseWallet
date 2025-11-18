using Microsoft.EntityFrameworkCore;
using WiseWallet.Data;
using WiseWallet.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Load PostgreSQL connection string safely
// - On Render: from env var DATABASE_URL
// - Local: from appsettings.json -> "ConnectionStrings:DefaultConnection"
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL") ??
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Database connection string not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ⭐ Ensure database + tables exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// MIDDLEWARE
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Root check
app.MapGet("/", () => Results.Ok(new { message = "WiseWallet API is running" }));

// ----------------------
// 📌 API ENDPOINTS
// ----------------------

// Get all subscriptions
app.MapGet("/api/subscriptions", async (AppDbContext db) =>
{
    var subs = await db.Subscriptions
        .OrderBy(s => s.MerchantName)
        .ToListAsync();

    return Results.Ok(subs);
});

// Create subscription
app.MapPost("/api/subscriptions", async (AppDbContext db, Subscription input) =>
{
    var sub = new Subscription
    {
        Id = Guid.NewGuid(),
        MerchantName = input.MerchantName,
        Category = string.IsNullOrWhiteSpace(input.Category) ? "General" : input.Category,
        Amount = input.Amount,
        PreviousAmount = input.PreviousAmount,
        BillingInterval = input.BillingInterval,
        Status = input.Status,
        CreatedAt = DateTime.UtcNow,
        NextBillingDate = input.NextBillingDate,
        HasPriceIncreased = false
    };

    sub.MonthlyEquivalent = sub.BillingInterval switch
    {
        "Yearly" => Math.Round(sub.Amount / 12m, 2),
        _ => sub.Amount
    };

    db.Subscriptions.Add(sub);
    await db.SaveChangesAsync();

    return Results.Created($"/api/subscriptions/{sub.Id}", sub);
});

// Update subscription
app.MapPut("/api/subscriptions/{id:guid}", async (AppDbContext db, Guid id, Subscription update) =>
{
    var existing = await db.Subscriptions.FindAsync(id);
    if (existing is null)
        return Results.NotFound();

    existing.PreviousAmount = existing.Amount;
    existing.Amount = update.Amount;
    existing.Category = string.IsNullOrWhiteSpace(update.Category) ? existing.Category : update.Category;
    existing.BillingInterval = update.BillingInterval;
    existing.Status = update.Status;
    existing.NextBillingDate = update.NextBillingDate;

    existing.MonthlyEquivalent = existing.BillingInterval switch
    {
        "Yearly" => Math.Round(existing.Amount / 12m, 2),
        _ => existing.Amount
    };

    existing.HasPriceIncreased = existing.PreviousAmount > 0 && existing.Amount > existing.PreviousAmount;

    await db.SaveChangesAsync();
    return Results.Ok(existing);
});

// Insights overview
app.MapGet("/api/insights/overview", async (AppDbContext db) =>
{
    var subs = await db.Subscriptions.ToListAsync();
    if (subs.Count == 0)
    {
        return Results.Ok(new
        {
            totalSubscriptions = 0,
            activeSubscriptions = 0,
            cancelledSubscriptions = 0,
            monthlySpend = 0m,
            annualizedSpend = 0m,
            priceIncreasesDetected = 0,
            upcomingRenewals = 0
        });
    }

    var active = subs.Where(s => s.Status == "Active").ToList();
    var cancelled = subs.Where(s => s.Status == "Cancelled").ToList();

    decimal monthlySpend = active.Sum(s => s.MonthlyEquivalent);
    decimal annualizedSpend = Math.Round(monthlySpend * 12m, 2);

    int priceIncreases = active.Count(s => s.HasPriceIncreased);
    DateTime cutoff = DateTime.UtcNow.Date.AddDays(30);
    int upcomingRenewals = active.Count(s => s.NextBillingDate.HasValue && s.NextBillingDate.Value.Date <= cutoff);

    return Results.Ok(new
    {
        totalSubscriptions = subs.Count,
        activeSubscriptions = active.Count,
        cancelledSubscriptions = cancelled.Count,
        monthlySpend = Math.Round(monthlySpend, 2),
        annualizedSpend,
        priceIncreasesDetected = priceIncreases,
        upcomingRenewals
    });
});

// Seed sample data
app.MapPost("/api/dev/seed", async (AppDbContext db) =>
{
    if (await db.Subscriptions.AnyAsync())
        return Results.BadRequest("Sample data already exists.");

    var now = DateTime.UtcNow;

    var samples = new List<Subscription>
    {
        new Subscription
        {
            Id = Guid.NewGuid(),
            MerchantName = "Netflix",
            Category = "Streaming",
            Amount = 19.99m,
            PreviousAmount = 15.99m,
            BillingInterval = "Monthly",
            Status = "Active",
            CreatedAt = now.AddMonths(-8),
            NextBillingDate = now.AddDays(10),
            HasPriceIncreased = true,
            MonthlyEquivalent = 19.99m
        },
        new Subscription
        {
            Id = Guid.NewGuid(),
            MerchantName = "Spotify",
            Category = "Music",
            Amount = 9.99m,
            PreviousAmount = 9.99m,
            BillingInterval = "Monthly",
            Status = "Active",
            CreatedAt = now.AddYears(-1),
            NextBillingDate = now.AddDays(5),
            HasPriceIncreased = false,
            MonthlyEquivalent = 9.99m
        },
        new Subscription
        {
            Id = Guid.NewGuid(),
            MerchantName = "Adobe Creative Cloud",
            Category = "Productivity",
            Amount = 239.88m,
            PreviousAmount = 199.99m,
            BillingInterval = "Yearly",
            Status = "Active",
            CreatedAt = now.AddYears(-2),
            NextBillingDate = now.AddDays(25),
            HasPriceIncreased = true,
            MonthlyEquivalent = Math.Round(239.88m / 12m, 2)
        },
        new Subscription
        {
            Id = Guid.NewGuid(),
            MerchantName = "Disney+",
            Category = "Streaming",
            Amount = 7.99m,
            PreviousAmount = 7.99m,
            BillingInterval = "Monthly",
            Status = "Cancelled",
            CreatedAt = now.AddMonths(-6),
            NextBillingDate = null,
            HasPriceIncreased = false,
            MonthlyEquivalent = 7.99m
        }
    };

    await db.Subscriptions.AddRangeAsync(samples);
    await db.SaveChangesAsync();

    return Results.Ok(samples);
});

// Run app
app.Run();
