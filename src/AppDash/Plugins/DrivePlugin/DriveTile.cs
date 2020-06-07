using System;
using System.IO;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;

namespace DrivePlugin
{
    [FactoryTile]
    public class DriveTile : UpdatingPluginTile<DrivePlugin, DriveComponent>
    {
        private readonly string _name;

        public DriveTile(DrivePlugin plugin, string name) : base(plugin, TimeSpan.FromMinutes(5))
        {
            _name = name;
        }

        public override Task OnAfterLoad()
        {
            var driveInfo = new DriveInfo(_name);

            CachedData.SetData("DriveName", driveInfo.Name);
            CachedData.SetData("TotalSpace", driveInfo.TotalSize);
            CachedData.SetData("SpaceUsed", driveInfo.TotalSize - driveInfo.TotalFreeSpace);

            return Task.CompletedTask;
        }

        public override Task<PluginData> OnUpdateDataRequest()
        {
            var driveInfo = new DriveInfo(_name);

            CachedData.SetData("DriveName", driveInfo.Name);
            CachedData.SetData("TotalSpace", driveInfo.TotalSize);
            CachedData.SetData("SpaceUsed", driveInfo.TotalSize - driveInfo.TotalFreeSpace);


            return Task.FromResult(CachedData);
        }
    }
}