namespace Website.Dtos
{
    public class UserForRegistrationDto
    {
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPassword { get; set; }
        public string UserPasswordConfirm { get; set; }
        public string UserRole { get; set; }
        public UserForRegistrationDto()
        {
            if (UserName == null)
            {
                UserName = "";
            }
            if (UserFirstName == null)
            {
                UserFirstName = "";
            }
            if (UserLastName == null)
            {
                UserLastName = "";
            }
            if (UserPassword == null)
            {
                UserPassword = "";
            }
            if (UserPasswordConfirm == null)
            {
                UserPasswordConfirm = "";
            }
            if (UserRole == null)
            {
                UserRole = "";
            }

        }
    }
}
