using TozawaMauiHybrid.Models.Dtos;

namespace TozawaMauiHybrid.Configurations
{
    public class AppSettings
    {
        public TozAwaNGOApiSettings TozAwaNGOApiSettings { get; set; }
        public JWTSettings JWTSettings { get; set; }
        public ActiveLanguageDto[] Languages { get; set; }
    }
}