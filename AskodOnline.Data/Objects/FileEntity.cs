using System.Collections.Generic;

namespace AskodOnline.Data.Objects
{
    public class FileEntity : BaseEntity
    {
        public virtual string FileName { get; set; }

        public virtual byte[] TextFile { get; set; }

        public virtual long AuthorId { get; set; }

        public virtual IList<DocSignEntity> DocSign { get; set; }

        #region unused fields
        //public virtual long Parent { get; set; }

        //public virtual long Version { get; set; }

        //public virtual DateTime FileDate { get; set; }

        //public virtual DateTime SaveDate { get; set; }

        //public virtual long CardId { get; set; }

        //public virtual long FileSize { get; set; }

        //public virtual string Barcode { get; set; }

        //public virtual string Notice { get; set; }

        //public virtual long CheckOther { get; set; }

        //public virtual byte[] Text { get; set; }

        //public virtual long Accesskey { get; set; }

        //public virtual DateTime Attach { get; set; }

        //public virtual DateTime Detach { get; set; }

        //public virtual long SpecifierId { get; set; }

        //public virtual DateTime CheckDate { get; set; }

        //public virtual long ClientId { get; set; }

        //public virtual long Pages { get; set; }

        //public virtual long DoctemplateId { get; set; }

        //public virtual string NameTable { get; set; }

        //public virtual string NameField { get; set; }

        //public virtual long ValueCounter { get; set; }

        //public virtual long CheckOriginal { get; set; }

        //public virtual long ErrorsCritical { get; set; }

        //public virtual long ErrorsMedium { get; set; }

        //public virtual long CheckUserId { get; set; }

        //public virtual long FileType { get; set; }

        //public virtual byte[] PdfFile { get; set; }

        //public virtual long FolderId { get; set; }

        //public virtual long RecognizeResult { get; set; }

        //public virtual long ExpeditionId { get; set; }

        //public virtual long MailNbuId { get; set; }

        //public virtual long MainDoc { get; set; }

        //public virtual long SourceId { get; set; }

        //public virtual long TryText { get; set; }

        //public virtual DateTime SentDate { get; set; }

        //public virtual long OrderIndex { get; set; }

        //public virtual long CheckPrimary { get; set; }

        //public virtual long HasRegistrationData { get; set; }

        //public virtual string TextShort { get; set; }

        //public virtual byte[] HashMd5 { get; set; }

        //public virtual long FinalizeSign { get; set; }

        //public virtual long SourceType { get; set; }
        #endregion
    }
}
