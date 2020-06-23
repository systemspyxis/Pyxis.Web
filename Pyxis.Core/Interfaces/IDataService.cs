using Newtonsoft.Json.Linq;
using Pyxis.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Core.Interfaces
{
    public interface IDataService
    {
        long SaveModel(List<JObject> model);
        int updateModel(JObject model);
        List<Cheque> QueryModel(string filter);
        bool GenerateFiles();
        
    }
}
