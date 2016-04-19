// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using LearnTsinghua.Extensions;
using LearnTsinghua.Models;

namespace LearnTsinghua.iOS
{
    public partial class CourseMaterialsController : UIViewController
    {
        public Course Course { get; set; }

        public CourseMaterialsController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.Title = Course?.Name;
            SegmentedControl.ValueChanged += (sender, e) =>
            {
                Console.WriteLine("Course SegmentedControl value changed.");
                var source = TableView.Source as CourseMaterialsSource;
                if (source != null)
                {
                    source.Segment = (int)SegmentedControl.SelectedSegment;
                    TableView.ReloadData();
                }
            };
            
            TableView.EstimatedRowHeight = 50;
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.Source = new CourseMaterialsSource();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var source = TableView.Source as CourseMaterialsSource;
            source?.Populate(Course);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "CourseAnnouncementSegue")
            {
                var dest = segue.DestinationViewController as AnnouncementController;
                if (dest != null)
                {
                    var source = TableView.Source as CourseMaterialsSource;
                    var rowPath = TableView.IndexPathForSelectedRow;
                    dest.Announcement = source.Announcements[rowPath.Row];
                }
            }
        }
    }

    public class CourseMaterialsSource : UITableViewSource
    {
        public List<Announcement> Announcements { get; set; }

        public List<File> Files { get; set; }

        public List<Assignment> Assignments { get; set; }

        public int Segment { get; set; }

        const string announcementCellIdentifier = "CourseAnnouncementCell";
        const string fileCellIdentifier = "CourseFileCell";
        const string assignmentCellIdentifier = "CourseAssignmentCell";

        public CourseMaterialsSource()
        {
            Announcements = new List<Announcement>();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            switch (Segment)
            {
                case 0:
                    return Announcements.Count;
                case 1:
                    return Files.Count;
                case 2:
                    return Assignments.Count;
                default:
                    return 0;
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            switch (Segment)
            {
                case 0:
                    {
                        var cell = tableView.DequeueReusableCell(announcementCellIdentifier);
                        var annc = Announcements[indexPath.Row];
                        cell.TextLabel.Text = annc.Title;
                        cell.DetailTextLabel.Text = annc.BodyText().Oneliner();
                        return cell;
                    }
                case 1:
                    {
                        var cell = tableView.DequeueReusableCell(fileCellIdentifier) as CourseFileCell;
                        var file = Files[indexPath.Row];
                        cell.Populate(file);
                        return cell;
                    }
                case 2:
                    {
                        var cell = tableView.DequeueReusableCell(assignmentCellIdentifier);
                        var assignment = Assignments[indexPath.Row];
                        cell.TextLabel.Text = assignment.Title;
                        cell.DetailTextLabel.Text = assignment.BodyText().Oneliner();
                        return cell;
                    }
                default:
                    return null;
            }
        }

        public void Populate(Course course)
        {
            Announcements = course.Announcements();
            Files = course.Files();
            Assignments = course.Assignments();
        }
    }
}
