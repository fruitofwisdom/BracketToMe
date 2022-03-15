using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BracketToMe
{
    /// <summary>
    /// The main page for the tournament results and interacted with the simulation.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TeamData teamData = new TeamData();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OpenTeamDataButtonClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".csv");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
                await teamData.ReadFile(file);

                if (teamData.teams.Count > 0)
                {
                    Button runSimulationButton = FindName("RunSimulationButton") as Button;
                    runSimulationButton.IsEnabled = true;
                }
            }
            else
            {
                // Do something else
            }
        }

        private void RunSimulationButtonClick(object sender, RoutedEventArgs e)
        {
            ;
        }
    }
}
