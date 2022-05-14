namespace SharedLibrary
{

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public int Score { get; set; }
        public List<Hero> Heroes { get; set; }
    }
}