using MauiApp1.Models;
using MauiApp1.Services;  // Add this

namespace MauiApp1;

public partial class QuestsPage : ContentPage
{
    private List<Quest> dailyQuests = new();

    public QuestsPage()
    {
        InitializeComponent();
        LoadQuests();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshQuests();
    }

    private void LoadQuests()
    {
        dailyQuests = QuestCompletionService.GetDailyQuests();
        UpdateTotalPoints();
        DisplayQuests();
    }

    private void RefreshQuests()
    {
        dailyQuests = QuestCompletionService.GetDailyQuests();
        UpdateTotalPoints();
        DisplayQuests();
    }

    private void UpdateTotalPoints()
    {
        int totalPoints = QuestCompletionService.GetTotalPoints();
        TotalPointsLabel.Text = $"{totalPoints} pts";
    }

    private void DisplayQuests()
    {
        QuestsContainer.Children.Clear();

        foreach (var quest in dailyQuests)
        {
            var questCard = CreateQuestCard(quest);
            QuestsContainer.Children.Add(questCard);
        }
    }

    private Border CreateQuestCard(Quest quest)
    {
        var card = new Border
        {
            Stroke = quest.IsCompleted ? Colors.Green : Colors.LightGray,
            StrokeThickness = 2,
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 15),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 15 },
            BackgroundColor = Colors.White
        };

        var layout = new VerticalStackLayout { Spacing = 10 };

        // Header with icon and title
        var header = new HorizontalStackLayout { Spacing = 10 };
        header.Children.Add(new Label
        {
            Text = quest.Icon,
            FontSize = 32,
            VerticalOptions = LayoutOptions.Center
        });

        var titleLayout = new VerticalStackLayout { Spacing = 5, VerticalOptions = LayoutOptions.Center };
        titleLayout.Children.Add(new Label
        {
            Text = quest.Title,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black
        });
        titleLayout.Children.Add(new Label
        {
            Text = quest.Description,
            FontSize = 14,
            TextColor = Colors.Gray
        });
        header.Children.Add(titleLayout);

        layout.Children.Add(header);

        // Progress bar
        var progressBar = new ProgressBar
        {
            Progress = quest.ProgressPercentage / 100,
            ProgressColor = quest.IsCompleted ? Colors.Green : Colors.Blue,
            HeightRequest = 8
        };
        layout.Children.Add(progressBar);

        // Progress text
        layout.Children.Add(new Label
        {
            Text = quest.ProgressText,
            FontSize = 12,
            TextColor = Colors.Gray
        });

        // Points and status
        var footer = new HorizontalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.End
        };

        footer.Children.Add(new Label
        {
            Text = $"+{quest.PointsReward} pts",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Orange,
            VerticalOptions = LayoutOptions.Center
        });

        if (quest.IsCompleted)
        {
            footer.Children.Add(new Label
            {
                Text = "✓ Completed",
                FontSize = 14,
                TextColor = Colors.Green,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            });
        }
        else if (quest.CurrentProgress >= quest.TargetValue)
        {
            var completeButton = new Button
            {
                Text = "Complete Quest",
                BackgroundColor = Colors.Green,
                TextColor = Colors.White,
                FontSize = 14,
                Padding = new Thickness(20, 10),
                CornerRadius = 8
            };
            completeButton.Clicked += (s, e) => OnCompleteQuestClicked(quest);
            footer.Children.Add(completeButton);
        }

        layout.Children.Add(footer);
        card.Content = layout;

        return card;
    }

    private async void OnCompleteQuestClicked(Quest quest)
    {
        QuestCompletionService.CompleteQuest(quest.Id, quest.PointsReward);
        await DisplayAlert("Success!", $"Quest completed! You earned {quest.PointsReward} points!", "OK");
        RefreshQuests();
    }
}