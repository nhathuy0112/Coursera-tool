namespace MyTool
{
    public class AccountInfo
    {
        public AccountInfo(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"Email: {Email}, Password: {Password}";
        }
    }
}