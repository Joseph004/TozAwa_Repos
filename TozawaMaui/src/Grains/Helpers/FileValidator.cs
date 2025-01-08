using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Grains.Attachment.Models.Requests;
using Grains.Models.Dtos;

namespace Grains.Helpers;

public static class FileValidator
{
    private static readonly string _pattern = @"^[a-zA-zŽžÀ-ÿZ0-9-_ ]*[.]{0,1}[a-zA-zŽžÀ-ÿZ0-9-_ ]*$";
    public static readonly string _imagesToConvertToPng = "image/tiff,image/x-tiff,image/bmp,image/x-windows-bmp";
    private static readonly Dictionary<int, ImageType> _imageTag = Enum.GetValues(typeof(ImageType))
               .Cast<ImageType>()
               .ToDictionary(t => (int)t);
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

    public static bool IsImage(string conentType) => !string.IsNullOrEmpty(conentType) && conentType == _jpg || conentType == _png;
    public static bool IsPdf(string conentType) => !string.IsNullOrEmpty(conentType) && conentType == pdf;
    public static bool IsTextplain(string conentType) => !string.IsNullOrEmpty(conentType) && conentType == textplain;
    public static bool IsExcel(string conentType) => !string.IsNullOrEmpty(conentType) && _excel.Split(",").Contains(conentType);
    public static bool IsXml(string conentType) => !string.IsNullOrEmpty(conentType) && _xml.Split(",").Contains(conentType);
    private static bool IsValidContent(byte[] bytes) => bytes != null && bytes.Length <= MaxAllowedSize;
    public static bool IsValidName(string fileName)
        => !string.IsNullOrEmpty(fileName) && IsValidFileName(fileName);

    public static bool IsValidLength(string fileName) => fileName.Length <= FileNameLength;
    public static bool IsValidePdf(byte[] bytes)
    {
        if (!IsValiBytes(bytes)) return false;

        if (bytes?.Length < 4) return false;
        var stopBefore = Math.Min(bytes.Length, 1024) - 3;
        for (var i = 0; i < stopBefore; i++)
            if (bytes[i] == '%'
                && bytes[i + 1] == 'P'
                && bytes[i + 2] == 'D'
                && bytes[i + 3] == 'F') return true;
        return false;
    }
    public static bool IsValiBytes(byte[] bytes)
    {
        return bytes != null && bytes.Length > 0;
    }
    public static bool IsValideImage(byte[] bytes)
    {
        if (!IsValiBytes(bytes)) return false;

        int key = (bytes[1] << 8) + bytes[0];
        if (_imageTag.TryGetValue(key, out ImageType s))
        {
            if (s == ImageType.None) return false;

            return true;
        }
        return false;
    }
    public static bool IsValideWord(byte[] bytes)
    {
        if (!IsValiBytes(bytes)) return false;
        if (ScanFileForMimeType(bytes) == "application/x-zip-compressed" || ScanFileForMimeType(bytes) == "application/octet-stream")
            return true;
        else
            return false;
    }
    private static string ScanFileForMimeType(byte[] bytes)
    {
        try
        {
            UInt32 mimeType = default(UInt32);
            FindMimeFromData(0, null, bytes, 256, null, 0, ref mimeType, 0);
            IntPtr mimeTypePtr = new IntPtr(mimeType);
            string mime = Marshal.PtrToStringUni(mimeTypePtr);
            Marshal.FreeCoTaskMem(mimeTypePtr);
            if (string.IsNullOrEmpty(mime))
                mime = "application/octet-stream";
            return mime;
        }
        catch (Exception)
        {
            return "application/octet-stream";
        }
    }

    [DllImport("urlmon.dll", CharSet = CharSet.Auto)]
    private static extern UInt32 FindMimeFromData(
       UInt32 pBC, [MarshalAs(UnmanagedType.LPStr)]
       string pwzUrl, [MarshalAs(UnmanagedType.LPArray)]
       byte[] pBuffer, UInt32 cbSize, [MarshalAs(UnmanagedType.LPStr)]
       string pwzMimeProposed, UInt32 dwMimeFlags, ref UInt32 ppwzMimeOut, UInt32 dwReserverd
    );
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
public enum ImageType
{
    None = 0,
    BMP = 0x4D42,
    JPG = 0xD8FF,
    GIF = 0x4947,
    PCX = 0x050A,
    PNG = 0x5089,
    PSD = 0x4238,
    RAS = 0xA659,
    SGI = 0xDA01,
    TIFF = 0x4949
}