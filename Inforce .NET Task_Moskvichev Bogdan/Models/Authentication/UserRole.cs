namespace Inforce_.NET_Task_Moskvichev_Bogdan.Models.Authentication
{
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
