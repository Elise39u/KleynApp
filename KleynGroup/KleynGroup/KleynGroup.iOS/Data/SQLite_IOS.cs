using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using KleynGroup.Data;
using KleynGroup.iOS.Data;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_IOS))]

namespace KleynGroup.iOS.Data
{
    public class SQLite_IOS : ISQLite
    {
        public SQLite_IOS() { }

        public SQLite.SQLiteConnection GetConnection()
        {
            var fileName = "Login.db3";
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentPath, "..", "library");
            var path = Path.Combine(libraryPath, fileName);
            var connection = new SQLite.SQLiteConnection(path);
            return connection;
        }
    }
}