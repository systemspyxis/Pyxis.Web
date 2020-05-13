using Pyxis.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Core.Interfaces
{
    public interface IDataService
    {
        int SaveModel(List<PyxisModel> model);
        int updateModel(List<PyxisModel> model);
        List<PyxisModel> QueryModel();
    }
}
