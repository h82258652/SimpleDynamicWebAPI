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
            throw new NotImplementedException();
        }
    }
}