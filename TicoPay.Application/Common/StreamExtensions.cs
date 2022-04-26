using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Common
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.GetBuffer();
        }
    }
}
