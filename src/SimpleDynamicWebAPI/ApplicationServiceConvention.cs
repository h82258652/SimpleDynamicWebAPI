using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace SimpleDynamicWebAPI
{
    public class ApplicationServiceConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (typeof(IApplicationService).IsAssignableFrom(controller.ControllerType))
                {
                    ConfigureApplicationService(controller);
                }
            }
        }

        private void ConfigureApplicationService(ControllerModel controller)
        {
            ConfigureApiExplorer(controller);
            ConfigureSelector(controller);
            ConfigureParameters(controller);
        }

        private void ConfigureApiExplorer(ControllerModel controller)
        {
            if (!controller.ApiExplorer.IsVisible.HasValue)
            {
                controller.ApiExplorer.IsVisible = true;
            }

            foreach (var action in controller.Actions)
            {
                if (!action.ApiExplorer.IsVisible.HasValue)
                {
                    action.ApiExplorer.IsVisible = true;
                }
            }
        }

        private void ConfigureSelector(ControllerModel controller)
        {
            RemoveEmptySelectors(controller.Selectors);

            if (controller.Selectors.Any(temp => temp.AttributeRouteModel != null))
            {
                return;
            }

            foreach (var action in controller.Actions)
            {
                ConfigureSelector(action);
            }
        }

        private void ConfigureSelector(ActionModel action)
        {
            RemoveEmptySelectors(action.Selectors);

            if (action.Selectors.Count <= 0)
            {
                AddApplicationServiceSelector(action);
            }
            else
            {
                NormalizeSelectorRoutes(action);
            }
        }

        private void ConfigureParameters(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                foreach (var parameter in action.Parameters)
                {
                    if (parameter.BindingInfo != null)
                    {
                        continue;
                    }

                    if (parameter.ParameterType.IsClass &&
                        parameter.ParameterType != typeof(string) &&
                        parameter.ParameterType != typeof(IFormFile))
                    {
                        var httpMethods = action.Selectors.SelectMany(temp => temp.ActionConstraints).OfType<HttpMethodActionConstraint>().SelectMany(temp => temp.HttpMethods).ToList();
                        if (httpMethods.Contains("GET") ||
                            httpMethods.Contains("DELETE") ||
                            httpMethods.Contains("TRACE") ||
                            httpMethods.Contains("HEAD"))
                        {
                            continue;
                        }

                        parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                    }
                }
            }
        }

        private void NormalizeSelectorRoutes(ActionModel action)
        {
            foreach (var selector in action.Selectors)
            {
                if (selector.AttributeRouteModel == null)
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(CalculateRouteTemplate(action)));
                }

                if (selector.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods?.FirstOrDefault() == null)
                {
                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));
                }
            }
        }

        private void AddApplicationServiceSelector(ActionModel action)
        {
            var selector = new SelectorModel();
            selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(CalculateRouteTemplate(action)));
            selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));

            action.Selectors.Add(selector);
        }

        private string CalculateRouteTemplate(ActionModel action)
        {
            var routeTemplate = new StringBuilder();
            routeTemplate.Append("api");

            // 控制器名称部分
            var controllerName = action.Controller.ControllerName;
            if (controllerName.EndsWith("ApplicationService"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - "ApplicationService".Length);
            }
            else if (controllerName.EndsWith("AppService"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - "AppService".Length);
            }
            controllerName += "s";
            routeTemplate.Append($"/{controllerName}");

            // id 部分
            if (action.Parameters.Any(temp => temp.ParameterName == "id"))
            {
                routeTemplate.Append("/{id}");
            }

            // Action 名称部分
            var actionName = action.ActionName;
            if (actionName.EndsWith("Async"))
            {
                actionName = actionName.Substring(0, actionName.Length - "Async".Length);
            }
            var trimPrefixes = new[]
            {
                "GetAll","GetList","Get",
                "Post","Create","Add","Insert",
                "Put","Update",
                "Delete","Remove",
                "Patch"
            };
            foreach (var trimPrefix in trimPrefixes)
            {
                if (actionName.StartsWith(trimPrefix))
                {
                    actionName = actionName.Substring(trimPrefix.Length);
                    break;
                }
            }
            if (!string.IsNullOrEmpty(actionName))
            {
                routeTemplate.Append($"/{actionName}");
            }

            return routeTemplate.ToString();
        }

        private string GetHttpMethod(ActionModel action)
        {
            var actionName = action.ActionName;
            if (actionName.StartsWith("Get"))
            {
                return "GET";
            }

            if (actionName.StartsWith("Put") || actionName.StartsWith("Update"))
            {
                return "PUT";
            }

            if (actionName.StartsWith("Delete") || actionName.StartsWith("Remove"))
            {
                return "DELETE";
            }

            if (actionName.StartsWith("Patch"))
            {
                return "PATCH";
            }

            return "POST";
        }

        private void RemoveEmptySelectors(IList<SelectorModel> selectors)
        {
            for (var i = selectors.Count - 1; i >= 0; i--)
            {
                var selector = selectors[i];
                if (selector.AttributeRouteModel == null &&
                    (selector.ActionConstraints == null || selector.ActionConstraints.Count <= 0) &&
                    (selector.EndpointMetadata == null || selector.EndpointMetadata.Count <= 0))
                {
                    selectors.Remove(selector);
                }
            }
        }
    }
}