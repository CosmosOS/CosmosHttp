using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;

namespace CosmosHttp.Client
{
    internal class HttpResponse
    {
        private string _action;
        private string _method;
        private string _charset;
        private string _head;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private int _received = 0;
        private HttpStatusCode _statusCode;
        private int _contentLength = -1;
        private string _contentType;
        private string _server;
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

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        public HttpResponse(HttpRequest ie, byte[] headBytes)
        {
            Cosmos.HAL.Global.debugger.Send("HttpResponse ctor.");

            _action = ie.Action;
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

            Cosmos.HAL.Global.debugger.Send("HttpResponse head created.");

            string[] heads = head.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string h in heads)
            {
                string[] nv = h.Split(new char[] { ':' }, 2);
                if (nv.Length == 2)
                {
                    string n = nv[0].Trim();
                    string v = nv[1].Trim();
                    if (v.EndsWith("; Secure")) v = v.Replace("; Secure", "");
                    if (v.EndsWith("; version=1")) v = v.Replace("; version=1", "");
                    switch (n.ToLower())
                    {
                        case "content-length":
                            if (!int.TryParse(v, out _contentLength)) _contentLength = -1;
                            break;
                        case "content-type":
                            idx = v.IndexOf("charset=", StringComparison.CurrentCultureIgnoreCase);
                            if (idx != -1)
                            {
                                string charset = v.Substring(idx + 8);
                                idx = charset.IndexOf(";");
                                if (idx != -1) charset = charset.Remove(idx);
                                if (string.Compare(_charset, charset, true) != 0)
                                {
                                    try
                                    {
                                        Encoding testEncode = Encoding.GetEncoding(charset);
                                        _charset = charset;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            _contentType = v;
                            break;
                        case "server":
                            _server = v;
                            break;
                        case "content-encoding":
                            _contentEncoding = v;
                            break;
                        default:
                            _headers.Add(n, v);
                            break;
                    }
                }
            }

            Cosmos.HAL.Global.debugger.Send("HttpResponse head parsed.");
        }

        public void SetStream(byte[] bodyBytes)
        {
            Cosmos.HAL.Global.debugger.Send("HttpResponse SetStream 1.1");
            _stream = bodyBytes;
            Cosmos.HAL.Global.debugger.Send("HttpResponse SetStream 1.2");
            _contentLength = bodyBytes.Length;
            Cosmos.HAL.Global.debugger.Send("HttpResponse SetStream 1.3");
        }
    }
}
