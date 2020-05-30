using System.Linq;
using AppDash.Core;
using Microsoft.AspNetCore.Mvc;
using PluginManager = AppDash.Server.Plugins.PluginManager;

namespace AppDash.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TilesController : ControllerBase
    {
        private readonly PluginManager _pluginManager;

        public TilesController(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        /// <summary>
        /// Get a list of loaded tiles and their CachedData objects.
        /// </summary>
        /// <returns>List with tile type and its CachedData object.</returns>
        [HttpGet]
        public ApiResult Get()
        {
            var tiles = _pluginManager.GetTiles();

            return ApiResult.Success(tiles.Select(tile => new
            {
                name = tile.GetType().Name,
                plugin = tile.GetType().BaseType?.GenericTypeArguments[0].Name,
                pluginTileComponent = tile.GetType().BaseType?.GenericTypeArguments[1].Name,
                cachedData = tile.CachedData
            }));
        }
    }
}
