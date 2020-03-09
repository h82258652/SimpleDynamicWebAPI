using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;

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
            throw new NotImplementedException();
        }

        private void ConfigureParameters(ControllerModel controller)
        {
            throw new NotImplementedException();
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