using System.Net.Http.Json;

namespace FitFusionMobile.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text ?? "";
        var password = PasswordEntry.Text ?? "";

        // simple UI validation (same idea as backend validation)
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageLabel.Text = "Email and Password are required.";
            return;
        }

        try
        {
            // This must match the port shown when my API runs (mine is 5029)
            var apiBaseUrl = "http://localhost:5029";

            using var client = new HttpClient();

            var response = await client.PostAsJsonAsync(
                $"{apiBaseUrl}/auth/login",
                new { email, password }
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

    private async void OnSignupTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SignupPage));
    }
}