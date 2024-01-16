namespace Website.Models
{
    public class NewsCategory
    {
        public int NewsCategoryId { get; set; }
        public string NewsCategoryTitle { get; set; }

        public NewsCategory()
        {
            if (NewsCategoryTitle == null)
            {
                NewsCategoryTitle = "";
            }

        }
    }
}
