using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Configurations
{
    public class AppSettings
    {
        public TozAwaNGOApiSettings TozAwaNGOApiSettings { get; set; }
        public JWTSettings JWTSettings { get; set; }
        public ActiveLanguageDto[] Languages { get; set; }
    }
}