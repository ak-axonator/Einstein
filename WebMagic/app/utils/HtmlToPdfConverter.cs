using System;
using System.IO;
// using iText.pdfHTML;
// using iText.Kernel.Pdf;

namespace WebMagic
{
    public static class HtmlToPdfConverter
{
    public static string Convert(string htmlFilePath)
    {
        // // Create output folder if not present
        // string outputFolder = Path.Combine(GlobalPaths.AssetsFolder, "PDF Forms");
        // Directory.CreateDirectory(outputFolder);

        // // Set output PDF file path
        // string pdfFilePath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(htmlFilePath) + ".pdf");

        // // Create PDF document object and open it for writing
        // PdfDocument pdfDoc = new PdfDocument(new PdfWriter(pdfFilePath));
        // pdfDoc.InitializeOutlines();

        // // Read input HTML file and convert it to PDF
        // // ConverterProperties converterProperties = new ConverterProperties();
        // // HtmlConverter.ConvertToPdf(File.Open(htmlFilePath, FileMode.Open), pdfDoc, converterProperties);

        // // Close the document
        // pdfDoc.Close();

        // return pdfFilePath;
        return htmlFilePath;
    }
}
}