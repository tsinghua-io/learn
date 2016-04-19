// WARNING
//
// This file has been generated automatically by Xamarin Studio Business to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LearnTsinghua.iOS
{
	[Register ("AnnouncementController")]
	partial class AnnouncementController
	{
		[Outlet]
		UIKit.UILabel Subtitle { get; set; }

		[Outlet]
		UIKit.UILabel Title { get; set; }

		[Outlet]
		UIKit.UIWebView Webview { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Webview != null) {
				Webview.Dispose ();
				Webview = null;
			}

			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}

			if (Subtitle != null) {
				Subtitle.Dispose ();
				Subtitle = null;
			}
		}
	}
}
