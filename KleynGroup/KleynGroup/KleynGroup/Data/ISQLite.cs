using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace KleynGroup.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
