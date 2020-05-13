using Microsoft.AspNetCore.SignalR;
using Pyxis.Web.Cheque.Scan.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Web.Cheque.Scan.Hubs
{
    public class ScanHub:Hub
    {
        public IScanner scanner;
        public ScanHub(IScanner sc)
        {
            scanner = sc;
        }

        public async Task InitializeScanner(string user, string message)
        {
            Console.WriteLine("Initializing");
            scanner = new Scanner();
            var status= scanner.Initialize();
            await Clients.All.SendAsync("initResult", user, status);
        }
        public async Task Scan(string user, string message)
        {
   
            var result = scanner.scan();
            await Clients.All.SendAsync("scanResult", user, result);
        }
    }
}
