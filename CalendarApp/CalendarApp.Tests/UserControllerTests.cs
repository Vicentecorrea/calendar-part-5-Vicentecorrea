using CalendarApp.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using CalendarApp.Models;

namespace Tests
{
    public class UserControllerTests
    {
        private UserController userController = null;

        [SetUp]
        public void Setup()
        {
            userController = new UserController();
        }

        [TearDown]
        public void TearDown()
        {
            userController = null;
        }

        [Test]
        [Category("Simple Tests")]
        public void CheckIfUserNameExists_UserName_ReturnsTrue()
        {
            User firstDefaultUser = new User("Pedro");
            User secondDefaultUser = new User("Juan");
            User thirdDefaultUser = new User("Diego");
            userController.Users = new List<User> {firstDefaultUser, secondDefaultUser, thirdDefaultUser};
            string userNameInput = "Juan";

            bool result = userController.CheckIfUserNameExists(userNameInput);
            Assert.IsTrue(result, string.Format("There is no user with username {0}", userNameInput));
        }

        [Test]
        [Category("Exception Tests")]
        public void CheckIfUserNameExists_NullUserName_ThrowsArgumentNullException()
        {
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => userController.CheckIfUserNameExists(null));
            Assert.That(argumentNullException.ParamName, Is.EqualTo("loginUserName"));
        }
    }
}
