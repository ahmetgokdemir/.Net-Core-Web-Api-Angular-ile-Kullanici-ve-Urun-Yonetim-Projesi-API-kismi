namespace ServerApp.Models
{
    public class UserQueryParams
    {
        public int UserId { get; set; }
        public bool Followers { get; set; }=false; // true ise kullanıcı takipçilerini istiyor
        public bool Followings { get; set; }=false; // true ise kullanıcı takip ettiği kişileri istiyor
    }
}