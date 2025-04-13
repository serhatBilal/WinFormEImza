using System;
using WinFormEImza.Operations;

namespace WinFormEImza.Objects
{
    internal class SignatureManager
    {
        private PdfSigner _pdfSigner;

        public SignatureManager()
        {
            _pdfSigner = new PdfSigner();
        }

        public string SignPdf(PdfRequestDTO requestDTO)
        {
            string result = "";
            try
            {
                GeneralOperations.GetPolicy();
                result = _pdfSigner.SignPDF(requestDTO);
                GeneralOperations.LogWrite(" Document signed..(" + requestDTO.SourcePdfPath + "-- > " + requestDTO.TargetPdfPath + ")");
            }
            catch (Exception ex)
            {
                GeneralOperations.LogWrite(" An error occurred while signing the file. (" + ex.Message + ")");
            }
            return result;
        }
    }
}
