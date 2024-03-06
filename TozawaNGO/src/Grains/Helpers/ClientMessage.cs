#nullable disable

namespace Grains.Helpers;
public static class UpdateMessages
{
    public static void Configure()
    {
    }
    public static string EntityDeletedSuccess => "Entity deleted successfully";
    public static string Success => "Success";
    public static string NotFound => "Not found";
    public static string Forbidden => "Forbidden";
    public static string EntityCreatedSuccess => "Entity created successfullt";
    public static string EntityCreatedError => "Error when create entity";
    public static string Error => "Error";
    public static string Unauthorized => "Unauthorized";
    public static string ImportSuccess => "Import success";
    public static string ImportError => "Error when importing";
}