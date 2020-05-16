using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace AppDash.Plugins
{
    public class CreateModPackageTask : Task
    {
        [Required]
        public string TargetDir { get; set; }

        public override bool Execute()
        {
            Log.LogError("test");

            return true;
        }
    }
}
