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
			DependencyProperty.Register("Result", typeof(string), typeof(ResultControl),
			new PropertyMetadata(null, new PropertyChangedCallback(OnResultChanged)));
		public string Result
		{
			get { return (string)GetValue(ResultProperty); }
			set { SetValue(ResultProperty, value); }
		}

		public ResultControl()
		{
			this.InitializeComponent();
		}

		private static void OnResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResultControl control = (ResultControl)d;
			TextBlock text = (TextBlock)control.FindName("ResultText");
			text.Text = control.Result;
		}
	}
}
