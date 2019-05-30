namespace AskodOnline.Editor.Models
{
    public class TransactionResult
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsNewFile { get; set; }

        public long NewFileCounter { get; set; }

        public string Command { get; set; }
    }
}