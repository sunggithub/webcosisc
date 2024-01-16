namespace Website.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductAuthor { get; set; }
        public string ProductImage { get; set; }
        public string ProductDescription { get; set; }
        public string ProductStatus { get; set; }
        public string ProductReleaseDate { get; set; }

        public Product()
        {
            if (ProductTitle == null)
            {
                ProductTitle = "";
            }
            if (ProductAuthor == null)
            {
                ProductAuthor = "";
            }
            if (ProductImage == null)
            {
                ProductImage = "";
            }
            if (ProductDescription == null)
            {
                ProductDescription = "";
            }
            if (ProductStatus == null)
            {
                ProductStatus = "";
            }
            if (ProductReleaseDate == null)
            {
                ProductReleaseDate = "";
            }
        }
    }
}
