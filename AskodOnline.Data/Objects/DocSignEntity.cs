using System;

namespace AskodOnline.Data.Objects
{
    public class DocSignEntity : BaseEntity
    {
        public virtual long AuthorId { get; set; }

        public virtual DateTime SignTime { get; set; }
    }
}
