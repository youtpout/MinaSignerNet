namespace MinaAspAuth
{
    public class LoginVM
    {
        public string Signer { get; set; } // Ethereum account that claim the signature
        public string Field { get; set; } // The signature R composant
        public string Scalar { get; set; } // The signature S composant
        public string Message { get; set; } // The plain message
    }

    public class UserVM
    {
        public string Account { get; set; } // Unique account name (the Ethereum account)
        public string Name { get; set; } // The user name
        public string Email { get; set; } // The user Email
    }

    public class ConnectionVM
    {
        public string Account { get; set; }
        public Guid Nonce { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class MessageVM
    {
        public string Account { get; set; }
        public string Message { get; set; }
    }

    public class TokenVM
    {
        public string Token { get; set; }
    }
}
