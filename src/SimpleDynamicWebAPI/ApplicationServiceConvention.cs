using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

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

        private void ConfigureParameters(ControllerModel controller)
        {
            throw new NotImplementedException();
        }

        private void ConfigureSelector(ControllerModel controller)
        {
            throw new NotImplementedException();
        }
    }
}