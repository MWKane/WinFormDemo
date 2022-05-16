using WinFormMVC.Model;

namespace WinFormMVC.Controller
{
    public interface IUsersView
    {
        void SetController(UsersController controller);
        void AddUserToGrid(User user);
        void UpdateGridWithChangedUser(User user);
        void RemoveUserFromGrid(User user);
        string GetIdOfSelectedUserInGrid();
        void SetSelectedUserInGrid(User user);

        string FirstName { get; set; }
        string LastName { get; set; }
        string Id { get; set; }
        string Department { get; set; }
        User.SexOfPerson Sex { get; set; }
        ViewSelectionMode Mode { get; set; }
    }
}