namespace Boilerplate.Settings
{
    public class Appsettings
    {
        public string ASPNETCORE_ENVIRONMENT { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string DbConnection { get; set; }
    }
}