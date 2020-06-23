using Newtonsoft.Json.Linq;
using Pyxis.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Core.Interfaces
{
    public interface IAuthenticationService
    {
        long CreateUser(JObject user);
        List<pyxUser> GetUsers();
    }
}
