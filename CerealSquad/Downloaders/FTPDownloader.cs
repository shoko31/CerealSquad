﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CerealSquad.Downloaders
{
    class FTPDownloader : IDownloader
    {
        public static readonly string FTP_PATH = "ftp://f13-preview.your-hosting.net/";
        private static readonly string FTP_USERNAME = "2233064_ClientGame";
        private static readonly string FTP_PASSWORD = "CerealSquadClient1:";

        private Dictionary<string, string> _Files = new Dictionary<string, string>();

        public bool FileExist(string name)
        {
            if (!_Files.ContainsKey(name))
                return false;
            string storedPath = _Files[name];
            return System.IO.File.Exists(storedPath);
        }

        public string GetFile(string name)
        {
            if (!FileExist(name))
                throw new ArgumentException("File not found");
            return _Files[name];
        }

        public async Task RequireFile(string name, string localPath, Uri distantPath, bool Override = true)
        {
            if (_Files.ContainsKey(name))
                throw new ArgumentException("File already exist");
            if (Override || !System.IO.File.Exists(localPath))
            {
                System.Net.FtpWebRequest ftpRequest = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(distantPath);
                ftpRequest.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                
                ftpRequest.Credentials = new System.Net.NetworkCredential(FTP_USERNAME, FTP_PASSWORD);

                System.Net.FtpWebResponse response = (System.Net.FtpWebResponse) await ftpRequest.GetResponseAsync();

                System.IO.Stream responseStream = response.GetResponseStream();
                System.IO.FileStream fileStream = new System.IO.FileStream(localPath, System.IO.FileMode.Create);

                int Length = 4096;
                Byte[] buffer = new Byte[Length];
                int bytesRead = responseStream.Read(buffer, 0, Length);

                while (bytesRead > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, Length);
                }

                /*System.IO.StreamReader reader = new System.IO.StreamReader(responseStream);

                System.IO.File.WriteAllText(localPath, reader.ReadToEnd(), reader.CurrentEncoding);*/

                //reader.Close();
                fileStream.Close();
                response.Close();
            }
            _Files.Add(name, localPath);
        }
    }
}
