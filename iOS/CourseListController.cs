using Foundation;
using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using UIKit;

using LearnTsinghua.Extensions;
using LearnTsinghua.Models;
using LearnTsinghua.Services;

namespace LearnTsinghua.iOS
{
    public partial class CourseListController : UITableViewController
    {
        public string SemesterId { get; set; }

        public CourseListController(IntPtr handle)
            : base(handle)
        {
            SemesterId = Semester.Get().Id;
        }

        //        public override void ViewDidLoad()
        //        {
        //            base.ViewDidLoad();
        //
        //            RefreshControl.ValueChanged += (sender, e) =>
        //            {
        //                var source = TableView.Source as CourseListSource;
        //                source?.Populate();
        //                TableView.ReloadData();
        //                RefreshControl.EndRefreshing();
        //            };
        //
        //            refreshButton.Clicked += async (sender, e) =>
        //            {
        //                await Semester.Update();
        //                await Me.Update();
        //                await Me.UpdateAttended();
        //                await Me.UpdateMaterials();
        //            };
        //        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Populate();
        }

        public void Populate()
        {
            SemesterButton.SetTitle(SemesterId.SemesterString(), UIControlState.Normal);
            TableView.Source = new CourseListSource(SemesterId);
            TableView.ReloadData();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "CourseSegue")
            {
                var dest = segue.DestinationViewController as CourseMaterialsController;
                if (dest != null)
                {
                    var source = TableView.Source as CourseListSource;
                    dest.Course = source.GetCourse(TableView.IndexPathForSelectedRow);
                }
            }
            else if (segue.Identifier == "SelectSemesterSegue")
            {
                var nav = segue.DestinationViewController as UINavigationController;
                var dest = nav?.TopViewController as SemesterController;
                if (dest != null)
                {
                    dest.SelectedSemesterId = SemesterId;
                }
            }
        }

        partial void UnwindToCourseListController(UIKit.UIStoryboardSegue segue)
        {
            if (segue.Identifier == "UnwindToCourseListController")
            {
                var src = segue.SourceViewController as SemesterController;
                if (src != null && src.SelectedSemesterId != SemesterId)
                {
                    SemesterId = src.SelectedSemesterId;
                    Populate();
                }
            }
        }
    }

    public class CourseListSource : UITableViewSource
    {
        List<Course> courses;
        const string cellIdentifier = "CourseCell";

        public CourseListSource(string semesterId)
        {
            courses = Me.Get().Attended(semesterId);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return courses.Count;
        }

        public Course GetCourse(NSIndexPath indexPath)
        {
            return courses[indexPath.Row];
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier);
            var course = GetCourse(indexPath);
            cell.TextLabel.Text = course.Name;

            string location = (course.Schedules != null && course.Schedules.Count > 0) ?
                course.Schedules[0].Location :
                "";
            cell.DetailTextLabel.Text = location;
            
            return cell;
        }
    }
}
