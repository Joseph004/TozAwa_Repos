using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Configurations
{
    public class AppSettings
    {
        public TozAwaNGOApiSettings TozAwaNGOApiSettings { get; set; }
        public string PrivateKey { get; set; }
        public GoogleDrive GoogleDrive { get; set; }
        public JWTSettings JWTSettings { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public ActiveLanguageDto[] Languages { get; set; }
    }
}