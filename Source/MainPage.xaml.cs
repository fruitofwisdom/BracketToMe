﻿using System;
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
				await Data.ReadFile(file);

				if (Data.Teams.Count > 0)
				{
					Results.Populate(Data);

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