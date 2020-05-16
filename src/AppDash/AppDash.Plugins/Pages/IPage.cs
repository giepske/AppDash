using System;
using System.Threading.Tasks;

namespace AppDash.Plugins.Pages
{
    public interface IPage
    {
        PageType PageType { get; }
        bool Refreshable { get; }
        string Url { get; }
        TimeSpan UpdateInterval { get; }
        PluginData CachedData { get; }

        /// <summary>
        /// Will be called after the plugin gets loaded.
        /// </summary>
        /// <returns></returns>
        Task OnAfterLoad();

        /// <summary>
        /// Should be called if the page needs to be updated.
        /// <para>
        /// This method should be called manually.
        /// </para>
        /// </summary>
        /// <param name="pluginData">The data the page will receive.</param>
        /// <returns></returns>
        void UpdateData(PluginData pluginData);

        /// <summary>
        /// Will be called when the page needs to be updated, based on the value of <see cref="Page{TPlugin,TRazorComponent}.UpdateInterval"/>.
        /// <para>
        /// This method will be called for you and should not be called manually.
        /// </para>
        /// <para>
        /// This only needs to be implemented when <see cref="Page{TPlugin,TRazorComponent}.TileType"/> is <c>PageType.UpdatingContent</c>.
        /// </para>
        /// </summary>
        /// <returns>Return a PluginData object to update the page or <see langword="null"/> to not update anything.</returns>
        Task<PluginData> OnUpdateDataRequest();
    }
}