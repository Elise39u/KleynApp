using System.IO;
using KleynGroup.Data;
using KleynGroup.Droid.Data;
using Xamarin.Forms;

[assembly: Dependency(typeof(SqLiteAndroid))]
namespace KleynGroup.Droid.Data
{
    public class SqLiteAndroid :ISqLite

    {
        public SQLite.SQLiteConnection GetConnection()
        {
            var sqliteFileName = "UserData.sqlite";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFileName);
            var conn = new SQLite.SQLiteConnection(path);

            return conn;
        }
    }
}