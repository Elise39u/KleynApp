using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Forms;
using KleynGroup.Models;

namespace KleynGroup.Data
{
    public class UserDatabaseController
    {
        SQLiteConnection _conn;

        //Create
        public void UserDatabase()
        {
            _conn = DependencyService.Get<ISqLite>().GetConnection();
            _conn.CreateTable<UserData>();
        }

        public void Droptable()
        {
            _conn.Execute("delete from UserData");
        }

        public IEnumerable<UserData> GetUsers()
        {
            var users = (from mem in _conn.Table<UserData>() select mem);
            return users;
        }


        //INSERT
        public string AddUser(Token token)
        {
            UserDatabase();
            _conn.Insert(token); // <-- Crash line System.NullReferenceException: Object reference not set to an instance of an object.
            return "success";
        }
        /*
        //INSERT
        public string AddUser(Token token)
        {
            var NumOfRows = _conn.Insert(token); // <-- Crash line System.NullReferenceException: Object reference not set to an instance of an object.
            if (NumOfRows > 0)
                return "Succes Users added";
            else
                return "Something went wrong contact a admin";
        }
        */

        public string DeleteUser(int id)
        {
            _conn.Delete<UserData>(id);
            return "success";
        }

        public List<UserData> GetAllUsers()
        {
            UserDatabase();
            return _conn.Query<UserData>("SELECT * FROM UserData");
        }
    }
}
