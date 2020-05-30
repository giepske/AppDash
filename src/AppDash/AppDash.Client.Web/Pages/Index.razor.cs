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

namespace AppDash.Client.Web.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [Inject]
        private PluginLoader PluginLoader { get; set; }

        [Inject]
        private PluginManager PluginManager { get; set; }

        [Inject]
        private TileManager TileManager { get; set; }

        private HubConnection hubConnection;

        private List<PluginTileComponent> _components;

        protected override async Task OnInitializedAsync()
        {
            //set events methods

            //await PluginLoader.LoadPlugins();

            _components = (await TileManager.GetTiles()).ToList();

            Console.WriteLine("Components loaded: " + _components.Count);

            hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/chatHub"))
                .Build();

            hubConnection.On<string, PluginData>("UpdateTileData", async (tileKey, pluginData) =>
            {
                var tile = await TileManager.GetTile(tileKey);

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
