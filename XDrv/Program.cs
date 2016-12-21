using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using System.Runtime.ExceptionServices;

namespace XDrv
{
    class Program
    {
        [HandleProcessCorruptedStateExceptions]
        static void Main(string[] args)
        {
            if (4 > args.Length)
            {
                Console.WriteLine("USAGE   : XDrv Account_Name \"Account_Key\" container_name page_blob_name");
                Console.WriteLine("Example : XDrv myaccount \"abef736498723afe19237871239871293879871231212==\" cloud_drive_container mypgblob");
                Console.WriteLine("          will mount the page blob as a drive.");
                return;
            }
            string sAcct = args[0];
            string sKey = args[1];
            string sContainer = args[2];
            string sVHD = args[3];
            string drvcreate = "";
            string sDrv = "";

            if (args.Length == 5)
                drvcreate = args[4];

            string soput = "XDrv " + sAcct + " " + sKey + " " + sContainer + " " + sVHD + " " + drvcreate;
            Console.WriteLine(soput);
            try
            {
                StorageCredentialsAccountAndKey scak = new StorageCredentialsAccountAndKey(sAcct, sKey);
                CloudStorageAccount csa = new CloudStorageAccount(scak, false);
                CloudBlobClient bc = csa.CreateCloudBlobClient();
                Console.WriteLine("1. Get container : " + sContainer);
                CloudBlobContainer bcon = bc.GetContainerReference(sContainer);
                bcon.CreateIfNotExist();
                Console.WriteLine("1. Done!");

                Console.WriteLine("2. Get page blob reference : " + String.Format("{0}/{1}", sContainer, sVHD));
                CloudPageBlob pb = bc.GetPageBlobReference(String.Format("{0}/{1}", sContainer, sVHD));
                Console.WriteLine("2. Done!");

                if (drvcreate == "Y")
                {
                    Console.WriteLine("2.5.Delete existing page blob : " + String.Format("{0}/{1}", sContainer, sVHD));
                    pb.DeleteIfExists();
                    Console.WriteLine("2.5. Done!");
                }
                Console.WriteLine("3. Got Cloud drive reference : ");
                CloudDrive mydrive = new CloudDrive(pb.Uri, pb.ServiceClient.Credentials);
                Console.WriteLine("3. Done!");

                if (drvcreate == "Y")
                {
                    Console.WriteLine("3.5.Create NTFS drive : " + String.Format("{0}/{1}", sContainer, sVHD));
                    mydrive.Create(500);
                    Console.WriteLine("3.5. Done!");
                }
                sDrv = mydrive.Mount(0, DriveMountOptions.None);
                Console.WriteLine("4. Mounted drive : " + sDrv);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press any key!");
            Console.ReadKey();
        }
    }
}
