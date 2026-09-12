namespace HotelBooking.Domain.ValueObjects;

public static class BookingReference
{
    private const string AllowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // excludes confusing characters O/0, I/1
    private const int ReferenceLength = 8;

    public static string Generate()
    {
        // build a sequence of random characters by picking one at a time from the allowed set
        var randomCharacters = Enumerable.Range(0, ReferenceLength)
            .Select(_ => AllowedCharacters[Random.Shared.Next(AllowedCharacters.Length)]);

        return new string(randomCharacters.ToArray());
    }
}
