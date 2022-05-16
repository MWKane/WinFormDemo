using System;
using System.Diagnostics;
using System.Windows.Forms;

using WinFormMVC.Model;
using WinFormMVC.Controller;
using WinFormMVC.Helper;

namespace WinFormMVC.View
{
    public partial class UsersView : Form, IUsersView
    {
        // Init Logging
        private static readonly TraceSource ts = new TraceSource("WinFormMVC");

        // Private Variables
        private UsersController _controller;
        private ViewSelectionMode _mode;

        // Public Variables
        public string Id
        {
            get { return txtId.Text; }
            set { txtId.Text = value; }
        }

        public string FirstName
        {
            get { return txtFirstName.Text; }
            set { txtFirstName.Text = value; }
        }

        public string LastName
        {
            get { return txtLastName.Text; }
            set { txtLastName.Text = value; }
        }

        public string Department
        {
            get { return txtDepartment.Text; }
            set { txtDepartment.Text = value; }
        }

        public User.SexOfPerson Sex
        {
            get
            {
                if (rdMale.Checked)
                {
                    return User.SexOfPerson.Male;
                }
                else if (rdFemale.Checked)
                {
                    return User.SexOfPerson.Female;
                }
                else
                {
                    return User.SexOfPerson.Other;
                }
            }

            set
            {
                if (value == User.SexOfPerson.Male)
                {
                    rdMale.Checked = true;
                }
                else if (value == User.SexOfPerson.Female)
                {
                    rdFemale.Checked = true;
                }
                else
                {
                    rdOther.Checked = true;
                }
            }
        }

        public ViewSelectionMode Mode
        {
            get { return _mode; }
            set
            {
                switch (value)
                {
                    case ViewSelectionMode.Read:
                        txtFirstName.ReadOnly = true;
                        txtLastName.ReadOnly = true;
                        txtDepartment.ReadOnly = true;
                        grbSex.Enabled = false;
                        break;

                    case ViewSelectionMode.Edit:
                        txtFirstName.ReadOnly = false;
                        txtLastName.ReadOnly = false;
                        txtDepartment.ReadOnly = false;
                        grbSex.Enabled = true;
                        break;

                    case ViewSelectionMode.Create:
                        txtFirstName.ReadOnly = false;
                        txtLastName.ReadOnly = false;
                        txtDepartment.ReadOnly = false;
                        grbSex.Enabled = true;
                        break;

                    default:
                        break;
                }

                _mode = value;
            }
        }

        // Public Methods
        public UsersView()
        {
            InitializeComponent();
            Mode = ViewSelectionMode.Read;

            ts.TraceEvent(TraceEventType.Information, (int)TraceEventId.UsersView, "Form created");
        }

        public void SetController(UsersController controller)
        {
            _controller = controller;
        }

        public void AddUserToGrid(User usr)
        {
            ListViewItem rowToAdd = usr.ConvertToListViewItem();
            grdUsers.Items.Add(rowToAdd);
        }

        public void UpdateGridWithChangedUser(User usr)
        {
            ListViewItem rowToUpdate = grdUsers.FindItemWithText(usr.Id, false, 0, true);
            if (rowToUpdate != null)
            {
                rowToUpdate.SubItems[1].Text = usr.FirstName;
                rowToUpdate.SubItems[2].Text = usr.LastName;
                rowToUpdate.SubItems[3].Text = usr.Department;
                rowToUpdate.SubItems[4].Text = Enum.GetName(typeof(User.SexOfPerson), usr.Sex);
            }
            else
            {
                ts.TraceEvent(TraceEventType.Warning, (int)TraceEventId.UsersView, $"Tried to update non-existent grdUser: '{usr.Id}'");
            }
        }

        public void RemoveUserFromGrid(User usr)
        {
            ListViewItem rowToRemove = grdUsers.FindItemWithText(usr.Id, false, 0, true);
            if (rowToRemove != null)
            {
                grdUsers.Items.Remove(rowToRemove);
                grdUsers.Focus();
            }
            else
            {
                ts.TraceEvent(TraceEventType.Warning, (int)TraceEventId.UsersView, $"Tried to remove non-existent grdUser: '{usr.Id}'");
            }
        }

        public string GetIdOfSelectedUserInGrid()
        {
            if (grdUsers.SelectedItems.Count > 0)
            {
                return grdUsers.SelectedItems[0].Text;
            }
            else
            {
                return "";
            }
        }

        public void SetSelectedUserInGrid(User usr)
        {
            ListViewItem rowToSelect = grdUsers.FindItemWithText(usr.Id, false, 0, true);
            if (rowToSelect != null)
            {
                rowToSelect.Selected = true;
            }
            else
            {
                ts.TraceEvent(TraceEventType.Warning, (int)TraceEventId.UsersView, $"Attempted to select non-existent grdUser: '{usr.Id}'");
            }
        }

        // Private Methods - ControllerUser Events
        private void btnAdd_Click(object sender, EventArgs e)
        {
            _controller.AddNewUser();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            _controller.EditUser();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            _controller.RemoveUser();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _controller.Save();
        }

        private void grdUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (grdUsers.SelectedItems.Count > 0)
            {
                _controller.SelectedUserChanged(grdUsers.SelectedItems[0].Text);
            }
        }

        private void grdUsers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            UserListViewSorter sorter = (UserListViewSorter)grdUsers.ListViewItemSorter;
            sorter.Column = e.Column;
            grdUsers.Sort();
        }

        private void rdMale_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdFemale_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdOther_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Overrides
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ts.TraceEvent(TraceEventType.Information, (int)TraceEventId.UsersView, "Form closing");
            _controller.OnFormClosing(e);
        }
    }
}
