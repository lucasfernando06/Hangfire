using Hangfire.Jobs;
using Hangfire.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hangfire
{
    public class Startup
    {
        private readonly CustomName CustomName = new CustomName 
        {
            Logo = "https://lucasfernando.dev/lflogo2.png",
            Style = "width:35px"
        };          

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHangfire(x => x.UseSqlServerStorage("Server=;Database=;Trusted_Connection=True;"));
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }       

            app.UseRouting();     

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {           
                DashboardTitle = new HtmlString($"<img style={CustomName.Style} src={CustomName.Logo} alt='Actio'>").Value
            });

            // Criamos servidores separados de acordo com o tipo de processamento, para conseguir processamento simultâneo
            // Limitamos o worker (1) por servidor, para funcionar como FIFO

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = "Rotinas Server",
                WorkerCount = 1,
                Queues = new[] { "rotinas" }
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = "Valores Server",
                WorkerCount = 1,
                Queues = new[] { "valores" }
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = "Itens Server",
                WorkerCount = 1,
                Queues = new[] { "itens" }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            // adicionando jobs ao mesmo tempo, diferentes filas
            AddJobs();
        }

        // O job é redirecionado para o servidor que possui a fila
        public void AddJobs()
        {
            // Simultâneos personalizados
            BackgroundJob.Enqueue(() => JobQueueService.RotinasJob());
            BackgroundJob.Enqueue(() => JobQueueService.ValoresJob());
            BackgroundJob.Enqueue(() => JobQueueService.ItensJob());

            // Vão pra fila
            BackgroundJob.Enqueue(() => RotinasJobQueueService.RotinasJob()); //Utilizando classe decorator
            BackgroundJob.Enqueue(() => JobQueueService.ValoresJob());
            BackgroundJob.Enqueue(() => JobQueueService.ItensJob());            
            
            // Server default possui 20 workers e não precisa ser mencionado (simultâneo)
            BackgroundJob.Enqueue(() => JobQueueService.DefaultJob());
            BackgroundJob.Enqueue(() => JobQueueService.DefaultJob());
            BackgroundJob.Enqueue(() => JobQueueService.DefaultJob());
        }
    }
}
