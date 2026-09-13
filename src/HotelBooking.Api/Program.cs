using HotelBooking.Api.Middleware;
using HotelBooking.Application.Interfaces;
using HotelBooking.Application.UseCases;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Web API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // scans controllers/actions and builds the API description
builder.Services.AddSwaggerGen(); // turns that description into the OpenAPI JSON document

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// UseCases
builder.Services.AddScoped<FindHotelsByName>();
builder.Services.AddScoped<FindAvailableRooms>();
builder.Services.AddScoped<BookRoom>();
builder.Services.AddScoped<GetBookingByReference>();

// Seeding
builder.Services.AddScoped<TestDataSeeder>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.

// Serve OpenAPI JSON document via Swagger page
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
