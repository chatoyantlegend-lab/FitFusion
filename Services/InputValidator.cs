using System.Text.RegularExpressions;

namespace FitFusionSecurityPrototype.Services
{
    public class InputValidator
    {
        // Username: 3–20 chars, letters/numbers/underscore only
        private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]{3,20}$");

        // Basic email pattern (good for student projects; server should still validate too)
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public (bool ok, string error) ValidateUsername(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return (false, "Username is required.");

            username = username.Trim();

            if (!UsernameRegex.IsMatch(username))
                return (false, "Username must be 3–20 characters and contain only letters, numbers, or underscores.");

            return (true, "");
        }

        public (bool ok, string error) ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.");

            email = email.Trim();

            if (!EmailRegex.IsMatch(email))
                return (false, "Please enter a valid email address.");

            return (true, "");
        }

        public (bool ok, string error) ValidatePassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password is required.");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            // Beginner-friendly rules: at least one letter + one number
            if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
                return (false, "Password must include at least one letter and one number.");

            return (true, "");
        }

        public (bool ok, string error) ValidateGoals(string? goals)
        {
            if (string.IsNullOrWhiteSpace(goals))
                return (false, "Goals cannot be empty.");

            goals = goals.Trim();

            if (goals.Length > 200)
                return (false, "Goals must be 200 characters or less.");

            return (true, "");
        }
    }
}