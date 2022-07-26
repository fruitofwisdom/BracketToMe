using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BracketToMe
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AdjustWeightsPage : Page
	{
		private MainPage MainPage;

		public AdjustWeightsPage()
		{
			this.InitializeComponent();
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
			if (MainPage != null)
			{
				MainPage.SimulateTournament();
			}
		}
	}

	// Convert a double to a string (and back) with the formatting we desire.
	public class WeightFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return String.Format("{0:N2}", value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return Double.Parse((string)value);
		}
	}
}
