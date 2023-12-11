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
    public class HttpResponse
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

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
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
            Cosmos.HAL.Global.debugger.Send("HttpResponse ctor.");

            _action = ie.IP;
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
                                        // Consider logging the exception
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


            Cosmos.HAL.Global.debugger.Send("HttpResponse head parsed.");
        }

        public void SetStream(byte[] bodyBytes)
        {
            _stream = bodyBytes;
            _contentLength = bodyBytes.Length;
        }
    }
}