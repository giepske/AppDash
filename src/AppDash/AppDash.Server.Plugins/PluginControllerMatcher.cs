using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AppDash.Plugins.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;

namespace AppDash.Server.Plugins
{
    public class PluginControllerMatcher
    {
        private readonly PluginManager _pluginManager;

        public PluginControllerMatcher(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public bool TryMatch(string pluginKey, string route, out (IPluginController, MethodInfo) pluginController)
        {
            pluginController = default;

            if (route == null)
                return false;

            var controllers = _pluginManager.GetPluginControllers(pluginKey)?.ToList();

            if (controllers == null || !controllers.Any())
                return false;

            route = GetRoute(route);

            foreach (IPluginController controller in controllers)
            {
                var routes = controller.GetType().GetMethods().Where(method =>
                    method.GetCustomAttribute<PluginControllerRouteAttribute>() != null);

                foreach (MethodInfo methodInfo in routes)
                {
                    var pluginControllerRoute = methodInfo.GetCustomAttribute<PluginControllerRouteAttribute>();

                    if (IsMatch(GetRoute(pluginControllerRoute.Template), route))
                    {
                        pluginController = (controller, methodInfo);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsMatch(string template, string route)
        {
            var routeTemplate = TemplateParser.Parse(template);

            var matcher = new TemplateMatcher(routeTemplate, new RouteValueDictionary());

            var values = new RouteValueDictionary();

            return matcher.TryMatch(route, values);
        }

        private RouteValueDictionary GetValues(string template, string route)
        {
            var routeTemplate = TemplateParser.Parse(template);

            var matcher = new TemplateMatcher(routeTemplate, new RouteValueDictionary());

            var values = new RouteValueDictionary();

            matcher.TryMatch(route, values);

            return values;
        }

        public IActionResult Execute(string pluginKey, string route, Stream requestBody,
            IPluginController pluginController, MethodInfo method)
        {
            pluginController.PluginKey = pluginKey;

            route = GetRoute(route);

            var pluginControllerRoute = method.GetCustomAttribute<PluginControllerRouteAttribute>();

            var values = GetValues(GetRoute(pluginControllerRoute.Template), route);

            object[] parameters = GetParameters(method, requestBody, values);

            var result = method.Invoke(pluginController, parameters);

            if (result is IActionResult actionResult)
                return actionResult;

            return new ObjectResult(result);
        }

        private object[] GetParameters(MethodInfo method, Stream requestBody, RouteValueDictionary values)
        {
            List<object> parameters = new List<object>();

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                if (parameter.GetCustomAttribute<FromBodyAttribute>() != null)
                {
                    using (StreamReader reader = new StreamReader(requestBody))
                    {
                        string result = reader.ReadToEndAsync().Result;
                        parameters.Add(JsonConvert.DeserializeObject(result, parameter.ParameterType));
                    }
                }
                else
                {
                    parameters.Add(values.ContainsKey(parameter.Name) ? GetValue(parameter.ParameterType, values[parameter.Name]) : null);
                }
            }

            return parameters.ToArray();
        }

        private object GetValue(Type valueType, object value)
        {
            try
            {
                return Convert.ChangeType(value, valueType);
            }
            catch (Exception)
            {
                if (valueType.IsValueType)
                    return Activator.CreateInstance(valueType);

                return null;
            }
        }

        private string GetRoute(string route)
        {
            return route.StartsWith("/") ? route : "/" + route;
        }
    }
}
