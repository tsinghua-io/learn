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
	[Register ("CourseDoneAssignmentCell")]
	partial class CourseDoneAssignmentCell
	{
		[Outlet]
		UIKit.UILabel MarkLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (MarkLabel != null) {
				MarkLabel.Dispose ();
				MarkLabel = null;
			}
		}
	}
}
