using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosHttp
{
    public static class GZip
    {
        public static byte[] Decompress(Stream stream)
        {
            try
            {
                stream.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Ionic.Zlib.GZipStream gzip = new Ionic.Zlib.GZipStream(stream, Ionic.Zlib.CompressionMode.Decompress))
                    {
                        byte[] data = new byte[1024];
                        int size = 0;
                        while ((size = gzip.Read(data, 0, data.Length)) > 0)
                        {
                            ms.Write(data, 0, size);
                        }
                    }
                    return ms.ToArray();
                }
            }
            catch { return (stream as MemoryStream).ToArray(); };
        }
        public static byte[] Decompress(byte[] bt)
        {
            return Decompress(new MemoryStream(bt));
        }
        public static byte[] Compress(string text)
        {
            return Compress(Encoding.UTF8.GetBytes(text));
        }
        public static byte[] Compress(byte[] bt)
        {
            return Compress(bt, 0, bt.Length);
        }
        public static byte[] Compress(byte[] bt, int startIndex, int length)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Ionic.Zlib.GZipStream gzip = new Ionic.Zlib.GZipStream(ms, Ionic.Zlib.CompressionMode.Compress))
                {
                    gzip.Write(bt, startIndex, length);
                }
                return ms.ToArray();
            }
        }
    }

    public static class Deflate
    {
        public static byte[] Decompress(Stream stream)
        {
            try
            {
                stream.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Ionic.Zlib.DeflateStream def = new Ionic.Zlib.DeflateStream(stream, Ionic.Zlib.CompressionMode.Decompress))
                    {
                        byte[] data = new byte[1024];
                        int size = 0;
                        while ((size = def.Read(data, 0, data.Length)) > 0)
                        {
                            ms.Write(data, 0, size);
                        }
                    }
                    return ms.ToArray();
                }
            }
            catch { return (stream as MemoryStream).ToArray(); };
        }
        public static byte[] Decompress(byte[] bt)
        {
            return Decompress(new MemoryStream(bt));
        }
        public static byte[] Compress(string text)
        {
            return Compress(Encoding.UTF8.GetBytes(text));
        }
        public static byte[] Compress(byte[] bt)
        {
            return Compress(bt, 0, bt.Length);
        }
        public static byte[] Compress(byte[] bt, int startIndex, int length)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Ionic.Zlib.DeflateStream def = new Ionic.Zlib.DeflateStream(ms, Ionic.Zlib.CompressionMode.Compress))
                {
                    def.Write(bt, startIndex, length);
                }
                return ms.ToArray();
            }
        }
    }
}
