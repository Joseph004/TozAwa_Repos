using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Configurations
{
    public class AppSettings
    {
        public TozAwaNGOApiSettings TozAwaNGOApiSettings { get; set; }
          public JWTSettings JWTSettings { get; set; }
        public ActiveLanguageDto[] Languages { get; set; }
    }
}