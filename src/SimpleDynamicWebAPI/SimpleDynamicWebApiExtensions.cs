using Microsoft.AspNetCore.Mvc;
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

            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new ApplicationServiceConvention());
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

            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new ApplicationServiceConvention());
            });

            return builder;
        }
    }
}