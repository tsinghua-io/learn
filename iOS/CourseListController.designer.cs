// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace LearnTsinghua.iOS
{
    [Register("CourseListController")]
    partial class CourseListController
    {
        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIBarButtonItem refreshButton { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (refreshButton != null)
            {
                refreshButton.Dispose();
                refreshButton = null;
            }
        }
    }
}
