using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using AppDash.Server.Core.Data;

namespace AppDash.Server.Core.Domain.Plugins
{
    public class PluginSettings : BaseEntity
    {
        public string PluginKey { get; set; }
        [Column(TypeName = "jsonb")]
        public string Data { get; set; }
    }
}