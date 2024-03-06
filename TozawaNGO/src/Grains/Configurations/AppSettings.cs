
namespace Grains.Configurations
{
    public class AppSettings
    {
        public string PrivateKey { get; set; }
        public GoogleDrive GoogleDrive { get; set; }
        public JWTSettings JWTSettings { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }
}