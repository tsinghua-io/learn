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

        public const int AnnouncementSegmentIndex = 0;
        public const int FileSegmentIndex = 1;
        public const int AssignmentSegmentIndex = 2;

        public CourseMaterialsController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SegmentedControl.ValueChanged += (sender, e) =>
            {
                var source = TableView.Source as CourseMaterialsSource;
                if (source != null)
                {
                    source.Segment = (int)SegmentedControl.SelectedSegment;
                    TableView.ReloadData();
                }
            };
            
            TableView.EstimatedRowHeight = 50;
            TableView.RowHeight = UITableView.AutomaticDimension;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.DeselectRow(TableView.IndexPathForSelectedRow, true);

            NavigationItem.Title = Course.Name;

            SegmentedControl.SetEnabled(Course.AnnouncementIds.Count > 0, AnnouncementSegmentIndex);
            SegmentedControl.SetEnabled(Course.FileIds.Count > 0, FileSegmentIndex);
            SegmentedControl.SetEnabled(Course.AssignmentIds.Count > 0, AssignmentSegmentIndex);

            if (SegmentedControl.SelectedSegment == -1 || !SegmentedControl.IsEnabled(SegmentedControl.SelectedSegment))
                for (int i = 0; i < SegmentedControl.NumberOfSegments; i++)
                {
                    if (SegmentedControl.IsEnabled((nint)i))
                    {
                        SegmentedControl.SelectedSegment = i;
                        break;
                    }
                }
                
            TableView.Source = new CourseMaterialsSource(Course, (int)SegmentedControl.SelectedSegment);
            TableView.ReloadData();
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
                    dest.Announcement = source.GetAnnouncement(rowPath);
                }
            }
            else if (segue.Identifier == "CourseFileSegue")
            {
                var dest = segue.DestinationViewController as FileController;
                if (dest != null)
                {
                    var source = TableView.Source as CourseMaterialsSource;
                    var rowPath = TableView.IndexPathForSelectedRow;
                    dest.File = source.GetFile(rowPath);
                }
            }
        }
    }

    public class CourseMaterialsSource : UITableViewSource
    {
        public int Segment { get; set; }

        Course course;
        List<Announcement> announcements;
        List<File> files;
        List<Assignment> undoneAssignments;
        List<Assignment> doneAssignments;

        const string announcementCellIdentifier = "CourseAnnouncementCell";
        const string fileCellIdentifier = "CourseFileCell";
        const string assignmentCellIdentifier = "CourseAssignmentCell";
        const string doneAssignmentCellIdentifier = "CourseDoneAssignmentCell";

        public CourseMaterialsSource(Course course, int segment)
        {
            Segment = segment;
            // Sort ids.
            course.AnnouncementIds.Sort((lhs, rhs) => rhs.CompareTo(lhs));
            course.FileIds.Sort((lhs, rhs) => rhs.CompareTo(lhs));
            this.course = course;

            announcements = new List<Announcement>(new Announcement[course.AnnouncementIds.Count]);
            files = new List<File>(new File[course.FileIds.Count]);

            // We have to read all the assignments here to figure out dones & undones.
            course.Assignments(out undoneAssignments, out doneAssignments);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            if (course.NoMaterials())
            {
                var noDataLabel = new UILabel(tableView.Bounds);
                noDataLabel.Text = "未使用网络学堂";
                noDataLabel.TextColor = UIColor.LightGray;
                noDataLabel.Font = UIFont.PreferredTitle1;
                noDataLabel.TextAlignment = UITextAlignment.Center;
                noDataLabel.Lines = 0;

                tableView.BackgroundView = noDataLabel;
                tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

                return 0;
            }
            else
            {
                tableView.BackgroundView = null;
                tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;

                return Segment == CourseMaterialsController.AssignmentSegmentIndex ? 2 : 1;
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            switch (Segment)
            {
                case CourseMaterialsController.AnnouncementSegmentIndex:
                    return announcements.Count;
                case CourseMaterialsController.FileSegmentIndex:
                    return files.Count;
                case CourseMaterialsController.AssignmentSegmentIndex:
                    return section == 0 ? undoneAssignments.Count : doneAssignments.Count;
                default:
                    return 0;
            }
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            switch (Segment)
            {
                case CourseMaterialsController.AnnouncementSegmentIndex:
                    return null;
                case CourseMaterialsController.FileSegmentIndex:
                    return null;
                case CourseMaterialsController.AssignmentSegmentIndex:
                    return section == 0 ? "未完成" : "已完成";
                default:
                    return null;
            }
        }

        public Announcement GetAnnouncement(NSIndexPath indexPath)
        {
            return announcements[indexPath.Row] ?? (announcements[indexPath.Row] = Announcement.Get(course.Id, course.AnnouncementIds[indexPath.Row]));
        }

        public File GetFile(NSIndexPath indexPath)
        {
            return files[indexPath.Row] ?? (files[indexPath.Row] = File.Get(course.Id, course.FileIds[indexPath.Row]));
        }

        public Assignment GetAssignment(NSIndexPath indexPath)
        {
            var assignments = indexPath.Section == 0 ? undoneAssignments : doneAssignments;
            return assignments[indexPath.Row] ?? (assignments[indexPath.Row] = Assignment.Get(course.Id, course.AssignmentIds[indexPath.Row]));
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            switch (Segment)
            {
                case CourseMaterialsController.AnnouncementSegmentIndex:
                    {
                        var cell = tableView.DequeueReusableCell(announcementCellIdentifier) as CourseAnnouncementCell;
                        cell.Populate(GetAnnouncement(indexPath));
                        return cell;
                    }
                case CourseMaterialsController.FileSegmentIndex:
                    {
                        var cell = tableView.DequeueReusableCell(fileCellIdentifier) as CourseFileCell;
                        cell.Populate(GetFile(indexPath));
                        return cell;
                    }
                case CourseMaterialsController.AssignmentSegmentIndex:
                    {
                        var assignment = GetAssignment(indexPath);
                        if (!assignment.Done())
                        {
                            var cell = tableView.DequeueReusableCell(assignmentCellIdentifier) as CourseAssignmentCell;
                            cell.Populate(assignment);
                            return cell;
                        }
                        else
                        {
                            var cell = tableView.DequeueReusableCell(doneAssignmentCellIdentifier) as CourseDoneAssignmentCell;
                            cell.Populate(assignment);
                            return cell;
                        }
                    }
                default:
                    return null;
            }
        }
    }
}
