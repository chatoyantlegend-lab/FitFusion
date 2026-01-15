using System.Net.Http.Json;

namespace FitFusionMobile.Pages;

public partial class SignupPage : ContentPage
{
    public SignupPage()
    {
        InitializeComponent();
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text ?? "";
        var email = EmailEntry.Text ?? "";
        var password = PasswordEntry.Text ?? "";

        // simple UI validation first
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            MessageLabel.Text = "Username, Email and Password are required.";
            return;
        }

        try
        {
            // Must match your API port when it runs (yours was 5029)
            var apiBaseUrl = "http://localhost:5029";

            using var client = new HttpClient();

            var response = await client.PostAsJsonAsync(
                $"{apiBaseUrl}/auth/signup",
                new { username, email, password }
            );

            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                MessageLabel.Text = $"✅ {responseText}";
            }
            else
            {
                MessageLabel.Text = $"❌ {responseText}";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"Error: {ex.Message}";
        }
    }
}