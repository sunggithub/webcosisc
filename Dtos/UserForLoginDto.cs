namespace Website.Dtos
{
    public partial class UserForLoginDto
    {
        public string Email {get; set;}
        public string UserPassword { get; set;}
        public UserForLoginDto()
        {
            if (Email == null)
            {
                Email = "";
            }
            if (UserPassword == null)
            {
                UserPassword = "";
            }
        }
    }
}