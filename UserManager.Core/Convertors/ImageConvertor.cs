using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Core.Convertors
{
    public static class ImageConvertor
    {
        public static byte[] GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static byte[] ImageToByte(IFormFile Image)
        {
            using var memoryStream = new MemoryStream();
            Image.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static string ByteToImage(byte[] ImageByte)
        {
            string imreBase64Data = Convert.ToBase64String(ImageByte);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            return imgDataURL;
        }
    }
}
