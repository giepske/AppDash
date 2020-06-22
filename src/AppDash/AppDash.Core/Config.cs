using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AppDash.Core
{
    public static class Config
    {
        public static string ApplicationDirectory =>
            Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
    }
}
