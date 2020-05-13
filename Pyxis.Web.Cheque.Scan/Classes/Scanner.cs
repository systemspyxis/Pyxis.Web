using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScanCRNet;
using ScanCRNet.Utility;
using ScanCRNet.Base;
using System.Drawing;
using System.Text;
using System.IO;
using System.Drawing.Imaging;

namespace Pyxis.Web.Cheque.Scan.Classes
{
    public class Scanner: IScanner
    {
        static LPFNSCANCALLBACK callBackEvent;
        List<string> MICRCode = new List<string>();
        ScanPage sp = new ScanPage();
        List<ScannedItemPair> listPair;
        List<BatchItem> Batch = new List<BatchItem>();

        public Scanner()
        {
            listPair = new List<ScannedItemPair>();
            sp.OnePageComp += new OnePageCompletedEventHandler(sp_OnpageComp);
            sp.ScanComp += new ScanCompletedEventHandler(sp_ScanComp);
            callBackEvent = OnCallBackFromDriver;
        }

        void sp_OnpageComp(object sender, EventArgs e)
        {
            if (e is OnePageCompletedEventArgs)
            {
                ScannedItemPair pair = ((OnePageCompletedEventArgs)e).Pair;
                listPair.Add(pair);
                //DisplayItem(pair);

            }
        }
        /// <summary>
        /// ScanComplete Event occur.
        /// </summary>
        void sp_ScanComp(object sender, ScanCompletedEventArgs e)
        {
            string strMsg = string.Empty;
            if (e.Error != null)
            {
                strMsg = e.Error.Message;
                //MessageBox.Show(strMsg);
            }
            else
            {
                //btnScan.Enabled = true;
                //btnStop.Enabled = false;
            }
            //dataGridViewBatch.Rows.Clear();
            foreach (BatchItem item in Batch)
            {
                //dataGridViewBatch.Rows.Add(item.Account, item.Amount);
            }
        }
        void OnCallBackFromDriver(int dwReason, int lParam, int nStatus)
        {
            if (dwReason == Csd.ReasonOfCallBack.MICR)
            {
                StringBuilder micr = new StringBuilder(70);
                int Status = Csd.ParGet(CSDP.MICRDATA, micr);
                if (nStatus == Csd.CODE.OK)
                {
                    string s = micr.ToString();
                    MICRCode.Add(s);
                    if (s.Contains("@"))
                    {
                        Csd.ParSet(CSDP.DECIDEPOCKET, Csd.SORTPOCKET.NO3);
                    }
                    else if (s.Length == 0)
                    {
                        Csd.ParSet(CSDP.DECIDEPOCKET, Csd.SORTPOCKET.NO2);
                    }
                    else
                    {
                        Csd.ParSet(CSDP.DECIDEPOCKET, Csd.SORTPOCKET.NO1);
                    }
                }
            }
        }
        public string Initialize()
        {
            try
            {
                int iStatus;
                if ((iStatus = Csd.ProbeEx()) != Csd.CODE.OK)
                {
                    return "Cannot initialize driver. " + ScanUtility.GetErrorMsg(iStatus);
                }
                else
                {
                    return "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                return "Can not load driver. [“" + ex.Message + "]";
                
            }
        }
        public List<BatchItem> scan()
        {
            Csd.ParSet(CSDP.CALLBACK_FUNC, callBackEvent);
            Csd.ParSet(CSDP.MAXDOCUMENT, 0);
            Csd.ParSet(CSDP.SORTBY, Csd.SORTBY.SCANNER);
            Csd.ParSet(CSDP.AUTOSIZE, true);
            //Csd.ParSet(CSDP.FRONT_MODE, Csd.MODE.BINARY);
            Csd.ParSet(CSDP.MICR, Csd.MICRFONT.E13B);
            Csd.ParSet(CSDP.FEEDER, Csd.FEEDER.DUPLEX);

            Csd.ParSet(CSDP.WINDOWCOUNT_FRONT, 2);
            Csd.ParSet(CSDP.WINDOWCOUNT_BACK, 1);
            Csd.ParSet(CSDP.WINDOW, 1);
            Csd.ParSet(CSDP.MODE, Csd.MODE.BINARY);
            Csd.ParSet(CSDP.FILETYPE, Csd.IMGTYPE.TIFF);
            Csd.ParSet(CSDP.WINDOW, 2);
            Csd.ParSet(CSDP.MODE, Csd.MODE.GRAYSCALE);
            Csd.ParSet(CSDP.FILETYPE, Csd.IMGTYPE.JPEG);
            Csd.ParSet(CSDP.WINDOW, -1);
            Csd.ParSet(CSDP.MODE, Csd.MODE.GRAYSCALE);
            Csd.ParSet(CSDP.FILETYPE, Csd.IMGTYPE.JPEG);
            Csd.ParSet(CSDP.WINDOW, 0);
            //MICRCode = new List<string>();
            Csd.StartScan();
            Image img = null;
            CEIIMAGEINFO rr = new CEIIMAGEINFO();
            rr.cbSize = ScanUtility.GetUnmanagedSize(rr);
            rr.lXResolution = 30;
            rr.lYResolution = 30;
            int stat = Csd.ReadPage(ref rr);
            
            int count = 0;
            string Img = "";
            List<BatchItem> Items = new List<BatchItem>();
            BatchItem item = new BatchItem();
            while (stat==0)
            {
                
                count++;
                if (count==1)
                {
                    StringBuilder micr = new StringBuilder(70);
                    int Status = Csd.ParGet(CSDP.MICRDATA, micr);

                    //item.MICR = micr.ToString();

                    PopulateMICRFields(micr.ToString(), item);

                    Csd.SaveImage(ref rr, @"C:\Clearing\" + count + ".tiff", 0, Csd.IMGTYPE.TIFF);
                    Img = Convert.ToBase64String(File.ReadAllBytes(@"C:\Clearing\" + count + ".tiff"));
                    item.Images.Add(Img);
                }
                else
                {
                    Csd.SaveImage(ref rr, @"C:\Clearing\" + count + ".jpeg", 0, Csd.IMGTYPE.JPEG);
                    Img = Convert.ToBase64String(File.ReadAllBytes(@"C:\Clearing\" + count + ".jpeg"));
                    item.Images.Add(Img);
                }

                rr = new CEIIMAGEINFO();
                rr.cbSize = ScanUtility.GetUnmanagedSize(rr);
                rr.lXResolution = 30;
                rr.lYResolution = 30;
                stat = Csd.ReadPage(ref rr);

                if (count==3)
                {
                    Items.Add(item);
                    count = 0;
                    item = new BatchItem();
                }
            }
            
           
            
            return Items;
        
            //Guid taskId = Guid.NewGuid();
            //sp.DoScanAsync(taskId);


        }
        public void PopulateMICRFields(string micr, BatchItem item)
        {
            if (micr.StartsWith("<")&&micr.StartsWith("<"))
            {
                item.MICR.Add(micr.Substring(18, 10));
                item.MICR.Add(micr.Substring(1, 6));
                item.MICR.Add(micr.Substring(8, 6));
                item.MICR.Add(micr.Substring(15, 2));
            }
            else
            {
                item.MICR.Add("");
                item.MICR.Add("");
                item.MICR.Add("");
                item.MICR.Add("");
            }
        }
    }


    public class BatchItem
    {
        public BatchItem()
        {
            this.Images=new List<string>();
            this.MICR = new List<string>();
        }
        public List<string> MICR { get; set; }
        public decimal Amount { get; set; }
        public List<string> Images { get; set; }

    }
}
