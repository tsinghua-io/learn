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
	[Register ("CourseAssignmentCell")]
	partial class CourseAssignmentCell
	{
		[Outlet]
		UIKit.UILabel CountdownLabel { get; set; }

		[Outlet]
		UIKit.UILabel DetailLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (CountdownLabel != null) {
				CountdownLabel.Dispose ();
				CountdownLabel = null;
			}

			if (DetailLabel != null) {
				DetailLabel.Dispose ();
				DetailLabel = null;
			}
		}
	}
}
