using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GrapeCity.Documents.Word;
using GrapeCity.Documents.Word.Layout;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Spire.Presentation;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels.Request;

namespace TozawaMauiHybrid.Helpers;

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
    public static readonly string powerpoint = "application/vnd.ms-powerpoint";
    public static readonly string powerpointXml = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
    public static readonly string word = "application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    public static readonly string textplain = "text/plain";
    public static readonly string _excel = "application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public static readonly string _xml = "application/xml";
    public static long MaxAllowedSize = 10 * 1024 * 1000;
    public static readonly string _validContentTypes = powerpoint + powerpointXml + _png + "," + _jpg + "," + pdf + "," + word + "," + textplain + "," + _excel;

    public static bool IsValidContentType(string contentType)
        => !string.IsNullOrEmpty(contentType) && (_validContentTypes.Split(",").Contains(contentType) || _imagesToConvertToPng.Split(",").Contains(contentType));

    public static bool IsImage(string conentType) => !string.IsNullOrEmpty(conentType) && conentType == _jpg || conentType == _png;
    public static bool IsPdf(string conentType) => !string.IsNullOrEmpty(conentType) && conentType == pdf;
    public static bool IsTextplain(string conentType) => !string.IsNullOrEmpty(conentType) && conentType == textplain;
    public static bool IsExcel(string conentType) => !string.IsNullOrEmpty(conentType) && word.Split(",").Contains(conentType);
    public static bool IsWord(string conentType) => !string.IsNullOrEmpty(conentType) && _excel.Split(",").Contains(conentType);
    public static bool IsXml(string conentType) => !string.IsNullOrEmpty(conentType) && _xml.Split(",").Contains(conentType);
    private static bool IsValidContent(byte[] bytes) => bytes != null && bytes.Length <= MaxAllowedSize;
    public static bool IsValidName(string fileName)
        => !string.IsNullOrEmpty(fileName) && IsValidFileName(fileName);

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
            IntPtr mimeTypePtr = new(mimeType);
            string mime = Marshal.PtrToStringUni(mimeTypePtr);
            Marshal.FreeCoTaskMem(mimeTypePtr);
            if (string.IsNullOrEmpty(mime))
                mime = "application/octet-stream";
            return mime;
        }
        catch (Exception ex)
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
    public static bool IsValidLength(string fileName) => fileName.Length <= FileNameLength;

    private static bool IsValidFileName(string name)
    {
        return Regex.IsMatch(name, _pattern);
    }
    public static async Task<byte[]> ImageToPng(byte[] buffer)
    {
        using var ms = new MemoryStream(buffer);
        return await Task.FromResult(GetPngBytes(ms));
    }
    private static byte[] GetPngBytes(MemoryStream ms)
    {
        using var saveStream = new MemoryStream();
        var image = System.Drawing.Image.FromStream(ms);
        image.Save(saveStream, System.Drawing.Imaging.ImageFormat.Png);
        return saveStream.ToArray();
    }
    public static async Task<byte[]> ConvertToPdf(byte[] buffer, string contentType)
    {
        byte[] result = [];
        if (_excel.Split(",").Contains(contentType)) result = ConvertExcelToPDF(buffer);
        if (powerpoint == contentType || powerpointXml == contentType) result = ConvertPowerPointToPDF(buffer, contentType);
        if (word.Split(",").Contains(contentType)) result = ConvertWordToPDF(buffer);
        if (textplain == contentType) result = ConvertTextPlainToPDF(buffer);

        return await Task.FromResult(result);
    }
    private static byte[] ConvertTextPlainToPDF(byte[] buffer)
    {
        byte[] returnBuffer = [];
        var extension = "txt";
        var assembly = AppDomain.CurrentDomain.BaseDirectory;
        string path = Path.Combine(assembly, @"tempfile") + "." + extension;
        File.WriteAllBytes(path, buffer);

        //first read text to end add to a string list.
        List<string> textFileLines = [];
        using (StreamReader sr = new(path))
        {
            while (!sr.EndOfStream)
            {
                textFileLines.Add(sr.ReadLine());
            }
        }

        Document doc = new();
        MigraDoc.DocumentObjectModel.Section section = doc.AddSection();

        //just font arrangements as you wish
        MigraDoc.DocumentObjectModel.Font font = new("Times New Roman", 15)
        {
            Bold = false
        };

        //add each line to pdf 
        foreach (string line in textFileLines)
        {
            MigraDoc.DocumentObjectModel.Paragraph paragraph = section.AddParagraph();
            paragraph.AddFormattedText(line, font);
        }

        //save pdf document
        PdfDocumentRenderer renderer = new()
        {
            Document = doc
        };
        renderer.RenderDocument();
        string extensionPdf = "pdf";
        string pathPdf = Path.Combine(assembly, @"temppdf") + "." + extensionPdf;
        renderer.Save(pathPdf);

        returnBuffer = File.ReadAllBytes(pathPdf);
        File.Delete(path);
        File.Delete(pathPdf);
        return returnBuffer;
    }
    private static byte[] ConvertPowerPointToPDF(byte[] buffer, string contentType)
    {
        byte[] returnBuffer = [];
        var extension = powerpoint == contentType ? "ppt" : "pptx";
        var assembly = AppDomain.CurrentDomain.BaseDirectory;
        string path = Path.Combine(assembly, @"tempfile") + "." + extension;
        File.WriteAllBytes(path, buffer);
        //Create a Presentation instance
        Presentation ppt = new();
        //Load a PowerPoint Presentation
        ppt.LoadFromFile(path);

        string extensionPdf = "pdf";
        string pathPdf = Path.Combine(assembly, @"temppdf") + "." + extensionPdf;
        //Save it to PDF
        ppt.SaveToFile(pathPdf, FileFormat.PDF);
        returnBuffer = File.ReadAllBytes(pathPdf);
        File.Delete(path);
        File.Delete(pathPdf);
        return returnBuffer;
    }
    private static byte[] ConvertWordToPDF(byte[] buffer)
    {
        byte[] returnBuffer = [];
        string extension = "docx";
        var assembly = AppDomain.CurrentDomain.BaseDirectory;
        string path = Path.Combine(assembly, @"tempfile") + "." + extension;
        File.WriteAllBytes(path, buffer);
        // Initalize Word instance
        var wordDoc = new GcWordDocument();
        // Load DOCX file 
        wordDoc.Load(path);
        // Create an instance of the GcWordLayout
        using var layout = new GcWordLayout(wordDoc);
        // Define the PDF output settings
        PdfOutputSettings pdfOutputSettings = new()
        {
            CompressionLevel = CompressionLevel.Fastest,
            ConformanceLevel = GrapeCity.Documents.Pdf.PdfAConformanceLevel.PdfA1a
        };
        // Save the Word layout as a PDF
        string extensionPdf = "pdf";
        string pathPdf = Path.Combine(assembly, @"temppdf") + "." + extensionPdf;
        layout.SaveAsPdf(pathPdf, null, pdfOutputSettings);
        returnBuffer = File.ReadAllBytes(pathPdf);
        File.Delete(path);
        File.Delete(pathPdf);
        return returnBuffer;
    }
    private static byte[] ConvertExcelToPDF(byte[] buffer)
    {
        var inputExcelData = StreamFromByteArray(buffer);
        MemoryStream pdfStream = new();
        //Instantiate the spreadsheet creation engine.
        using (ExcelEngine excelEngine = new())
        {
            //Instantiate the Excel application object.
            Syncfusion.XlsIO.IApplication application = excelEngine.Excel;

            //Set the default application version.
            application.DefaultVersion = ExcelVersion.Xlsx;

            //Load the existing Excel file into IWorkbook.
            IWorkbook workbook = application.Workbooks.Open(inputExcelData);

            //Settings for Excel to PDF conversion.
            XlsIORendererSettings settings = new()
            {
                //Set the layout option to fit all columns on one page.
                LayoutOptions = Syncfusion.XlsIORenderer.LayoutOptions.FitAllColumnsOnOnePage
            };

            //Initialize the XlsIORenderer.
            XlsIORenderer renderer = new();

            //Initialize the PDF document.
            PdfDocument pdfDocument = new();

            //Convert the Excel document to PDF.
            pdfDocument = renderer.ConvertToPDF(workbook, settings);

            //Save the PDF file.
            pdfDocument.Save(pdfStream);

            //Close the PDF document.
            pdfDocument.Close();

            //Close the workbook.
            workbook.Close();
        }
        return pdfStream.ToArray();
    }
    public static MemoryStream StreamFromByteArray(byte[] content)
    {
        MemoryStream stream = new(content);

        return stream;
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