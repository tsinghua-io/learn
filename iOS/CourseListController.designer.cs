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
	[Register ("CourseListController")]
	partial class CourseListController
	{
		[Outlet]
		UIKit.UIButton SemesterButton { get; set; }

		[Action ("UnwindToCourseListController:")]
		partial void UnwindToCourseListController (UIKit.UIStoryboardSegue segue);
		
		void ReleaseDesignerOutlets ()
		{
			if (SemesterButton != null) {
				SemesterButton.Dispose ();
				SemesterButton = null;
			}
		}
	}
}
