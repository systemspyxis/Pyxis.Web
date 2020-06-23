using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Core.Models
{
    public class pyxUser
    {
        public pyxUser()
        {
            ID = Guid.NewGuid().ToString();
            
            created = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            lastLogin = "";
        }
        [ExplicitKey]
        public string ID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool locked { get; set; }
        public int attempts { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string displayName { get; set; }
        public bool disabled { get; set; }
        public string description { get; set; }
        public string lastLogin { get; set; }
        public string created { get; set; }
        public bool admin { get; set; }

    }
}
