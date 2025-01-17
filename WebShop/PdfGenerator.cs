using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebShop.Models;

namespace MyPdfLibrary
{
    public class PdfGenerator
    {
        public static async Task CreateInvoice(string filePath, string userName, DateTime date, List<Product> products)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || userName == null || products == null)
                {
                    throw new ArgumentNullException();
                }

                using (var fs = new FileStream(filePath, FileMode.Create))
                using (var writer = new StreamWriter(fs, Encoding.ASCII))
                {
                    // Company details
                    string companyName = "Your Console Game Shop";
                    string companyWebsite = "www.consolegameshop.com";
                    string companyEmail = "contact@consolegameshop.com";
                    string companyPhone = " +46 73 638 3393";

                    // Calculate total
                    decimal total = 0;
                    foreach (var product in products)
                    {
                        total += product.Price;
                    }

                    // PDF Header
                    writer.WriteLine("%PDF-1.4");
                    writer.WriteLine("%\xE2\xE3\xCF\xD3"); // Binary signature for PDF files

                    // Object 1: Catalog
                    writer.WriteLine("1 0 obj");
                    writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
                    writer.WriteLine("endobj");

                    // Object 2: Pages
                    writer.WriteLine("2 0 obj");
                    writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
                    writer.WriteLine("endobj");

                    // Object 3: Page
                    writer.WriteLine("3 0 obj");
                    writer.WriteLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R >>");
                    writer.WriteLine("endobj");

                    // Object 4: Contents
                    var contentStream = new StringBuilder();
                    contentStream.AppendLine("BT /F1 12 Tf");
                    contentStream.AppendLine($"100 800 Td ({companyName}) Tj");
                    contentStream.AppendLine($"0 -20 Td (Website: {companyWebsite}) Tj");
                    contentStream.AppendLine($"0 -20 Td (Email: {companyEmail}) Tj");
                    contentStream.AppendLine($"0 -20 Td (Phone: {companyPhone}) Tj");
                    contentStream.AppendLine($"0 -40 Td (Invoice for {userName}) Tj");
                    contentStream.AppendLine($"0 -20 Td (Date: {date:yyyy-MM-dd}) Tj");

                    // Add product lines
                    int yPosition = 720;
                    foreach (var product in products)
                    {
                        contentStream.AppendLine($"0 -20 Td (Product: {product.Name} - ${product.Price:F2}) Tj");
                        yPosition -= 20;
                    }

                    // Add total
                    contentStream.AppendLine($"0 -20 Td (Total: ${total:F2}) Tj");
                    contentStream.AppendLine("ET");

                    var contentBytes = Encoding.ASCII.GetBytes(contentStream.ToString());
                    var contentLength = contentBytes.Length;

                    writer.WriteLine("4 0 obj");
                    writer.WriteLine($"<< /Length {contentLength} >>");
                    writer.WriteLine("stream");
                    writer.WriteLine(contentStream.ToString());
                    writer.WriteLine("endstream");
                    writer.WriteLine("endobj");

                    // Object 5: Font
                    writer.WriteLine("5 0 obj");
                    writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
                    writer.WriteLine("endobj");

                    // Cross-reference table
                    long xrefOffset = fs.Position;
                    writer.WriteLine("xref");
                    writer.WriteLine("0 6");
                    writer.WriteLine("0000000000 65535 f ");
                    writer.WriteLine("0000000010 00000 n ");
                    writer.WriteLine("0000000062 00000 n ");
                    writer.WriteLine("0000000114 00000 n ");
                    writer.WriteLine("0000000182 00000 n ");
                    writer.WriteLine("0000000280 00000 n ");

                    // Trailer
                    writer.WriteLine("trailer");
                    writer.WriteLine("<< /Size 6 /Root 1 0 R >>");
                    writer.WriteLine("startxref");
                    writer.WriteLine(xrefOffset);
                    writer.WriteLine("%%EOF");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
