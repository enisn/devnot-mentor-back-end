using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DevnotMentor.Api.Entities;
using DevnotMentor.Api.Helpers;
using DevnotMentor.Api.Services;
using DevnotMentor.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DevnotMentor.Api.Middleware;
using DevnotMentor.Api.ActionFilters;
using DevnotMentor.Api.Utilities.Security.Token;

namespace DevnotMentor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");

            services.Configure<AppSettings>(appSettingsSection);
            services.Configure<ResponseMessages>(Configuration.GetSection("ResponseMessages"));
            services.AddDbContext<MentorDBContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddControllers();
            services.AddTokenAuthentication(appSettingsSection);
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMentorService, MentorService>();
            services.AddScoped<IMenteeService, MenteeService>();

            services.AddSingleton<ITokenService, JwtTokenService>();
            services.AddSingleton<TokenAuthentication>();

            services.AddCors(options =>
            {
                options
                .AddPolicy("AllowMyOrigin", builder =>
                    builder
                    //.WithOrigins("http://localhost:8080")
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddCustomSwagger();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseCors("AllowMyOrigin");
            app.UseCustomSwagger();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}