using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppDash.Client.Plugins;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AppDash.Client.Web.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [Inject]
        private TileManager TileManager { get; set; }

        private HubConnection hubConnection;

        private List<PluginTileComponent> _components;

        protected override async Task OnInitializedAsync()
        {
            _components = (await TileManager.GetPluginTileComponents()).ToList();

            hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/chatHub"))
                .ConfigureLogging(options =>
                {
                    options.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            hubConnection.On<string, string, PluginData>("UpdateTileData", async (pluginKey, tileKey, pluginData) =>
            {
                var tile = await TileManager.GetPluginTileComponent(pluginKey, tileKey);

                tile.Data = pluginData;

                if (tile.OnUpdate())
                    tile.StateHasChanged();

                StateHasChanged();
            });

            await hubConnection.StartAsync();

            await base.OnInitializedAsync();
        }
    }
}
