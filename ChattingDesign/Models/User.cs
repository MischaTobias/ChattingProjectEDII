using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChattingDesign.Models
{
    public class User
    {
        public string Username { get; set; }
        private string Password;

        public void SetPassword(string pass) { Password = pass; }
        public bool CheckPassword(string pass) { return Password == pass; }
    }
}
