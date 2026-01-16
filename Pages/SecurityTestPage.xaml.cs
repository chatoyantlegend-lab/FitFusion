using FitFusionSecurityPrototype.Services;

namespace FitFusionSecurityPrototype.Pages;

public partial class SecurityTestPage : ContentPage
{
    private readonly InputValidator _validator;
    private readonly InputSanitizer _sanitizer;

    public SecurityTestPage(InputValidator validator, InputSanitizer sanitizer)
    {
        InitializeComponent();
        _validator = validator;
        _sanitizer = sanitizer;
    }

    private async void OnValidateClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;
        var goalsRaw = GoalsEditor.Text;

        var (uOk, uErr) = _validator.ValidateUsername(username);
        if (!uOk) { await DisplayAlertAsync("Invalid Username", uErr, "OK"); return; }

        var (eOk, eErr) = _validator.ValidateEmail(email);
        if (!eOk) { await DisplayAlertAsync("Invalid Email", eErr, "OK"); return; }

        var (pOk, pErr) = _validator.ValidatePassword(password);
        if (!pOk) { await DisplayAlertAsync("Invalid Password", pErr, "OK"); return; }

        var goalsClean = _sanitizer.SanitizeFreeText(goalsRaw);
        var (gOk, gErr) = _validator.ValidateGoals(goalsClean);
        if (!gOk) { await DisplayAlertAsync("Invalid Goals", gErr, "OK"); return; }

        ResultLabel.Text = $"✅ Valid!\n\nSanitized Goals:\n{goalsClean}";
    }
}
