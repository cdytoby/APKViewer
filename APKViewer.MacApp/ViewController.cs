using System;
using System.ComponentModel;
using APKViewer.Decoders;
using AppKit;
using Foundation;

namespace APKViewer.MacApp
{
	public partial class ViewController: NSViewController
	{
		private const string MY_TEST_APK = "/Volumes/MyData/Work/APKViewerData/Test.apk";

		private APKViewModel bindedViewModel;

		public ViewController(IntPtr handle): base(handle)
		{
			bindedViewModel = new APKViewModel();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Do any additional setup after loading the view.

			AppNameTextField.Cell.Title = "Text Changed.";

			bindedViewModel.PropertyChanged += BindedViewModel_PropertyChanged;

			bindedViewModel.SetDecoder(new DefaultAPKDecoder(new MacCmdProvider()), null);
			bindedViewModel.SetNewFile(new Uri(MY_TEST_APK));
		}

		private void BindedViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(nameof(APKViewModel.AppName)))
			{
				AppNameTextField.Cell.Title = bindedViewModel.AppName;
			}
		}

		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}
			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}