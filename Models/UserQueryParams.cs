namespace ServerApp.Models
{
    public class UserQueryParams
    {
        public int UserId { get; set; }

        //public bool Followers { get; set; }=false;  
        //public bool Followings { get; set; }=false;  
        
        public bool Followers { get; set; } // true ise kullanıcı takipçilerini istiyor, api/users?followers=true
        public bool Followings { get; set; } // true ise kullanıcı takip ettiği kişileri istiyor, api/users?followings=true
        public string Gender { get; set; }
        public int minAge { get; set; } = 18;
        public int maxAge { get; set; } = 100;
        public string City { get; set; }
        public string Country { get; set; }
    }
}