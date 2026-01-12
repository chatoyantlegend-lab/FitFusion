using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private int currentSteps = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private void LoadData()
    {
        var username = Preferences.Get("Username", "User");
        var currentStreak = GetIntPreference("CurrentStreak", 0);
        currentSteps = GetIntPreference("TodaySteps", 0);

        GreetingLabel.Text = $"Hey {username}!";
        SubtitleLabel.Text = "Let's reach today's goals!";

        StreakDaysLabel.Text = $"{currentStreak} Days";
        StreakLabel.Text = "Current Streak";
        NextMilestoneLabel.Text = "Next Milestone";
        
        int nextMilestone = ((currentStreak / 10) + 1) * 10;
        MilestoneDaysLabel.Text = $"{nextMilestone} Days";

        StepsValueLabel.Text = currentSteps.ToString("N0");
        StepsLabel.Text = "Steps";

        LoadTodaysChallenge();
    }

    private void LoadTodaysChallenge()
    {
        // Get the actual daily quests from the service
        var quests = QuestCompletionService.GetDailyQuests();
        
        if (quests.Count == 0)
        {
            // If no quests are available, show a default message
            TodayChallengeLabel.Text = "No challenges available";
            return;
        }

        // Calculate remaining time for each quest (until end of day)
        var timeUntilMidnight = DateTime.Today.AddDays(1) - DateTime.Now;
        foreach (var quest in quests)
        {
            quest.TimeRemaining = timeUntilMidnight;
        }

        // Select the quest with the highest points
        var maxPoints = quests.Max(q => q.PointsReward);
        var topQuests = quests.Where(q => q.PointsReward == maxPoints).ToList();
        var todaysChallenge = topQuests[Random.Shared.Next(topQuests.Count)];

        TodayChallengeLabel.Text = "Today's Challenge";
        ChallengeTypeLabel.Text = todaysChallenge.Description;
        ChallengePointsLabel.Text = $"+{todaysChallenge.PointsReward} pts";
        ChallengeTitleLabel.Text = todaysChallenge.Title;
        ProgressTextLabel.Text = "Progress";
        ProgressPercentageLabel.Text = todaysChallenge.ProgressText;

        var progressWidth = todaysChallenge.ProgressPercentage / 100.0;
        ProgressBarFill.WidthRequest = Math.Min(progressWidth * 280, 280);

        TimeRemainingLabel.Text = FormatTimeRemaining(todaysChallenge.TimeRemaining);
    }

    private string FormatTimeRemaining(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
        {
            var days = (int)timeSpan.TotalDays;
            var hours = timeSpan.Hours;
            return $"{days}d {hours}h remaining";
        }
        else if (timeSpan.TotalHours >= 1)
        {
            return $"{timeSpan.Hours}h {timeSpan.Minutes}m remaining";
        }
        else
        {
            return $"{timeSpan.Minutes}m remaining";
        }
    }

    private static int GetIntPreference(string key, int defaultValue)
    {
        var strValue = Preferences.Get(key, defaultValue.ToString());
        return int.TryParse(strValue, out var result) ? result : defaultValue;
    }
}
