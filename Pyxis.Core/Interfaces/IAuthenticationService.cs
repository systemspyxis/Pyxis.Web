﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Core.Interfaces
{
    public interface IAuthenticationService
    {
        long CreateUser(JObject user);
    }
}
