using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace AppDash.Client.Web.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private List<ComponentBase> _components;

        protected override async Task OnInitializedAsync()
        {
            _components = new List<ComponentBase>();

            string s = JsonConvert.False;

            Console.WriteLine(s);

            using (HttpClient httpClient = new HttpClient())
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Console.WriteLine(assembly.FullName);
                }

                string url = _navigationManager.BaseUri + "plugins";

                Console.WriteLine(url);

                var assemblies = await httpClient.GetJsonAsync<List<byte[]>>(url);

                try
                {
                    for (var i = 0; i < assemblies.Count; i++)
                    {
                        Console.WriteLine("Loading " + i);

                        byte[] assemblyBytes = assemblies[i];
                        Assembly assembly = Assembly.Load(assemblyBytes);

                        foreach (var type in assembly.GetTypes())
                        {
                            Console.WriteLine(type.Name);

                            if (type.BaseType == typeof(ComponentBase))
                            {
                                _components.Add((ComponentBase)Activator.CreateInstance(type));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await base.OnInitializedAsync();
        }

        RenderFragment RenderComponent(ComponentBase component)
        {
            return builder =>
            {
                builder.OpenComponent(0, component.GetType());
                builder.AddComponentReferenceCapture(1, inst => { component = (ComponentBase)inst; });
                builder.CloseComponent();
            };
        }
    }
}
