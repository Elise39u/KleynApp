﻿using System.Collections.Generic;
using System.Linq;
using SQLite;
using Xamarin.Forms;
using KleynGroup.Models;

namespace KleynGroup.Data
{
   public class UserDatabase
    {
        private readonly SQLiteConnection _conn;

        //Create
        public UserDatabase()
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
            return users.ToList();
        }
        //INSERT
        public string AddUser(Token token)
        {
            _conn.Insert(token);
            return "success";
        }

        //DELETE
        public string DeleteUser(int id)
        {
            _conn.Delete<UserData>(id);
            return "success";
        }

        public List<UserData> GetAllUsers() { 
            return _conn.Query<UserData>("SELECT * FROM UserData");
        }
    }
}
