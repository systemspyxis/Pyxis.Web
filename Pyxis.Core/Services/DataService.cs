using Newtonsoft.Json.Linq;
using Pyxis.Core.Interfaces;
using Pyxis.Core.Models;
using System;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;

namespace Pyxis.Core.Services
{
    public class DataService : IDataService
    {
        public bool GenerateFiles()
        {
            using (SqlConnection connection = new SqlConnection(@"Server=.\sql2017;Database=Pyxis;User Id=sa;Password=Today123;"))
            {
                try
                {
                    var res = connection.Query<Cheque>(@"SELECT * FROM Cheques where stage='EXPORT'").ToList();
                    WriteFile(res);
                    var resfin = connection.Execute(@"UPDATE Cheques SET stage='EXPORTED'  WHERE stage='EXPORT'");
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            throw new NotImplementedException();
        }
        private void WriteFile(List<Cheque> records)
        {
            foreach (var item in records)
            {
                string exchangeDate = DateTime.Now.ToString("ddMMyyyy");
                string PresentingCC = "01",header="", fileSerial="", BankCode= item.sortCode.Substring(0, 2);
                string ControlVoucher = "", DRN="", chequeRecord="", ReasonCode="00", VoucherType=item.voucherType;
                double CHQValue = 0;
                string fileName = item.sortCode.Substring(0,2) + exchangeDate + "0100" + ".J" + PresentingCC;
                fileName = @"C:\Clearing\OUTWARDS\" + fileName;
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    //Write the header Record
                    header = "182" + exchangeDate + PresentingCC + "000000";
                    fileSerial = DateTime.Now.ToString("0MMddfff");
                    header += BankCode + "000000" + fileSerial;
                    header += "0";
                    header += "0".PadRight(83, '0');
                    header += "\r\n";
                    writer.Write(header.ToCharArray());
                    //Write TCV
                    ControlVoucher = "16700000000000000000";
                    ControlVoucher += BankCode;
                    ControlVoucher += "0".PadRight(36, '0') + PresentingCC + "0".PadRight(3, '0');
                    DRN = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                    DRN = "0" + PresentingCC + DRN;
                    ControlVoucher += "000001" + DRN + "\r\n";
                    writer.Write(ControlVoucher.ToCharArray());
                    //write Cheque
                    chequeRecord = ReasonCode + VoucherType;
                    CHQValue = (double)item.amount;
                    CHQValue = CHQValue * 100;
                    chequeRecord += CHQValue.ToString().PadLeft(13, '0') + "0";
                    chequeRecord += BankCode + item.sortCode.Substring(2, 3);
                    chequeRecord += item.accountNumber + item.sortCode.Substring(5);
                    chequeRecord += int.Parse(VoucherType) <= 30 ? "00" : VoucherType;
                    DRN = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                    DRN = "0" + PresentingCC + DRN;
                    chequeRecord += "00" + "0".PadRight(20, '0');
                    if (true)
                    {
                        chequeRecord += "test Payee".PadRight(35, ' ');
                    }
                    chequeRecord += PresentingCC + item.depositorsBranch + item.serialNumber + DRN;
                    writer.Write(chequeRecord.ToCharArray());

                    using (MemoryStream ms = new MemoryStream())
                    {
                        
                        var Data = Convert.FromBase64String(item.frontTiff);
                        //write length
                        writer.Write(Data.Length);
                        SHA384 sha384 = new SHA384CryptoServiceProvider();
                        byte[] sig = sha384.ComputeHash(Data);
                        writer.Write(sig);

                        ;
                        Data = Convert.FromBase64String(item.frontJpeg); 
                        //write length
                        writer.Write(Data.Length);
                        sha384 = new SHA384CryptoServiceProvider();
                        sig = sha384.ComputeHash(Data);
                        writer.Write(sig);

                        
                        Data = Convert.FromBase64String(item.backJpeg);
                        //write length
                        writer.Write(Data.Length);
                        sha384 = new SHA384CryptoServiceProvider();
                        sig = sha384.ComputeHash(Data);
                        writer.Write(sig);

                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var Data = Convert.FromBase64String(item.frontTiff);
                        //write Image
                        writer.Write(Data);

                        Data = Convert.FromBase64String(item.frontJpeg);
                        //write Image
                        writer.Write(Data);

                        Data = Convert.FromBase64String(item.backJpeg); ;
                        //write Image
                        writer.Write(Data);
                        writer.Write("\r\n".ToCharArray());
                    }

                    //Write BCV
                    ControlVoucher = "16710000000000000000";
                    ControlVoucher += item.sortCode.Substring(0, 2);
                    ControlVoucher += "0".PadRight(36, '0') + PresentingCC + "0".PadRight(3, '0');
                    DRN = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                    DRN = "0" + PresentingCC + DRN;
                    ControlVoucher += "000001" + DRN + "\r\n";
                    writer.Write(ControlVoucher.ToCharArray());

                    //Write the trailer Record
                    header = "19" + PresentingCC + "0".PadRight(114, '0');
                    header += "\r\n";
                    writer.Write(header.ToCharArray());


                }
            }

            
        }

        public List<Cheque> QueryModel(string filter)
        {
            using (SqlConnection connection = new SqlConnection(@"Server=.\sql2017;Database=Pyxis;User Id=sa;Password=Today123;"))
            {
                try
                {
                    var res = connection.Query<Cheque>(@"SELECT * FROM Cheques where stage=@Filter",new {Filter=filter }).ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public long SaveModel(List<JObject> model)
        {
            var batchGuid = Guid.NewGuid().ToString();
            List<Cheque> cheques = new List<Cheque>();
            foreach (var item in model)
            {
                Cheque CHQ = new Cheque();

                CHQ.accountNumber =  item["accountNumber"].ToString();
                CHQ.amount = decimal.Parse(item["amount"].ToString());
                CHQ.batchGuid = batchGuid;
                CHQ.depositorsAccount = item["depositorsAccount"].ToString();
                CHQ.depositorsBranch = item["depositorsBranch"].ToString();
                CHQ.depositorsNarration = item["depositorsNarration"].ToString();
                CHQ.narration = item["narration"].ToString();
                CHQ.payeeName = item["payeeName"].ToString();
                CHQ.recordID = Guid.NewGuid().ToString();
                CHQ.modelID = Guid.NewGuid().ToString();
                CHQ.serialNumber = item["serialNumber"].ToString();
                CHQ.sortCode = item["sortCode"].ToString();
                CHQ.stage = "VERIFY";
                CHQ.transactionAuthorisedDate = "";
                CHQ.transactionCaptureDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                CHQ.transactionCapturedBy = "Admin";
                CHQ.voucherType = item["voucherType"].ToString();
                CHQ.frontTiff = item["Images"][0].ToString();
                CHQ.frontJpeg = item["Images"][1].ToString();
                CHQ.backJpeg = item["Images"][2].ToString();

                cheques.Add(CHQ);
            }
            using (SqlConnection connection = new SqlConnection(@"Server=.\sql2017;Database=Pyxis;User Id=sa;Password=Today123;"))
            {
                try
                {
                    var res = connection.Insert<List<Cheque>>(cheques);
                    return res;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            
        }

        public int updateModel(JObject model)
        {
            using (SqlConnection connection = new SqlConnection(@"Server=.\sql2017;Database=Pyxis;User Id=sa;Password=Today123;"))
            {
                try
                {
                    //var chq = connection.Get<Cheque>(model["recordID"].ToString());
                    var AuthDate= DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    var res = connection.Execute(@"UPDATE Cheques SET stage='EXPORT' ,transactionAuthorisedDate=@authDate WHERE modelID=@modelID", new { modelID= model["modelID"].ToString(), authDate = AuthDate });

                    return res;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }
    }
}
