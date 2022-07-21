using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BracketToMe
{
	/// <summary>
	/// The main page for tournament results and interacting with the simulation.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private TeamData Data = new TeamData();
		public TournamentResults Results = new TournamentResults();
		private Windows.UI.WindowManagement.AppWindow adjustWeightsWindow;

		public MainPage()
		{
			this.InitializeComponent();
		}

		private async void QuitClick(object sender, RoutedEventArgs e)
		{
			if (adjustWeightsWindow != null)
			{
				await adjustWeightsWindow.CloseAsync();
			}
			Application.Current.Exit();
		}

		private async void OpenTeamDataClick(object sender, RoutedEventArgs e)
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
				await Data.ReadFile(file);

				if (Data.Teams.Count > 0)
				{
					Results.Populate(Data);
					SimulateTournament();
				}
			}
			else
			{
				// Do something else
			}
		}

		private async void AdjustWeightsClick(object sender, RoutedEventArgs e)
		{
			adjustWeightsWindow = await Windows.UI.WindowManagement.AppWindow.TryCreateAsync();

			Frame adjustWeightsFrame = new Frame();
			adjustWeightsFrame.Navigate(typeof(AdjustWeightsPage), this);

			Windows.UI.Xaml.Hosting.ElementCompositionPreview.SetAppWindowContent(adjustWeightsWindow, adjustWeightsFrame);

			adjustWeightsWindow.RequestSize(new Windows.Foundation.Size(500, 590));
			adjustWeightsWindow.Closed += delegate
			{
				adjustWeightsFrame.Content = null;
				adjustWeightsWindow = null;
			};

			await adjustWeightsWindow.TryShowAsync();
		}

		public void SimulateTournament()
		{
			if (Results.Results.Count > 0)
			{
				Results.SimulateTournament(Data);
			}
		}
	}
}
