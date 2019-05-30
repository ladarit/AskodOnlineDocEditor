namespace AskodOnline.Editor.Models
{
    public class FileModel : BaseModel
    {
        public virtual string FileName { get; set; }

        public virtual byte[] TextFile { get; set; }

        public virtual long AuthorId { get; set; }

        public virtual bool IsDocSign { get; set; }

        public virtual long TeamworkId { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(FileName)
                               && AuthorId != default(long)
                               && TeamworkId != default(long)
                               && Counter != default(long);
    }
}