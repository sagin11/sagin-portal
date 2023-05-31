
using System.Text;

namespace SaginPortal.Packages;

public class RandomStringGenerator {
    private readonly Random random;
    private const string AllowedCharacters = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";

    public RandomStringGenerator() {
        random = new Random();
    }

    public string GenerateRandomString() {
        var length = random.Next(5, 10);
        var stringBuilder = new StringBuilder(length);
        var allowedCharactersLength = AllowedCharacters.Length;

        for (var i = 0; i < length; i++) {
            var randomIndex = random.Next(allowedCharactersLength);
            var randomCharacter = AllowedCharacters[randomIndex];
            stringBuilder.Append(randomCharacter);
        }

        return stringBuilder.ToString();
    }
}
