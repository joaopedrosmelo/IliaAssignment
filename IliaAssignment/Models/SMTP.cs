using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Models
{
    public class SMTP
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
