using Microsoft.Maui.Devices.Sensors;
using MauiApp1.Resources.Strings;
using MauiApp1.Services;

namespace MauiApp1;

public partial class ActivityPage : ContentPage
{
    private int todaySteps = 0;
    private int todayDistance = 0;
    private bool isMonitoring = false;
    private double lastMagnitude = 0;
    private const double StepThreshold = 1.2;
    private const int StepGoal = 10000;
    private const double AverageStepLength = 0.78;

    public ActivityPage()
    {
        InitializeComponent();
        LoadPersistedData();
        InitializeUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartPedometerMonitoring();
    }

    private void LoadPersistedData()
    {
        var lastDateStr = Preferences.Get("LastStepDate", string.Empty);
        DateTime lastDate = DateTime.MinValue;
        
        if (!string.IsNullOrEmpty(lastDateStr))
        {
            DateTime.TryParse(lastDateStr, out lastDate);
        }
        
        if (lastDate.Date == DateTime.Today)
        {
            todaySteps = int.TryParse(Preferences.Get("TodaySteps", "0"), out var steps) ? steps : 0;
            todayDistance = int.TryParse(Preferences.Get("TodayDistance", "0"), out var distance) ? distance : 0;
        }
        else
        {
            todaySteps = 0;
            todayDistance = 0;
            Preferences.Set("LastStepDate", DateTime.Today.ToString("O"));
        }
    }

    private void InitializeUI()
    {
        HeaderLabel.Text = AppResources.Activity_Header;
        WeeklyTitle.Text = AppResources.Activity_WeeklyTitle;
        WeeklySubtitle.Text = AppResources.Activity_WeeklySubtitle;
        SectionTitle.Text = AppResources.Activity_TodayStats;
        
        StepsLabel.Text = AppResources.Activity_TotalSteps;
        StepsValue.Text = todaySteps.ToString();
        UpdateStepsGoalText();
        
        HeartRateLabel.Text = "Heart Rate";
        HeartRateValue.Text = AppResources.Common_NoData;
        HeartRateDetail.Text = "bpm";
        
        ActiveMinutesLabel.Text = AppResources.Activity_ActiveMinutes;
        ActiveMinutesValue.Text = AppResources.Common_Zero;
        ActiveMinutesDetail.Text = AppResources.Activity_Min;

        Section2Title.Text = "Connected Devices";
        DeviceTitle.Text = "No device connected";
        DeviceSubtitle.Text = "Connect a fitness tracker";

        int totalPoints = QuestCompletionService.GetTotalPoints();
        HighlightTitle.Text = AppResources.Activity_PointsEarned;
        HighlightIcon.Text = AppResources.Activity_StarIcon;
        HighlightValue.Text = string.Format(AppResources.Activity_PointsValue, totalPoints);
        HighlightDetail1.Text = AppResources.Activity_PointsFromQuests;
        HighlightDetail2.Text = string.Empty;
        HighlightDetail3.Text = string.Empty;
    }

    private async void StartPedometerMonitoring()
    {
        try
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                    Accelerometer.Default.Start(SensorSpeed.UI);
                    isMonitoring = true;
                }
            }
            else
            {
                await DisplayAlertAsync(
                    AppResources.Activity_NotSupported,
                    AppResources.Activity_AccelerometerUnavailable,
                    AppResources.Common_OK);
            }
        }
        catch (Exception ex)
        {
               await DisplayAlertAsync(
                AppResources.Activity_Error,
                string.Format(AppResources.Activity_PedometerFailed, ex.Message),
                AppResources.Common_OK);
        }
    }

    private void Accelerometer_ReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var data = e.Reading;
        double magnitude = Math.Sqrt(
            data.Acceleration.X * data.Acceleration.X +
            data.Acceleration.Y * data.Acceleration.Y +
            data.Acceleration.Z * data.Acceleration.Z);

        if (Math.Abs(magnitude - lastMagnitude) > StepThreshold)
        {
            todaySteps++;
            todayDistance = (int)(todaySteps * AverageStepLength);
            
            Preferences.Set("TodaySteps", todaySteps.ToString());
            Preferences.Set("TodayDistance", todayDistance.ToString());
            
            MainThread.BeginInvokeOnMainThread(UpdateStepsDisplay);
        }

        lastMagnitude = magnitude;
    }

    private void UpdateStepsDisplay()
    {
        StepsValue.Text = todaySteps.ToString();
        UpdateStepsGoalText();
    }

    private void UpdateStepsGoalText()
    {
        int remaining = Math.Max(0, StepGoal - todaySteps);
        StepsDetail.Text = remaining > 0 
            ? string.Format(AppResources.Activity_StepsRemaining, StepGoal, remaining)
            : AppResources.Activity_GoalReached;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        Preferences.Set("TodaySteps", todaySteps.ToString());
        Preferences.Set("TodayDistance", todayDistance.ToString());
        Preferences.Set("LastStepDate", DateTime.Today.ToString("O"));
        
        if (isMonitoring && Accelerometer.Default.IsMonitoring)
        {
            Accelerometer.Default.Stop();
            Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
        }
    }
}