using MauiApp1.Models;
using System.Text.Json;

namespace MauiApp1.Services;  // Changed from MauiApp1 to MauiApp1.Services

public class QuestCompletionService
{
    private static readonly List<Quest> AvailableQuests = new()
    {
        new Quest 
        { 
            Id = "walk_3000_steps",
            Title = "Morning Walker",
            Description = "Walk 3,000 steps",
            Type = QuestType.Steps,
            TargetValue = 3000,
            PointsReward = 15,
            Icon = "🚶"
        },
        new Quest 
        { 
            Id = "walk_5km",
            Title = "Distance Champion",
            Description = "Walk 5 kilometers",
            Type = QuestType.Distance,
            TargetValue = 5000,
            PointsReward = 25,
            Icon = "🏃"
        },
        new Quest 
        { 
            Id = "walk_5000_steps",
            Title = "Step Master",
            Description = "Walk 5,000 steps",
            Type = QuestType.Steps,
            TargetValue = 5000,
            PointsReward = 25,
            Icon = "👟"
        },
        new Quest 
        { 
            Id = "walk_1km",
            Title = "Quick Stroll",
            Description = "Walk 1 kilometer",
            Type = QuestType.Distance,
            TargetValue = 1000,
            PointsReward = 5,
            Icon = "🚶‍♂️"
        },
        new Quest 
        { 
            Id = "run_2km_15min",
            Title = "Speed Runner",
            Description = "Run 2km in 15 minutes",
            Type = QuestType.TimedRun,
            TargetValue = 2000,
            TimeLimit = 15,
            PointsReward = 30,
            Icon = "⚡"
        },
        new Quest 
        { 
            Id = "walk_1km_casual",
            Title = "Casual Walker",
            Description = "Walk 1 kilometer",
            Type = QuestType.Distance,
            TargetValue = 1000,
            PointsReward = 5,
            Icon = "🌳"
        },
        new Quest 
        { 
            Id = "walk_2000_steps",
            Title = "Step Starter",
            Description = "Walk 2,000 steps",
            Type = QuestType.Steps,
            TargetValue = 2000,
            PointsReward = 10,
            Icon = "🎯"
        }
    };

    public static List<Quest> GetDailyQuests()
    {
        var lastQuestDateStr = Preferences.Get("LastQuestDate", string.Empty);
        DateTime lastQuestDate = DateTime.MinValue;
        
        if (!string.IsNullOrEmpty(lastQuestDateStr))
        {
            DateTime.TryParse(lastQuestDateStr, out lastQuestDate);
        }
        
        if (lastQuestDate.Date != DateTime.Today)
        {
            GenerateNewDailyQuests();
        }
        
        var dailyQuests = new List<Quest>();
        for (int i = 0; i < 3; i++)
        {
            var questId = Preferences.Get($"DailyQuest_{i}", string.Empty);
            if (!string.IsNullOrEmpty(questId))
            {
                var quest = AvailableQuests.FirstOrDefault(q => q.Id == questId);
                if (quest != null)
                {
                    var questCopy = CreateQuestCopy(quest);
                    UpdateQuestProgress(questCopy);
                    dailyQuests.Add(questCopy);
                }
            }
        }
        
        return dailyQuests;
    }
    
    private static Quest CreateQuestCopy(Quest original)
    {
        var copy = new Quest
        {
            Id = original.Id,
            Title = original.Title,
            Description = original.Description,
            Type = original.Type,
            TargetValue = original.TargetValue,
            TimeLimit = original.TimeLimit,
            PointsReward = original.PointsReward,
            Icon = original.Icon,
            IsCompleted = Preferences.Get($"Quest_{original.Id}_Completed", false)
        };
        
        // Load completion timestamp if available
        var completedDateStr = Preferences.Get($"Quest_{original.Id}_CompletedDate", string.Empty);
        if (!string.IsNullOrEmpty(completedDateStr) && DateTime.TryParse(completedDateStr, out var completedDate))
        {
            copy.CompletedAt = completedDate;
        }
        
        return copy;
    }
    
    private static void GenerateNewDailyQuests()
    {
        // Reset all quest completions for new day
        foreach (var quest in AvailableQuests)
        {
            Preferences.Remove($"Quest_{quest.Id}_Completed");
            Preferences.Remove($"Quest_{quest.Id}_CompletedDate");
        }
        
        // Select 3 random quests
        var random = new Random();
        var selectedQuests = AvailableQuests
            .OrderBy(q => random.Next())
            .Take(3)
            .ToList();
        
        // Save selected quests
        for (int i = 0; i < selectedQuests.Count; i++)
        {
            Preferences.Set($"DailyQuest_{i}", selectedQuests[i].Id);
        }
        
        Preferences.Set("LastQuestDate", DateTime.Today.ToString("O"));
        
        // Increment daily quest generation counter - using string to avoid type issues
        int totalDays = GetIntPreference("TotalDaysActive", 0);
        SetIntPreference("TotalDaysActive", totalDays + 1);
    }
    
    private static void UpdateQuestProgress(Quest quest)
    {
        switch (quest.Type)
        {
            case QuestType.Steps:
                quest.CurrentProgress = GetIntPreference("TodaySteps", 0);
                break;
            case QuestType.Distance:
                quest.CurrentProgress = GetIntPreference("TodayDistance", 0);
                break;
            case QuestType.TimedRun:
                quest.CurrentProgress = GetIntPreference($"Quest_{quest.Id}_Progress", 0);
                break;
        }
    }
    
    public static void CompleteQuest(string questId, int pointsAwarded)
    {
        // Mark quest as completed
        Preferences.Set($"Quest_{questId}_Completed", true);
        Preferences.Set($"Quest_{questId}_CompletedDate", DateTime.Now.ToString("O")); // ISO 8601 format
        
        // Award points
        int currentPoints = GetIntPreference("TotalPoints", 0);
        SetIntPreference("TotalPoints", currentPoints + pointsAwarded);
        
        // Track completion statistics
        int totalQuestsCompleted = GetIntPreference("TotalQuestsCompleted", 0);
        SetIntPreference("TotalQuestsCompleted", totalQuestsCompleted + 1);
        
        // Track daily completion count
        int todayCompleted = GetIntPreference($"QuestsCompleted_{DateTime.Today:yyyy-MM-dd}", 0);
        SetIntPreference($"QuestsCompleted_{DateTime.Today:yyyy-MM-dd}", todayCompleted + 1);
    }
    
    public static bool IsQuestCompleted(string questId)
    {
        return Preferences.Get($"Quest_{questId}_Completed", false);
    }
    
    public static int GetTotalPoints()
    {
        return GetIntPreference("TotalPoints", 0);
    }
    
    public static QuestStatistics GetStatistics()
    {
        return new QuestStatistics
        {
            TotalPoints = GetTotalPoints(),
            TotalQuestsCompleted = GetIntPreference("TotalQuestsCompleted", 0),
            TotalDaysActive = GetIntPreference("TotalDaysActive", 0),
            TodayQuestsCompleted = GetIntPreference($"QuestsCompleted_{DateTime.Today:yyyy-MM-dd}", 0)
        };
    }
    
    // Helper methods to safely store and retrieve integers as strings
    private static void SetIntPreference(string key, int value)
    {
        Preferences.Set(key, value.ToString());
    }
    
    private static int GetIntPreference(string key, int defaultValue)
    {
        var strValue = Preferences.Get(key, defaultValue.ToString());
        return int.TryParse(strValue, out var result) ? result : defaultValue;
    }
}

public class QuestStatistics
{
    public int TotalPoints { get; set; }
    public int TotalQuestsCompleted { get; set; }
    public int TotalDaysActive { get; set; }
    public int TodayQuestsCompleted { get; set; }
}
