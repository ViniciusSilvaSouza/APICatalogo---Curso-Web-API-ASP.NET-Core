using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filter;
using APICatalogo.Logging;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogo
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

            services.AddCors( opt =>  {
                opt.AddPolicy("PermitirApiRequest", builder => builder.WithOrigins("").WithMethods("GET"));
            }
            );
            //! Adiciona AutoMapper 
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); }
            );
            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddScoped<APILoggingFilter>();

            // Connection String da aplicação
            string sqlConnection = Configuration.GetConnectionString("DefaultConnection");

            //  Adiciona DBContext da aplicação
            services.AddDbContext<AppDbContext>(options =>
           options.UseMySql(sqlConnection, ServerVersion.AutoDetect(sqlConnection)));


            //! Serviço que adiciona Identity na aplicação
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            /*! 
             * JWT
             * Adiciona o manipulador de autenticação e define o esquema de autenticação usado: Bearer
             * valida o emissor, a audiencia e a chave usando a chave secreta, depois valida assinatura
            */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidAudience = Configuration["TokenConfiguration:Audience"],
                ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            });

            services.AddTransient<IMeuServico, MeuServico>();

            //! Adição do UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICatalogo", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo v1"));
            }

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();
            // Adiciona middleware de autencicação
            app.UseRouting();
            // Adiciona middleware de autenticaçao
            app.UseAuthentication();

            //!Adiciona middleware de autorização
            app.UseAuthorization();


            app.UseCors();
            // Adiciona o middleware que executa o endpoint do request atual
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
