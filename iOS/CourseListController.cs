using Foundation;
using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using UIKit;

namespace LearnTsinghua.iOS
{
    public partial class CourseListController : UITableViewController
    {
        public CourseListController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.Source = new CourseListSource();
        }
    }

    public class CourseListSource : UITableViewSource
    {
        List<string> Courses { get; set; }

        const string cellIdentifier = "CourseCell";
        // Set in the Storyboard.
       
        public CourseListSource()
        {
            Courses = new List<string> { "aaa", "bbb" }; 
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Courses.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // in a Storyboard, Dequeue will ALWAYS return a cell, 
            var cell = tableView.DequeueReusableCell(cellIdentifier);
            
            cell.TextLabel.Text = Courses[indexPath.Row];
//            if (tableItems[indexPath.Row].Done)
//                cell.Accessory = UITableViewCellAccessory.Checkmark;
//            else
//                cell.Accessory = UITableViewCellAccessory.None;
            return cell;
        }
    }
}
