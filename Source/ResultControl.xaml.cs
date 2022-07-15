using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// This custom control allows for a uniform presentation of results that look like a normal
// TextBox, but behave much more like a fancy TextBlock. The User Control item template is
// documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BracketToMe
{
	public sealed partial class ResultControl : UserControl
	{
		public static readonly DependencyProperty ResultProperty =
			DependencyProperty.Register("Result", typeof(Result), typeof(ResultControl),
			new PropertyMetadata(null, new PropertyChangedCallback(OnResultChanged)));
		public Result Result
		{
			get { return (Result)GetValue(ResultProperty); }
			set { SetValue(ResultProperty, value); }
		}

		public ResultControl()
		{
			this.InitializeComponent();
		}

		private static void OnResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResultControl control = (ResultControl)d;
			// If no team is in the results, then no results have been calculated yet.
			if (control.Result.Team != null)
			{
				TextBlock seed = control.FindName("SeedText") as TextBlock;
				seed.Text = control.Result.Seed.ToString();
				TextBlock team = control.FindName("TeamText") as TextBlock;
				team.Text = control.Result.Team;
				TextBlock score = control.FindName("ScoreText") as TextBlock;
				score.Text = control.Result.Score.ToString();
			}
		}
	}
}
