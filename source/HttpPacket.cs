﻿/*
* PROJECT:          CosmosHttp Development
* CONTENT:          Base Http packet class (Heavily inspered by https://github.com/2881099/TcpClientHttpRequest)
* PROGRAMMERS:      Valentin Charbonnier <valentinbreiz@gmail.com>
*/

using System;
using System.Collections.Generic;

namespace CosmosHttp
{
    public class HttpPacket : IDisposable
    {
        internal string _domain;
        internal string _ip;
        internal string _method = "GET";
        internal string _charset = "us-ascii";
        internal string _data;
        internal string _head;
        internal Dictionary<string, string> _headers;

        public HttpPacket()
        {
            _headers = new Dictionary<string, string>();
        }

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

        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                _domain = value;
            }
        }

        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
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

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        public void Dispose()
        {
        }
    }
}
