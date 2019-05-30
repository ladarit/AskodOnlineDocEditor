using AskodOnline.Data.Objects;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace AskodOnline.DataAccess.Mapping
{
    public class FileEntityMapping : IAutoMappingOverride<FileEntity>
    {
        public void Override(AutoMapping<FileEntity> mapping)
        {
            mapping.Table("Savefiles");
            mapping.Id(t => t.Counter);
            mapping.Map(r => r.AuthorId, "AUTHOR_ID");
            mapping.HasMany(x => x.DocSign).KeyColumn("FILE_ID").Cascade.None();

            #region unused fields
            //mapping.Map(r => r.CardId, "CARD_ID");
            //mapping.Map(r => r.SpecifierId, "SPECIFIER_ID");
            //mapping.Map(r => r.ClientId, "CLIENT_ID");
            //mapping.Map(r => r.DoctemplateId, "DOCTEMPLATE_ID");
            //mapping.Map(r => r.CheckUserId, "CHECKUSER_ID");
            //mapping.Map(r => r.FolderId, "FOLDER_ID");
            //mapping.Map(r => r.RecognizeResult, "RECOGNIZE_RESULT");
            //mapping.Map(r => r.ExpeditionId, "EXPEDITION_ID");
            //mapping.Map(r => r.MailNbuId, "MAIL_NBU_ID");
            //mapping.Map(r => r.SourceId, "SOURCE_ID");
            //mapping.Map(r => r.TryText, "TRY_TEXT");
            //mapping.Map(r => r.HashMd5, "HASH_MD5");
            #endregion
        }
    }
}
