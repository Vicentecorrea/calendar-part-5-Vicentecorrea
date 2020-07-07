using CalendarApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp.Controllers
{
    public class UserController
    {
        #region Fields
        static private List<User> users = new List<User>();
        static private string loggedUserName;
        #endregion

        #region Properties
        /// <summary>Public property for accessing the users.</summary>
        public List<User> Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
            }
        }

        /// <summary>Public property for accessing the logged in user.</summary>
        static public string LoggedUserName
        {
            get
            {
                return loggedUserName;
            }
            set
            {
                loggedUserName = value;
            }
        }
        #endregion

        #region Methods
        public void SaveUser(User user)
        {
            Users.Add(user);
            SerializeUsers();
        }

        public void LoadUsers()
        {
            Stream stream = null;
            try
            {
                stream = File.Open(Constants.PathToUsersSerializationFile, FileMode.OpenOrCreate);
                if (stream.Length > Constants.ZeroItemsInList)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    object usersData = binaryFormatter.Deserialize(stream);
                    Users = usersData as List<User>;
                    if (Users == null)
                    {
                        Users = new List<User>();
                    }
                }
            }
            finally
            {
                stream.Close();
            }
        }

        public bool CheckIfUserNameExists(string loginUserName)
        {
            if (loginUserName == null)
            {
                throw new ArgumentNullException("loginUserName");
            }
            foreach (User user in Users)
            {
                if (loginUserName.Equals(user.UserName))
                {
                    return true;
                }
            }
            return false;
        }

        private void SerializeUsers()
        {
            Stream stream = null;
            try
            {
                stream = File.Open(Constants.PathToUsersSerializationFile, FileMode.OpenOrCreate);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, Users);
            }
            finally
            {
                stream.Close();
            }
        }
        #endregion
    }
}
