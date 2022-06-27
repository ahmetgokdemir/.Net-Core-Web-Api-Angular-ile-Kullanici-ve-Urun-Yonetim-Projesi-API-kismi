namespace ServerApp.Models
{
    public class UserToUser
    {
        public int UserId { get; set; }         // Takip edilen id
        public User User { get; set; }          // Takip edilen  
        public int FollowerId { get; set; }     // Takipçi id
        public User Follower { get; set; }      // Takipçi
    }
}