using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;

using WinFormMVC.Model;
using WinFormMVC.Controller;
using WinFormMVC.View;


// Contains an id for each module's error traces
public enum TraceEventId
{
    Application = 0,

    Users = 100,
    UsersModel = 101,
    UsersController = 102,
    IUsersView = 103,
    UsersView = 104,
    UsersHelper = 105
}

namespace WinFormMVC
{
    internal static class Application
    {
        private static readonly TraceSource ts = new TraceSource("WinFormMVC");

        static void Main()
        {
            ts.TraceEvent(TraceEventType.Information, (int)TraceEventId.Application, "Application started");

            // Create Users View
            UsersView view = new UsersView();
            view.Visible = false;

            // Create a list of users then convert it into a dictionary
            IList<User> userList = new List<User>();
            userList.Add(new User("Vladimir", "Putin", "Goverment of Russia", User.SexOfPerson.Male));
            userList.Add(new User("Barack", "Obama", "Goverment of USA", User.SexOfPerson.Male));
            userList.Add(new User("Stephen", "Harper", "Goverment of Canada", User.SexOfPerson.Male));
            userList.Add(new User("Jean", "Charest", "Goverment of Quebec", User.SexOfPerson.Male));
            userList.Add(new User("David", "Cameron", "Goverment of United Kingdom", User.SexOfPerson.Male));
            userList.Add(new User("Angela", "Merkel", "Goverment of Germany", User.SexOfPerson.Female));
            userList.Add(new User("Nikolas", "Sarkozy", "Goverment of France", User.SexOfPerson.Male));
            userList.Add(new User("Silvio", "Berlusconi", "Goverment of Italy", User.SexOfPerson.Male));
            userList.Add(new User("Yoshihiko", "Noda", "Goverment of Japan", User.SexOfPerson.Male));

            Dictionary<string, User> userDictionary = new Dictionary<string, User>();
            foreach (User usr in userList)
            {
                userDictionary.Add(usr.Id, usr);
            }

            // Create the Controller and pass it the View
            UsersController controller = new UsersController(view, userDictionary);
            controller.LoadView();
            controller.OnFormClosingCallback = (FormClosingEventArgs e) =>
                ts.TraceEvent(TraceEventType.Information, (int)TraceEventId.Application, "Application stopped");

            view.ShowDialog();
        }
    }
}
