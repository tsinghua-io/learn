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
	[Register ("CourseFileCell")]
	partial class CourseFileCell
	{
		[Outlet]
		UIKit.UILabel Description { get; set; }

		[Outlet]
		UIKit.UILabel FileSize { get; set; }

		[Outlet]
		UIKit.UIView Icon { get; set; }

		[Outlet]
		UIKit.UILabel Title { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Icon != null) {
				Icon.Dispose ();
				Icon = null;
			}

			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}

			if (FileSize != null) {
				FileSize.Dispose ();
				FileSize = null;
			}

			if (Description != null) {
				Description.Dispose ();
				Description = null;
			}
		}
	}
}
