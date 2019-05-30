using System.Collections.Generic;

namespace AskodOnline.Editor.Models
{
    public class UserGroupModel : BaseModel
    {
        public long DocumentCounter { get; set; }

        public SynchronizedCollection<UserModel> Users { get; set; }
    }
}