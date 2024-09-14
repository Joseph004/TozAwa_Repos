using System.Text.RegularExpressions;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels.Request;

namespace ShareRazorClassLibrary.Helpers;

public static class FileValidator
{
    private static readonly string _pattern = @"^[a-zA-zŽžÀ-ÿZ0-9-_ ]*[.]{0,1}[a-zA-zŽžÀ-ÿZ0-9-_ ]*$";
    private static readonly int FileNameLength = 255;
    private static readonly string _png = "image/png";
    private static readonly string _jpg = "image/jpeg";
    public static readonly string pdf = "application/pdf";
    public static readonly string word = "application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    public static readonly string textplain = "text/plain";
    public static readonly string _excel = "application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public static readonly string _xml = "application/xml";
    public static long MaxAllowedSize = 10 * 1024 * 1000;
    public static readonly string _validContentTypes = _png + "," + _jpg + "," + pdf + "," + word + "," + textplain + "," + _excel;

    public static bool IsValidContentType(string contentType)
        => !string.IsNullOrEmpty(contentType) && _validContentTypes.Split(",").Contains(contentType);

    public static bool IsExcel(string conentType) => !string.IsNullOrEmpty(conentType) && _excel.Split(",").Contains(conentType);
    public static bool IsXml(string conentType) => !string.IsNullOrEmpty(conentType) && _xml.Split(",").Contains(conentType);
    private static bool IsValidContent(byte[] bytes) => bytes != null && bytes.Length <= MaxAllowedSize;
    public static bool IsValidName(string fileName)
        => !string.IsNullOrEmpty(fileName) && IsValidFileName(fileName);

    public static bool IsValidLength(string fileName) => fileName.Length <= FileNameLength;

    private static bool IsValidFileName(string name)
    {
        return Regex.IsMatch(name, _pattern);
    }

    private static bool IsValidSize(double? size) => size != null && size <= MaxAllowedSize;

    public static bool IsValideFile(AddAttachmentRequest file)
    {
        if (!IsValidContentType(file.MimeType))
        {
            return false;
        }
        else if (!IsValidName(file.Name))
        {
            return false;
        }
        else if (!IsValidLength(file.Name))
        {
            return false;
        }
        else if (!IsValidSize(file.Size))
        {
            return false;
        }
        return true;
    }
    public static bool IsValideFile(AttachmentUploadDto file)
    {
        if (!IsValidContentType(file.ContentType))
        {
            return false;
        }
        else if (!IsValidName(file.Name))
        {
            return false;
        }
        else if (!IsValidContent(file.Content))
        {
            return false;
        }
        return true;
    }
}