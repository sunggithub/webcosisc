namespace Website.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string UserNameContact { get; set; }
        public string UserEmailContact { get; set; }
        public string ContentContact { get; set; }
        public string ContactDate { get; set; }
        public Contact()
        {
            if (UserNameContact == null)
            {
                UserNameContact = "";
            }
            if (UserEmailContact == null)
            {
                UserEmailContact = "";
            }
            if (ContentContact == null)
            {
                ContentContact = "";
            }
            if (ContactDate == null)
            {
                ContactDate = "";
            }
        }
    }
}
