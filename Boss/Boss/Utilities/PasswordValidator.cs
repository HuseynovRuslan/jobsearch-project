using System.Text.RegularExpressions;

namespace Boss.Utilities
{
    public static class PasswordValidator
    {
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

          
            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
           
            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
           
            bool hasDigit = Regex.IsMatch(password, @"\d");
          
            bool hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*(),.?\:{ }|<>]");
          
            bool isLongEnough = password.Length >= 8;

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar && isLongEnough;
        }
    }
}
