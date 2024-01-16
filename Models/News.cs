namespace Website.Models
{
    public class News
    {
        public int NewsId { get; set; }
        public int NewsCategoryId { get; set; }
        public string NewsTitle { get; set; }
        public string NewsAuthor { get; set; }
        public string NewsDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsImage { get; set; }
        public string NewsStatus { get; set; }
        public int NewsView { get; set; }

        public News()
        {
            if (NewsTitle == null)
            {
                NewsTitle = "";
            }
            if (NewsAuthor == null)
            {
                NewsAuthor = "";
            }
            if (NewsDate == null)
            {
                NewsDate = "";
            }
            if (NewsContent == null)
            {
                NewsContent = "";
            }
            if (NewsImage == null)
            {
                NewsImage = "";
            }
            if (NewsStatus == null)
            {
                NewsStatus = "";
            }
            
        }

    }
}
