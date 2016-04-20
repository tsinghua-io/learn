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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.RightBarButtonItem = EditButtonItem;

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
        }

        public override void SetEditing(bool editing, bool animated)
        {
            var source = TableView.Source as CourseListSource;

            if (editing)
            {
                base.SetEditing(editing, animated);
                source?.BeginEditing(TableView);
            }
            else
            {
                source?.EndEditing(TableView);
                base.SetEditing(editing, animated);
            }
        }

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

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            return !Editing && base.ShouldPerformSegue(segueIdentifier, sender);
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
        List<Course> shownCourses;
        const string cellIdentifier = "CourseCell";

        public CourseListSource(string semesterId)
        {
            courses = Me.Get().Attended(semesterId);
            PopulateShownCourses();
        }

        void PopulateShownCourses()
        {
            shownCourses = new List<Course>();
            foreach (var course in courses)
            {
                if (!course.Hide)
                    shownCourses.Add(course);
            }
        }

        NSIndexPath[] HiddenPaths()
        {
            var paths = new List<NSIndexPath>();

            for (int i = 0; i < courses.Count; i++)
            {
                if (courses[i].Hide)
                    paths.Add(NSIndexPath.FromRowSection((nint)i, 0));
            }

            return paths.ToArray();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return shownCourses.Count;
        }

        public Course GetCourse(NSIndexPath indexPath)
        {
            return shownCourses[indexPath.Row];
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

        public void BeginEditing(UITableView tableView)
        {
            // Select all.
            for (int i = 0; i < shownCourses.Count; i++)
                tableView.SelectRow(NSIndexPath.FromRowSection((nint)i, 0), true, UITableViewScrollPosition.None);

            var hidden = HiddenPaths();
            tableView.BeginUpdates();
            tableView.InsertRows(hidden, UITableViewRowAnimation.Top);
            shownCourses = courses;
            tableView.EndUpdates();
        }

        public void EndEditing(UITableView tableView)
        {
            foreach (var course in courses)
                course.Hide = true;
            if (tableView.IndexPathsForSelectedRows != null)
            {
                foreach (var index in tableView.IndexPathsForSelectedRows)
                    courses[index.Row].Hide = false;
            }
            foreach (var course in courses)
                course.SaveHide();
            
            var hidden = HiddenPaths();
            tableView.BeginUpdates();
            tableView.DeleteRows(hidden, UITableViewRowAnimation.Top);
            PopulateShownCourses();
            tableView.EndUpdates();
        }
    }
}
