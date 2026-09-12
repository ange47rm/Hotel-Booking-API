using HotelBooking.Infrastructure.Seeding;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("testing")]
public class TestingController : ControllerBase
{
    private readonly TestDataSeeder _seeder;

    public TestingController(TestDataSeeder seeder)
    {
        _seeder = seeder;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        await _seeder.SeedAsync();

        return Ok();
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _seeder.ResetAsync();

        return Ok();
    }
}