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
	[Register ("CourseAnnouncementCell")]
	partial class CourseAnnouncementCell
	{
		[Outlet]
		UIKit.UILabel CreatedAt { get; set; }

		[Outlet]
		UIKit.UILabel Detail { get; set; }

		[Outlet]
		UIKit.UILabel Title { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}

			if (CreatedAt != null) {
				CreatedAt.Dispose ();
				CreatedAt = null;
			}

			if (Detail != null) {
				Detail.Dispose ();
				Detail = null;
			}
		}
	}
}
