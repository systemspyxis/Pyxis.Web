using Pyxis.Web.Cheque.Scan.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Web.Cheque.Scan
{
    public interface IScanner
    {
        List<BatchItem> scan();
        string Initialize();
    }
}
