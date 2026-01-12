namespace MauiApp1.Models;

public enum QuestType
{
    Steps,
    Distance,
    TimedRun
}

public enum QuestStatus
{
    Locked,      // Not yet available
    InProgress,  // Available and being worked on
    Claimable,   // Goal reached, waiting for user to claim
    Completed    // Claimed and points awarded
}

public class Quest
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public QuestType Type { get; set; }
    public int TargetValue { get; set; }
    public int? TimeLimit { get; set; }
    public int PointsReward { get; set; }
    public string Icon { get; set; }
    public bool IsCompleted { get; set; }
    public int CurrentProgress { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public QuestStatus Status { get; }
    public string ProgressText { get; }
    public string StatusText { get; }
    public double ProgressPercentage { get; }
    public Color StatusColor { get; }

    // Add this property to fix CS1061
    public TimeSpan TimeRemaining { get; set; }
}   