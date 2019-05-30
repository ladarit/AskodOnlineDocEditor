namespace AskodOnline.AdminDAL.Objects
{
	public class AdminEntity : BaseEntity
	{
		public virtual string Login { get; set; }

		public virtual string Password { get; set; }

		public virtual string RefreshToken { get; set; }
	}
}
