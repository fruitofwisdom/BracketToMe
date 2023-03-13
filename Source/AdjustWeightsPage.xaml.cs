using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace BracketToMe
{
	// This page allows for the adjusting of weights by binding each slider to a public field in
	// the static Weights class. As the slider values are changed, the tournament is re-simulated.
	public sealed partial class AdjustWeightsPage : Page
	{
		private MainPage MainPage;

		public AdjustWeightsPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter is MainPage)
			{
				MainPage = e.Parameter as MainPage;
			}
			base.OnNavigatedTo(e);
		}

		private void SliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			MainPage?.SimulateTournament();
		}
	}

	// Convert a double to a string (and back) with the formatting we desire.
	public class WeightFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return string.Format("{0:N2}", value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return double.Parse((string)value);
		}
	}
}
