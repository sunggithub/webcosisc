using Microsoft.AspNetCore.Identity;

namespace Website.Models
{
    public class User 
    {
        public int UserId { get; set; }
        public string  UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPassword { get; set; }
        public string UserRole { get; set; }
        public string UserImage { get; set; }
        public User()
        {
            if(UserName == null)
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
            if (UserRole == null)
            {
                UserRole = "";
            }
            if (UserImage == null)
            {
                UserImage = "";
            }
        }
    }
}
