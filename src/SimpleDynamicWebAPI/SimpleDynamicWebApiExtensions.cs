using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleDynamicWebAPI
{
    public static class SimpleDynamicWebApiExtensions
    {
        public static IMvcBuilder AddDynamicWebApi(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureApplicationPartManager(applicationPartManager =>
            {
                applicationPartManager.FeatureProviders.Add(new ApplicationServiceControllerFeatureProvider());
            });
            return builder;
        }

        public static IMvcCoreBuilder AddDynamicWebApi(this IMvcCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureApplicationPartManager(applicationPartManager =>
            {
                applicationPartManager.FeatureProviders.Add(new ApplicationServiceControllerFeatureProvider());
            });
            return builder;
        }
    }
}