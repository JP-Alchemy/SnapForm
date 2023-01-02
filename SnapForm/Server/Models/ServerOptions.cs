namespace SnapForm.Server
{
    public class ServerOptions : IServerOptions
    {
        public string MongoConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Token { get; set; }
    }

    public interface IServerOptions
    {
        string MongoConnectionString { get; set; }
        string DatabaseName { get; set; }
        string Token { get; set; }
    }

    public static class CollectionNames
    {
        public static string Forms = "Forms";
        public static string Fields = "Fields";
        public static string UserFields = "UserFields";
        public static string FormSubmissions = "FormSubmissions";
        public static string AppUser = "AppUser";
        public static string EnterpriseUser = "EnterpriseUser";
        public static string Enterprise = "Enterprise";
        public static string UserPresets = "UserPresets";
        public static string Occupation = "Occupation";
    }
}