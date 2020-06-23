using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Pyxis.Core.Interfaces;
using Pyxis.Core.Models;
using Scrypt;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public long CreateUser(JObject model)
        {


            pyxUser user = new pyxUser();

            using (SqlConnection connection = new SqlConnection(@"Server=.\sql2017;Database=Pyxis;User Id=sa;Password=Today123;"))
            {
                ScryptEncoder encoder = new ScryptEncoder();
                string hashedPassword = encoder.Encode(model["password"].ToString());

                user.admin = false;
                user.description = model["description"].ToString();
                user.username = model["username"].ToString();
                user.password= hashedPassword;
                user.displayName = model["fullName"].ToString();
                user.firstName = model["firstname"].ToString();
                user.lastName = model["lastname"].ToString();

                try
                {
                    var res = connection.Insert(user);
                    return res;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        public List<pyxUser> GetUsers()
        {
            using (SqlConnection connection = new SqlConnection(@"Server=.\sql2017;Database=Pyxis;User Id=sa;Password=Today123;"))
            {
                try
                {
                    var res = connection.Query<pyxUser>(@"SELECT * FROM pyxUsers" ).ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
