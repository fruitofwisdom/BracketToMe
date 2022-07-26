using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace BracketToMe
{
	// The main page for tournament results and interacting with the simulation.
	public sealed partial class MainPage : Page
	{
		private AppWindow AdjustWeightsWindow;
		private TeamData Data = new TeamData();
		public TournamentResults Results = new TournamentResults();

		public MainPage()
		{
			this.InitializeComponent();

			// Attempt to set a preferred size for ourselves.
			ApplicationView.PreferredLaunchViewSize = new Size(1250, 750);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
		}

		private async void QuitClick(object sender, RoutedEventArgs e)
		{
			if (AdjustWeightsWindow != null)
			{
				await AdjustWeightsWindow.CloseAsync();
			}
			Application.Current.Exit();
		}

		private async void OpenTeamDataClick(object sender, RoutedEventArgs e)
		{
			FileOpenPicker picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			picker.FileTypeFilter.Add(".csv");

			StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				StorageApplicationPermissions.FutureAccessList.Add(file);
				await Data.ReadFile(file);

				if (Data.Teams.Count > 0)
				{
					Results.Populate(Data);
					SimulateTournament();
				}
			}
			else
			{
				// Do something else?
			}
		}

		private async void AdjustWeightsClick(object sender, RoutedEventArgs e)
		{
			AdjustWeightsWindow = await AppWindow.TryCreateAsync();

			Frame adjustWeightsFrame = new Frame();
			adjustWeightsFrame.Navigate(typeof(AdjustWeightsPage), this);

			ElementCompositionPreview.SetAppWindowContent(AdjustWeightsWindow, adjustWeightsFrame);

			// Attempt to size the window to the actual canvas size.
			AdjustWeightsWindow.RequestSize(new Size(500, 1155));
			AdjustWeightsWindow.Closed += delegate
			{
				adjustWeightsFrame.Content = null;
				AdjustWeightsWindow = null;
			};

			await AdjustWeightsWindow.TryShowAsync();
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
