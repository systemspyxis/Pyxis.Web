using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pyxis.Core.Models
{
    public class Cheque
    {
        public string serialNumber { get; set; }
        public string sortCode { get; set; }
        public string voucherType { get; set; }
        public string accountNumber { get; set; }
        public decimal amount { get; set; }
        public string payeeName { get; set; }
        public string narration { get; set; }
        public string Images { get; set; }
        public string depositorsAccount { get; set; }
        public string depositorsBranch { get; set; }
        public string depositorsNarration { get; set; }
        public string batchGuid { get; set; }
        public string recordID { get; set; }
        public string transactionCaptureDate { get; set; }
        public string transactionCapturedBy { get; set; }
        public string transactionAuthorisedDate { get; set; }
        public string transactionexportDate { get; set; }
        public string stage { get; set; }
    
    }
}
