using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Forms;

using WinFormMVC.Model;

namespace WinFormMVC.Controller
{
    public class UsersController
    {
        // Public Variables
        public UsersController(IUsersView view, Dictionary<string, User> users)
        {
            _view = view;
            _users = users;
            view.SetController(this);
        }

        public ReadOnlyDictionary<string, User> Users
        {
            get { return new ReadOnlyDictionary<string, User>(_users); }
        }

        public Action<FormClosingEventArgs> OnFormClosingCallback
        {
            get { return _onFormClosingCallback; }
            set { _onFormClosingCallback = value; }
        }

        // Private Variables
        private static readonly TraceSource ts = new TraceSource("WinFormMVC");

        private IUsersView _view;
        private Dictionary<string, User> _users;
        private User _selectedUser;
        private Action<FormClosingEventArgs> _onFormClosingCallback = (e) =>
            ts.TraceEvent(TraceEventType.Warning, (int)TraceEventId.UsersController, "Unhandled form closing");

        // Public Methods
        public void LoadView()
        {
            // Populate the grid with users
            foreach (KeyValuePair<string, User> usr in _users)
            {
                _view.AddUserToGrid(usr.Value);
            }
        }

        public void SelectedUserChanged(string selectedUserId)
        {
            bool doesUserExist = _users.TryGetValue(selectedUserId, out User usr);

            if (doesUserExist)
            {
                _selectedUser = usr;
                UpdateViewDetailValues(usr);
                _view.SetSelectedUserInGrid(usr);
                _view.Mode = ViewSelectionMode.Read;
            }
            else
            {
                ts.TraceEvent(TraceEventType.Error, (int)TraceEventId.UsersController, $"User {selectedUserId} doesn't exist");
            }
        }

        public void AddNewUser()
        {
            if (_view.Mode == ViewSelectionMode.Create)
            {
                Save();
            }

            _selectedUser = new User(
                "", // FirstName
                "", // Lastname
                "", // Department
                User.SexOfPerson.Other // Sex
            );
            UpdateViewDetailValues(_selectedUser);
            _view.Mode = ViewSelectionMode.Create;

            ts.TraceEvent(TraceEventType.Verbose, (int)TraceEventId.UsersController, $"Created new User: '{_selectedUser.Id}'");
        }

        public void EditUser()
        {
            string id = _view.GetIdOfSelectedUserInGrid();
            if (!string.IsNullOrEmpty(id))
            {
                _view.Mode = ViewSelectionMode.Edit;
            }
            else
            {
                ts.TraceEvent(TraceEventType.Warning, (int)TraceEventId.UsersController, "Attempted to edit null User");
            }
        }

        public void RemoveUser()
        {
            if (_view.Mode == ViewSelectionMode.Create)
            {
                _selectedUser = null;
                UpdateViewDetailValues(null);
                _view.Mode = ViewSelectionMode.Read;

                ts.TraceEvent(TraceEventType.Verbose, (int)TraceEventId.UsersController, "Deleted User being created");
                return;
            }

            string id = _view.GetIdOfSelectedUserInGrid();
            bool doesUserExist = _users.TryGetValue(id, out User userToRemove);

            if (doesUserExist)
            {
                _users.Remove(userToRemove.Id);
                _view.RemoveUserFromGrid(userToRemove);

                ts.TraceEvent(TraceEventType.Verbose, (int)TraceEventId.UsersController, $"Removed User: '{id}'");
            }
            else
            {
                ts.TraceEvent(TraceEventType.Warning, (int)TraceEventId.UsersController, "Attempted to remove null User");
            }
        }

        public void Save()
        {
            if (_selectedUser == null)
            {
                ts.TraceEvent(TraceEventType.Warning, (int)TraceEventId.UsersController, "Attempted to save null User");
                return;
            }

            UpdateUserWithViewValues(_selectedUser);

            if (!_users.ContainsValue(_selectedUser))
            {
                // Add new user
                _users.Add(_selectedUser.Id, _selectedUser);
                _view.AddUserToGrid(_selectedUser);

                ts.TraceEvent(TraceEventType.Verbose, (int)TraceEventId.UsersController, $"Saved new User: '{_selectedUser.Id}'");
            }
            else
            {
                // Update existing user
                _view.UpdateGridWithChangedUser(_selectedUser);

                ts.TraceEvent(TraceEventType.Verbose, (int)TraceEventId.UsersController, $"Edited existing User: '{_selectedUser.Id}'");
            }

            _view.SetSelectedUserInGrid(_selectedUser);
            _view.Mode = ViewSelectionMode.Read;
        }

        public void OnFormClosing(FormClosingEventArgs e)
        {
            _onFormClosingCallback(e);
        }

        // Private Methods
        private void UpdateViewDetailValues(User usr)
        {
            // Assign empty strings if usr is null
            _view.FirstName = usr?.FirstName ?? "";
            _view.LastName = usr?.LastName ?? "";
            _view.Id = usr?.Id ?? "";
            _view.Department = usr?.Department ?? "";
            _view.Sex = usr?.Sex ?? User.SexOfPerson.Other;
        }

        private void UpdateUserWithViewValues(User usr)
        {
            if (usr != null)
            {
                usr.FirstName = _view.FirstName;
                usr.LastName = _view.LastName;
                usr.Department = _view.Department;
                usr.Sex = _view.Sex;
            }
        }
    }
}


