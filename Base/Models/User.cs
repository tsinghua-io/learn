using System;

namespace LearnTsinghua.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }

    public class LocalUser: User
    {
    }
}
