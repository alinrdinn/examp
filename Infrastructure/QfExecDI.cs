using backend_dtap.Modules.Qf.Executive.Application.Interfaces;
using backend_dtap.Modules.Qf.Executive.Application.Services;
using backend_dtap.Modules.Qf.Executive.Entites.Repositories;
using backend_dtap.Modules.Qf.Executive.Infrastructure.Repositories;
using backend_dtap.Modules.Qf.Executive.SecondLayer.Infrastructure;
using backend_dtap.Modules.Qf.Executive.SecondLayer.Network.Application.Interfaces;
using backend_dtap.Modules.Qf.Executive.SecondLayer.Network.Application.Services;
using backend_dtap.Modules.Qf.Executive.SecondLayer.Network.Entites.Repositories;
using backend_dtap.Modules.Qf.Executive.SecondLayer.Network.Infrastructure.Repositories;

namespace backend_dtap.Modules.Qf.Executive.Infrastructure
{
    public static class QfExecDI
    {
        public static IServiceCollection AddQfExecModule(this IServiceCollection services)
        {
            services.AddQfExecSecLayerModule();

            services.AddScoped<IExecService, ExecService>();
            services.AddScoped<IExecRepo, ExecRepo>();
            
            services.AddScoped<IExecutiveCardService, ExecutiveCardService>();
            services.AddScoped<IExecutiveCardRepository, ExecutiveCardRepository>();
            services.AddScoped<IExecutiveV2Service, ExecutiveV2Service>();
            services.AddScoped<IExecutiveV2Repository, ExecutiveV2Repository>();
            return services;
        }
    }
}