using System.Configuration;

namespace DundeeComicBookStore
{
    public static class ConnectionHelper
    {
        public static string ConnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}