using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resources.XML
{
    public static class Config
    {
        public const string _RootTag = @"config";
        public const string _CredentialsTag = @"credentials";
        public const string _TokenTag = @"token";
        public const string _TempTag = @"temp";
        public const string _ClientTag = @"client";
        public const string _AssetsTag = @"assets";
        public const string _PageSizeTag = @"page";
        public const string _UploadTag = @"upload";
        public const string _DownloadTag = @"download";
        public const string _DataTag = @"data";
        public const string _SourceTag = @"source";
        public const string _DestinationTag = @"destination";
        public const string _Extension = @".xml";
    }
}
