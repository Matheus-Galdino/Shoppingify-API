namespace ShoppingifyAPI.Models
{
    public class KeepUserLogged
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string UserHash { get; set; }
    }
}
