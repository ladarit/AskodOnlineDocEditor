namespace AskodOnline.Editor.Models
{
    public class UserModel : BaseModel
    {
        public string Name { get; set; }

        public string AvatarPath { get; set; }

        public BrowserModel Agent { get; set; }
    }
}