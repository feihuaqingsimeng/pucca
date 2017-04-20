using System;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace NAMU
{
    /// <summary>
    /// Provide simple interface for compression and decompression,
    /// using Deflate algorithm.
    /// </summary>
    public static class SimpleCompressor
    {
        /// <summary>
        /// Provide unchecked operations.
        /// </summary>
        public static class Unchecked
        {
            /// <summary>
            /// Compress raw bytes using Deflate.
            /// </summary>
            public static byte[] Compress(byte[] data)
            {
                return DeflateStream.CompressBuffer(data);
            }

            /// <summary>
            /// Decompress raw bytes.
            /// </summary>
            /// <exception cref="Ionic.Zlib.ZlibException">Could be thrown when source data was corrupted.</exception>
            public static byte[] Decompress(byte[] data)
            {
                return DeflateStream.UncompressBuffer(data);
            }

            /// <summary>
            /// Put compressed raw bytes to stream.
            /// </summary>
            public static void Compress(Stream input, Stream output)
            {
                using (var strm = new DeflateStream(output, CompressionMode.Compress))
                {
                    CopyStream(input, output);
                }
            }

            /// <summary>
            /// Decompress input stream to output stream.
            /// </summary>
            /// <exception cref="Ionic.Zlib.ZlibException">Could be thrown when source data was corrupted.</exception>
            public static void Decompress(Stream input, Stream output)
            {
                using (var strm = new DeflateStream(input, CompressionMode.Decompress))
                {
                    CopyStream(input, output);
                }
            }

            private static void CopyStream(Stream input, Stream output, int bufsize = 4096)
            {
                byte[] buf = new byte[bufsize];
                int readsize = input.Read(buf, 0, buf.Length);
                while (readsize > 0)
                {
                    output.Write(buf, 0, readsize);
                    readsize = input.Read(buf, 0, buf.Length);
                }
            }

            /// <summary>
            /// Compress string and return data as base64 string.
            /// </summary>
            /// <exception cref="Ionic.Zlib.ZlibException"></exception>
            public static string CompressString(string s, Encoding encoding)
            {
                return Convert.ToBase64String(Compress(encoding.GetBytes(s)));
            }

            /// <summary>
            /// Decompress string from base64 string.
            /// </summary>
            /// <exception cref="System.FormatException"></exception>
            /// <exception cref="Ionic.Zlib.ZlibException"></exception>
            public static string DecompressString(string base64string, Encoding encoding)
            {
                return encoding.GetString(Decompress(Convert.FromBase64String(base64string)));
            }
        }

        /// <summary>
        /// Call Unchecked.CompressString(), returns null instead if exceptions are thrown.
        /// </summary>
        public static string CompressString(string s, Encoding encoding)
        {
            string result;
            try { result = Unchecked.CompressString(s, encoding); }
            catch { result = null; }
            return result;
        }

        /// <summary>
        /// CompressString() with Encoding.UTF8.
        /// </summary>
        public static string CompressString(string s)
        {
            return CompressString(s, Encoding.UTF8);
        }

        /// <summary>
        /// Call Unchecked.DecompressString(), returns null instead if exceptions are thrown.
        /// </summary>
        public static string DecompressString(string base64string, Encoding encoding)
        {
            string result;
            try { result = Unchecked.DecompressString(base64string, encoding); }
            catch { result = null; }
            return result;
        }

        /// <summary>
        /// DecompressString() with Encoding.UTF8.
        /// </summary>
        public static string DecompressString(string base64string)
        {
            return DecompressString(base64string, Encoding.UTF8);
        }
    }
}
