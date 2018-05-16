using SQLite;

namespace KleynGroup.Data
{
    public interface ISqLite
    {
        SQLiteConnection GetConnection();
    }
}
