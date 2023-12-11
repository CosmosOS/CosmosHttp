﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosHttp.Client
{
    public class HttpRequest : IDisposable
    {
        private TcpClient _client;
        private string _method = "GET";
        private string _remote;
        private string _action;
        private string _data;
        private string _head;
        private string _charset = "us-ascii";
        private int _timeout = 20000;
        private NetworkStream _stream;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private HttpResponse _response;

        public string Method
        {
            get
            {
                return _method;
            }
            set
            {
                _method = value.ToUpper();
            }
        }

        public string Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }
        public string Charset
        {
            get
            {
                return _charset;
            }
            set
            {
                _charset = value;
            }
        }

        public HttpRequest()
        {
            _headers.Add("Connection", "Keep-Alive");
            _headers.Add("Accept", "*/*");
            _headers.Add("User-Agent", "CosmosHttp Client (CosmosOS)");
            _headers.Add("Accept-Language", "en-us");
            _headers.Add("Accept-Encoding", "identity");
        }

        public void Close()
        {
            Cosmos.HAL.Global.debugger.Send("HttpRequest Close.");
            if (_client != null)
            {
                Cosmos.HAL.Global.debugger.Send("HttpRequest Close 1.1");
                if (_stream != null)
                {
                    Cosmos.HAL.Global.debugger.Send("HttpRequest Close 1.2");
                    _stream.Close();
                    Cosmos.HAL.Global.debugger.Send("HttpRequest Close 2.");
                }
                Cosmos.HAL.Global.debugger.Send("HttpRequest Close 3.");
                _client.Close();
                Cosmos.HAL.Global.debugger.Send("HttpRequest Close 4.");
                _client = null;
            }
        }

        public void Send()
        {
            Send(string.Empty);
        }

        public virtual void Send(string data)
        {
            Send(data, 0);
        }

        private void Send(string data, int redirections)
        {
            Cosmos.HAL.Global.debugger.Send("HttpRequest Send.");

            _data = data;
            Cosmos.HAL.Global.debugger.Send("HttpRequest Send - creating _headers.");

            _headers.Remove("Content-Length");
            if (!string.IsNullOrEmpty(data) && string.Compare(_method, "post", true) == 0)
            {
                _headers["Content-Length"] = string.Concat(Encoding.ASCII.GetBytes(data).Length);
                if (string.IsNullOrEmpty(_headers["Content-Type"]))
                {
                    _headers["Content-Type"] = "application/x-www-form-urlencoded; charset=" + _charset;
                }
                else if (_headers["Content-Type"].IndexOf("multipart/form-data") == -1)
                {
                    if (_headers["Content-Type"].IndexOf("application/x-www-form-urlencoded") == -1)
                    {
                        _headers["Content-Type"] += "; application/x-www-form-urlencoded";
                    }
                    if (_headers["Content-Type"].IndexOf("charset=") == -1)
                    {
                        _headers["Content-Type"] += "; charset=" + _charset;
                    }
                }
                data += "\r\n\r\n";
            }
            _headers["Host"] = _action;

            Cosmos.HAL.Global.debugger.Send("HttpRequest Send - _headers created.");

            string http = _method + " " + _action + " HTTP/1.1\r\n";
            foreach (string head in _headers.Keys)
            {
                http += head + ": " + _headers[head] + "\r\n";
            }

            Cosmos.HAL.Global.debugger.Send("HttpRequest Send - http created.");

            http += "\r\n" + data;
            _head = http;
            byte[] request = Encoding.ASCII.GetBytes(http);
            if (_client == null || _remote == null)
            {
                _remote = _action;
                this.Close();
                _client = new TcpClient(_action, 80);
            }
            try
            {
                _stream = getStream();
                _stream.Write(request, 0, request.Length);
            }
            catch
            {
                this.Close();
                _client = new TcpClient(_action, 80);
                _stream = getStream();
                _stream.Write(request, 0, request.Length);
            }
            receive(_stream, redirections, _action);
        }

        protected void receive(Stream stream, int redirections, string action)
        {
            Cosmos.HAL.Global.debugger.Send("HttpRequest receive.");

            // stream.ReadTimeout = _timeout; TO PLUG
            _response = null;
            byte[] bytes = new byte[1024];
            int bytesRead = 0;
            byte[] headBuffer = null;
            byte[] bodyBuffer = null;
            Exception exception = null;

            while (true)
            {
                int idx = -1;
                try
                {
                    bytesRead = stream.Read(bytes, 0, bytes.Length);
                    if (bytesRead == 0)
                    {
                        if (headBuffer == null || headBuffer.Length == 0)
                        {
                            throw new Exception("headBuffer is empty and no more data to read");
                        }
                        break;
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                    break;
                }

                if (_response == null)
                {
                    Cosmos.HAL.Global.debugger.Send("HttpRequest receive _response==null.");

                    // Add the newly read bytes to the head buffer
                    int oldLength = headBuffer != null ? headBuffer.Length : 0;
                    byte[] newHeadBuffer = new byte[oldLength + bytesRead];
                    if (headBuffer != null)
                    {
                        Array.Copy(headBuffer, 0, newHeadBuffer, 0, oldLength);
                    }
                    Array.Copy(bytes, 0, newHeadBuffer, oldLength, bytesRead);
                    headBuffer = newHeadBuffer;

                    // Check for the header delimiter
                    idx = Utils.findBytes(headBuffer, new byte[] { 13, 10, 13, 10 }, 0);
                    if (idx != -1)
                    {
                        // Create the response with the header
                        byte[] header = new byte[idx];
                        Array.Copy(headBuffer, 0, header, 0, idx);
                        _response = new HttpResponse(this, header);
                        _response.Received += headBuffer.Length - idx - 4;

                        // Transfer remaining bytes to the body buffer
                        int bodyLength = headBuffer.Length - idx - 4;
                        bodyBuffer = new byte[bodyLength];
                        Array.Copy(headBuffer, idx + 4, bodyBuffer, 0, bodyLength);

                        Cosmos.HAL.Global.debugger.Send("HttpRequest receive _response==null bodyLength=" + bodyLength);
                        Cosmos.HAL.Global.debugger.Send("HttpRequest receive _response==null headBuffer=" + headBuffer.Length);
                        
                    }
                }
                else
                {
                    Cosmos.HAL.Global.debugger.Send("HttpRequest receive _response!=null.");

                    _response.Received += bytesRead;
                    // Add the newly read bytes to the body buffer
                    int oldLength = bodyBuffer != null ? bodyBuffer.Length : 0;
                    byte[] newBodyBuffer = new byte[oldLength + bytesRead];
                    if (bodyBuffer != null)
                    {
                        Array.Copy(bodyBuffer, 0, newBodyBuffer, 0, oldLength);
                    }
                    Array.Copy(bytes, 0, newBodyBuffer, oldLength, bytesRead);
                    bodyBuffer = newBodyBuffer;
                }

                if (_response != null)
                {
                    if (_response.ContentLength >= 0)
                    {
                        if (_response.ContentLength <= bodyBuffer.Length)
                        {
                            break;
                        }
                    }
                }
            }

            Cosmos.HAL.Global.debugger.Send("HttpRequest receive continue.");

            if (_response == null)
            {
                this.closeTcp();

                // Construct the request headers string
                List<string> sb = new List<string>();
                sb.Add(_method.ToUpper() + " " + _action + " HTTP/1.1");
                foreach (string header in _headers.Keys)
                {
                    sb.Add(header + ": " + _headers[header]);
                }

                // Throw a WebException with the appropriate message
                if (exception == null)
                {
                    throw new WebException("WebException " + string.Join("\r\n", sb.ToArray()));
                }
                else
                {
                    throw new WebException(exception.Message + "\r\n" + string.Join("\r\n", sb.ToArray()), exception);
                }
            }

            Cosmos.HAL.Global.debugger.Send("HttpRequest receive set stream.");

            if (_response.ContentLength >= 0)
            {
                Cosmos.HAL.Global.debugger.Send("HttpRequest receive set stream 1.");
                _response.SetStream(bodyBuffer); // Use bodyBuffer for the response content
                Cosmos.HAL.Global.debugger.Send("HttpRequest receive set stream 1.1");
            }
            else
            {
                Cosmos.HAL.Global.debugger.Send("HttpRequest receive set stream 2.");
                _response.SetStream(bodyBuffer); // Use bodyBuffer for the response content
                Cosmos.HAL.Global.debugger.Send("HttpRequest receive set stream 2.1");
            }

            Cosmos.HAL.Global.debugger.Send("HttpRequest receive close.");

            this.closeTcp();


            Cosmos.HAL.Global.debugger.Send("HttpRequest receive done.");
        }

        protected bool closeTcp()
        {
            this.Close();
            return false;
        }

        protected NetworkStream getStream()
        {
            return _client.GetStream();
        }

        public void Dispose()
        {
            Cosmos.HAL.Global.debugger.Send("HttpRequest Dispose.");
        }
    }

    public class Utils
    {
        public static int findBytes(byte[] source, byte[] find, int startIndex)
        {
            if (find == null) return -1;
            if (find.Length == 0) return -1;
            if (source == null) return -1;
            if (source.Length == 0) return -1;
            if (startIndex < 0) startIndex = 0;
            int idx = -1, idx2 = startIndex - 1;
            do
            {
                idx2 = idx = Array.FindIndex<byte>(source, Math.Min(idx2 + 1, source.Length), delegate (byte b) {
                    return b == find[0];
                });
                if (idx2 != -1)
                {
                    for (int a = 1; a < find.Length; a++)
                    {
                        if (++idx2 >= source.Length || source[idx2] != find[a])
                        {
                            idx = -1;
                            break;
                        }
                    }
                    if (idx != -1) break;
                }
            } while (idx2 != -1);
            return idx;
        }
    }
}
