using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace WinFormMVC.Model
{
    // Modes for when a user is selected in the View
    public enum ViewSelectionMode
    {
        Read,
        Edit,
        Create
    }

    public class User
    {
        // Public Variables
        public enum SexOfPerson
        {
            Male = 1,
            Female = 2,
            Other = 3
        }

        public String FirstName
        {
            get { return _FirstName; }
            set
            {
                if (value.Length > 50)
                {
                    ts.TraceEvent(TraceEventType.Error, (int)TraceEventId.UsersModel, "FirstName must be less than 51 characters");
                }
                else
                {
                    _FirstName = value;
                }
            }
        }

        public string LastName
        {
            get { return _LastName; }
            set
            {
                if (value.Length > 50)
                {
                    ts.TraceEvent(TraceEventType.Error, (int)TraceEventId.UsersModel, "LastName must be less than 51 characters");
                }
                else
                {
                    _LastName = value;
                }
            }
        }

        public string Id
        {
            get { return _Id; }
            private set { _Id = value; }
        }

        public string Department
        {
            get { return _Department; }
            set { _Department = value; }
        }

        public SexOfPerson Sex
        {
            get { return _Sex; }
            set { _Sex = value; }
        }

        // Private Variables
        private static readonly TraceSource ts = new TraceSource("WinFormMVC");

        private static int nextId = 0;
        private string _FirstName;
        private string _LastName;
        private string _Id;
        private string _Department;
        private SexOfPerson _Sex;

        // Public Methods
        public User(string firstName, string lastName, string department, SexOfPerson sex)
        {
            FirstName = firstName;
            LastName = lastName;
            Id = Interlocked.Increment(ref nextId).ToString();
            Department = department;
            Sex = sex;
        }

        // Helper Methods
        public ListViewItem ConvertToListViewItem()
        {
            ListViewItem item = new ListViewItem(this.Id);
            item.SubItems.Add(this.FirstName);
            item.SubItems.Add(this.LastName);
            item.SubItems.Add(this.Department);
            item.SubItems.Add(Enum.GetName(typeof(User.SexOfPerson), this.Sex));

            return item;
        }
    }
}