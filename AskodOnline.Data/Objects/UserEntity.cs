namespace AskodOnline.Data.Objects
{
    public class UserEntity : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual AvatarEntity Avatar { get; set; }
    }
}
