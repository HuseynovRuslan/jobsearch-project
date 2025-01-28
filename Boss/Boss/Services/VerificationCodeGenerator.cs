public static class VerificationCodeGenerator
{
    public static string GenerateCode(int length = 6)
    {
        var random = new Random();
        const string digits = "0123456789";
        return new string(Enumerable.Repeat(digits, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
