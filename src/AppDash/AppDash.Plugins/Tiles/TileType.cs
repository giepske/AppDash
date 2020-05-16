namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// The type of tile.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// Tile contains static data that can optionally be manually refreshed.
        /// </summary>
        StaticData,
        /// <summary>
        /// Tile contains data that gets updated on a given interval.
        /// </summary>
        UpdatingData,
        /// <summary>
        /// Tile will receive updated content in real-time.
        /// </summary>
        RealtimeData,
        /// <summary>
        /// Tile will contain a link to either a page in the AppDash application or a 3th party domain.
        /// </summary>
        Link
    }
}