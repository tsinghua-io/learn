// This file has been autogenerated from a class added in the UI designer.

using System;
using System.IO;

using Foundation;
using UIKit;

using LearnTsinghua.Extensions;
using LearnTsinghua.Models;

namespace LearnTsinghua.iOS
{
    public partial class AnnouncementController : UIViewController
    {
        const string cssFile = "announcement.css";

        public Announcement Announcement { get; set; }

        public AnnouncementController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title.Text = Announcement?.Title;
            Subtitle.Text = string.Format("{0} 发布于 {1}", Announcement?.Owner?.Name, Announcement?.CreatedAt);

            var contentDir = Path.Combine(NSBundle.MainBundle.BundlePath, "Content/");
            var html = Announcement?.Body?.WrapWithCss(cssFile);
            Webview.LoadHtmlString(html, new NSUrl(contentDir, true));
        }
    }
}
