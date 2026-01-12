namespace MauiApp1.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CurrentStreak { get; set; }
    public int TotalSteps { get; set; }
}