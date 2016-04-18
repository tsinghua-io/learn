using Foundation;
using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using UIKit;
using LearnTsinghua.Models;
using LearnTsinghua.Services;

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
            RefreshControl.ValueChanged += (sender, e) =>
            {
                var source = TableView.Source as CourseListSource;
                source?.Refresh();
                RefreshControl.EndRefreshing();
            };
            
//            refreshButton.Clicked += async (sender, e) =>
//            {
//                await Semester.Update();
//                await Me.Update();
//                await Me.UpdateAttended();
//                await Me.UpdateMaterials();
//            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.Source = new CourseListSource();
        }
    }

    public class CourseListSource : UITableViewSource
    {
        List<Course> Courses { get; set; }

        const string cellIdentifier = "CourseCell";
        // Set in the Storyboard.
       
        public CourseListSource()
        {
            Refresh();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Courses.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // in a Storyboard, Dequeue will ALWAYS return a cell, 
            var cell = tableView.DequeueReusableCell(cellIdentifier);
            var course = Courses[indexPath.Row];
            cell.TextLabel.Text = course.Name;

            string location = (course.Schedules != null && course.Schedules.Count > 0) ?
                course.Schedules[0].Location :
                "";
            cell.DetailTextLabel.Text = location;
            
            return cell;
        }

        public void Refresh()
        {
            var semesterId = Semester.Get().Id;
            Courses = Me.Get().Attended(semesterId);
        }
    }
}
