/*
* PROJECT:          CosmosHttp Development
* CONTENT:          Http Response class (Heavily inspered by https://github.com/2881099/TcpClientHttpRequest)
* PROGRAMMERS:      Valentin Charbonnier <valentinbreiz@gmail.com>
*/

using System;
using System.Net;
using System.Text;

namespace CosmosHttp.Client
{
    public class HttpResponse : HttpPacket
    {
        private int _received = 0;
        private HttpStatusCode _statusCode;
        private int _contentLength = -1;
        private string _contentType;
        private string _server;
        private string _content;
        private string _contentEncoding = string.Empty;
        private byte[] _stream = new byte[] { };

        public int Received
        {
            get { return _received; }
            internal set { _received = value; }
        }

        public string TransferEncoding
        {
            get { return _headers["Transfer-Encoding"]; }
            set { _headers["Transfer-Encoding"] = value; }
        }

        public int ContentLength
        {
            get { return _contentLength; }
        }

        public string Content
        {
            get
            {
                if (_content == null)
                {
                    _content = Encoding.ASCII.GetString(_stream);
                }
                return _content;
            }
        }

        public HttpResponse(HttpRequest ie, byte[] headBytes)
        {
            _ip = ie.IP;
            _method = ie.Method;
            _charset = ie.Charset;
            string head = Encoding.ASCII.GetString(headBytes);
            _head = head = head.Trim();
            int idx = head.IndexOf(' ');
            if (idx != -1)
            {
                head = head.Substring(idx + 1);
            }
            idx = head.IndexOf(' ');
            if (idx != -1)
            {
                _statusCode = (HttpStatusCode)int.Parse(head.Remove(idx));
                head = head.Substring(idx + 1);
            }
            idx = head.IndexOf("\r\n");
            if (idx != -1)
            {
                head = head.Substring(idx + 2);
            }

            string[] heads = head.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string h in heads)
            {
                string[] nv = h.Split(new char[] { ':' }, 2);
                if (nv.Length == 2)
                {
                    string n = nv[0].Trim();
                    string v = nv[1].Trim();

                    // Handle specific headers and their unique cases
                    switch (n.ToLower())
                    {
                        case "content-length":
                            if (!int.TryParse(v, out _contentLength)) _contentLength = -1;
                            break;
                        case "content-type":
                            _contentType = v;
                            idx = v.IndexOf("charset=", StringComparison.OrdinalIgnoreCase);
                            if (idx != -1)
                            {
                                string charset = v.Substring(idx + 8).Split(';')[0].Trim();
                                if (string.Compare(_charset, charset, StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    try
                                    {
                                        Encoding testEncode = Encoding.GetEncoding(charset);
                                        _charset = charset;
                                    }
                                    catch (Exception ex)
                                    { 
                                        Cosmos.HAL.Global.debugger.Send("Ex: " + ex.ToString());
                                    }
                                }
                            }
                            break;
                        case "server":
                            _server = v;
                            break;
                        case "content-encoding":
                            _contentEncoding = v;
                            break;
                        // Add more specific headers as needed
                        default:
                            if (_headers.ContainsKey(n))
                            {
                                // Append or replace based on your requirement
                                _headers[n] = v;
                            }
                            else
                            {
                                _headers.Add(n, v);
                            }
                            break;
                    }
                }
            }
        }

        public void SetStream(byte[] bodyBytes)
        {
            _stream = bodyBytes;
            _contentLength = bodyBytes.Length;
        }

        public byte[] GetStream()
        {
            switch (_contentEncoding.ToLower())
            {
                case "gzip":
                    return GZip.Decompress(_stream);
                case "deflate":
                    return Deflate.Decompress(_stream);
                default:
                    return _stream;
            }
        }
    }
}