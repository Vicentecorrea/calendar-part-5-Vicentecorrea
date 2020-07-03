using CalendarApp.Controllers;
using CalendarApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarApp.Views
{
    public partial class LoginForm : Form
    {
        #region Fields
        private readonly UserController userController = new UserController();
        #endregion

        #region Methods
        public LoginForm()
        {
            InitializeComponent();
            userController.LoadUsers();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            bool isValidUserName = !String.IsNullOrWhiteSpace(userNameTextBox.Text);
            if (isValidUserName)
            {
                string loginUserName = userNameTextBox.Text;
                bool userAlreadyExists = userController.CheckIfUserNameExists(loginUserName);
                if (userAlreadyExists)
                {
                    Login(loginUserName);
                }
                else
                {
                    MessageBox.Show("There is no user with that username");
                }
            }
            else
            {
                MessageBox.Show("You should enter a username");
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            bool isValidUserName = !String.IsNullOrWhiteSpace(userNameTextBox.Text);
            if (isValidUserName)
            {
                string loginUserName = userNameTextBox.Text;
                bool userAlreadyExists = userController.CheckIfUserNameExists(loginUserName);
                if (!userAlreadyExists)
                {
                    Register(loginUserName);
                    Login(loginUserName);
                }
                else
                {
                    MessageBox.Show("A user already exists with that username, choose another username");
                }
            }
            else
            {
                MessageBox.Show("You should enter a username");
            }
        }

        private void Login(string loginUserName)
        {
            UserController.LoggedUserName = loginUserName;
            this.Hide();
            CalendarForm calendarForm = new CalendarForm(userController);
            calendarForm.ShowDialog();
            this.Close();
        }

        private void Register(string loginUserName)
        {
            User newUser = new User(loginUserName);
            userController.SaveUser(newUser);
            MessageBox.Show("Successfully created user");
        }
        #endregion
    }
}
