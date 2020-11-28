using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API
{
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    public static class Utilities
    {
        public static byte[] ImageToByte(string imgpaths, string type)
        {
            try
            {
                string file = System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, imgpaths);
                if (File.Exists(file))
                {
                    return GetImageStreamasByte(file);
                }
                else
                {
                    if (type.Trim() == "header")
                        imgpaths = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + @"\\img\appheader\IMG_20180601.jpg";
                    else if (type.Trim() == "logo")
                        imgpaths = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + @"\\img\cmplogo\IMG_20180601.jpg";
                    else if (type.Trim() == "member")
                        imgpaths = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + @"\\img\memphoto\IMG_20180601.jpg";
                    if (!string.IsNullOrEmpty(imgpaths))
                    {
                        return GetImageStreamasByte(imgpaths);
                    }
                    return null;
                }
            }
            catch (Exception ex) { return null; }
        }

        public static byte[] GetImageStreamasByte(string file)
        {
            Stream input = File.OpenRead(file);
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string GetFileSize(long TotalBytes)
        {
            if (TotalBytes >= 10737824) //Giga Bytes
            {
                decimal FileSize = decimal.Divide(TotalBytes, 1073741824);
                return string.Format("{0:##.##} GB", FileSize);
            }
            else if (TotalBytes >= 1048576) //Mega Bytes
            {
                decimal FileSize = decimal.Divide(TotalBytes, 1048576);
                return string.Format("{0:##.##} MB", FileSize);
            }
            else if (TotalBytes >= 1024) //Kilo Bytes
            {
                decimal FileSize = decimal.Divide(TotalBytes, 1024);
                return string.Format("{0:##.##} KB", FileSize);
            }
            else if (TotalBytes >= 0)
            {
                decimal FileSize = TotalBytes;
                return string.Format("{0:##.##} Bytes", FileSize);
            }
            else
            {
                return "0 Bytes";
            }
        }
    }
}
