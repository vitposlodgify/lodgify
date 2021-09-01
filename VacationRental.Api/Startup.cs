using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using VacationRental.Api.Model;
using VacationRental.Api.Validations;
using VacationRental.Data;
using VacationRental.Data.Models;
using VacationRental.Data.Repositories;
using VacationRental.Services;
using VacationRental.Services.Models;

namespace VacationRental.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(option => {
                option.EnableEndpointRouting = false;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(opts => opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Vacation rental information", Version = "v1" }));

            services.AddScoped<IValidator<BookingBindingModel>, BookingValidator>();
            services.AddScoped<IValidator<CalendarValidation>, CalendarValidator>();
            services.AddScoped<IValidator<RentalBindingModel>, RentalBindingModelValidator>();
            services.AddScoped<IValidator<RentalViewModel>, RentalViewModelValidator>();

            services.AddScoped<IRentalService, RentalService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ICalendarService, CalendarService>();

            services.AddScoped<IRentalRepository, RentalRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            services.AddSingleton(new VacationContext());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "VacationRental v1"));
        }
    }
}
