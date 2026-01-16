namespace FitFusionSecurityPrototype.Services
{
    public class InputSanitizer
    {
        /// <summary>
        /// Simple defensive sanitization for free-text fields.
        /// For a student project, we:
        /// - Trim whitespace
        /// - Remove angle brackets to reduce HTML/script injection payloads
        /// - Normalize line endings
        /// </summary>
        public string SanitizeFreeText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var cleaned = input.Trim();

            // Reduce simple HTML/script payloads (e.g., <script>alert(1)</script>)
            cleaned = cleaned.Replace("<", "").Replace(">", "");

            // Normalize newlines for consistent storage/display
            cleaned = cleaned.Replace("\r\n", "\n");

            return cleaned;
        }
    }
}