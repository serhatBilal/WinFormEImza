using System;

namespace WinFormEImza.Objects
{
    [Serializable]
    public class PdfRequestDTO
    {
        public string DonglePassword { get; set; }
        public string SourcePdfPath { get; set; }
        public string TargetPdfPath { get; set; }

    }
}
