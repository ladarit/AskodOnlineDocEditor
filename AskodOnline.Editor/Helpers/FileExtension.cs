using System;
using System.IO;

namespace AskodOnline.Editor.Helpers
{
    public class FileExtension
    {
        private readonly string _name;

        // for DevExpress
        public static readonly FileExtension Xls = new FileExtension(".xls");
        public static readonly FileExtension Xlsx = new FileExtension(".xlsx");
        public static readonly FileExtension Rtf = new FileExtension(".rtf");
        public static readonly FileExtension Doc = new FileExtension(".doc");
        public static readonly FileExtension Docx = new FileExtension(".docx");
        public static readonly FileExtension Odt = new FileExtension(".odt");

        private FileExtension(string name)
        {
            _name = name;
        }

        public FileExtension()
        {
        }

        public override string ToString()
        {
            return _name;
        }

        public static bool IsSupportedByDevExpress(string ext)
        {
            return
                string.Compare(ext, Xls.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(ext, Xlsx.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(ext, Rtf.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(ext, Doc.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(ext, Docx.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(ext, Odt.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public DevExpress.XtraRichEdit.DocumentFormat ResolveRichEditFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.Compare(extension, Doc.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return DevExpress.XtraRichEdit.DocumentFormat.Doc;
            }
            if (string.Compare(extension, Docx.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return DevExpress.XtraRichEdit.DocumentFormat.OpenXml;
            }
            if (string.Compare(extension, Rtf.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return DevExpress.XtraRichEdit.DocumentFormat.Rtf;
            }
            if (string.Compare(extension, Odt.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return DevExpress.XtraRichEdit.DocumentFormat.OpenDocument;
            }
            return DevExpress.XtraRichEdit.DocumentFormat.OpenXml;
        }

        public DevExpress.Spreadsheet.DocumentFormat ResolveSpreadSheetFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.Compare(extension, Xls.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return DevExpress.Spreadsheet.DocumentFormat.Xls;
            }
            if (string.Compare(extension, Xlsx.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return DevExpress.Spreadsheet.DocumentFormat.Xlsx;
            }
            return DevExpress.Spreadsheet.DocumentFormat.Xlsx;
        }
    }
}
