namespace Website.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int CommentNewsId { get; set; }
        public string CommentAuthor { get; set; }
        public string CommentEmail { get; set; }
        public string CommentStatus { get; set; }
        public string CommentContent { get; set; }
        public string CommentDate { get; set; }

        public Comment()
        {
            if (CommentAuthor == null)
            {
                CommentAuthor = "";
            }
            if (CommentEmail == null)
            {
                CommentEmail = "";
            }
            if (CommentStatus == null)
            {
                CommentStatus = "";
            }
            if (CommentContent == null)
            {
                CommentContent = "";
            }
            if (CommentDate == null)
            {
                CommentDate = "";
            }
        }
    }
}
