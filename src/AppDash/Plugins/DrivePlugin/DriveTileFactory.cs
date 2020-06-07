using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppDash.Plugins.Tiles;

namespace DrivePlugin
{
    public class DriveTileFactory : ITileFactory<DrivePlugin>
    {
        public IEnumerable<ITile> GetTiles(DrivePlugin plugin)
        {
            var drives = DriveInfo.GetDrives();

            foreach (DriveInfo driveInfo in drives.Where(drive => drive.IsReady))
            {
                yield return new DriveTile(plugin, driveInfo.Name);
            }
        }
    }
}
