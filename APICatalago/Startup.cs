using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APICatalago.Context;
using APICatalago.Controllers;
using APICatalago.DTOs.Mappings;
using APICatalago.Extensions;
using APICatalago.Filters;
using APICatalago.Logging;
using APICatalago.Repository;
using AutoMapper;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace APICatalago
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

            // incluindo o servi�o Cors, assim permite requisi��o cross entre url diferente na web, https://www.apirequest.io/
            services.AddCors(options =>
           {
               options.AddPolicy("PermitirApiRequest",
                   builder =>
                   builder.WithOrigins("https://www.apirequest.io")
                       .WithMethods("GET")
                );
           });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper); // ADCIONAR O SERVI�O DE AUTOMAPPER PARA AS CLASSES DE DTO

            services.AddScoped<ApiLoggingFilter>(); // AddScoped permite que cada requisi��o aplica uma nova instancia do servi�o

            services.AddScoped<IUnitOfWork, UnitOfWork>(); // adicionado como servi�o unit of work

            services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>() // Adicionando servi�o do Identity para autentica��o e seguran�a
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //JWT 
            //adicona o manipulador de autentica��o e define o esquema de autentica��o usado : Bearer
            //valida o emissor, a aduiencia e a chave usando a chave secreta valida e assinatura
            object p = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = Configuration["TokenConfiguration:Audience"],
                    ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:key"]))
                });

            //utilizar o data para select, filtra na url
            services.AddOData();

            //Adicionando versionamento para API
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; // assume vers�o padr�o quando nenhuma for informada
                options.DefaultApiVersion = new ApiVersion(1, 0); // definido a vers�o padr�o
                options.ReportApiVersions = true; // adiciona no response do request a informa��o da compatibilidade da vers�o
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version"); // le no cabe�alho http a vers�o da api
            });

            services.AddTransient<IMeuServico, MeuServico>(); // cria o objeto toda vez que criar // add servi�o

            //Swagger
            services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc("v1", new OpenApiInfo
               {
                   Version = "v1",
                   Title = "APICatalogo",
                   Description = "Cat�lago",
                   TermsOfService = new Uri("https://macoratti.net/terms"), // endere�o ficticio
                   Contact = new OpenApiContact
                   {
                       Name = "Lucas",
                       Email = "Lucasandrade595@gmail.com",
                       Url = new Uri("https://www.macoratti.net"), //
                   },
                   License = new OpenApiLicense
                   {
                       Name = "Usar sobre LICX",
                       Url = new Uri("https://macoratti.net/license"),
                   }
               });

               var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
               var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
               c.IncludeXmlComments(xmlPath);

           });


            //autentica��o para o swagger
           // services.AddSwaggerGen(c =>
           //{
           //      c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogoAPI", Version = "v1" });
           //      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
           //      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
           //      c.IncludeXmlComments(xmlPath);

           //      var security = new Dictionary<string, IEnumerable<string>>
           //      {
           //         { "Bearer", new string[] { }},
           //      };

           //c.AddSecurityDefinition(
           //    "Bearer",
           //    new 
           //    {
           //        In = "header",
           //        Description = "Copiar 'bearer' + token'",
           //        Name = "Authorization",
           //        Type = "apiKey"
           //    });
           //    c.AddSecurityRequirement(security);

           //});

            services.AddControllers()
                    .AddNewtonsoftJson(options => //Nuget Microst.AspNetCore.Mvc.NewtonsoftJson
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // ignora a Referencia aciclaca na serializa��o da resposta json
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) // adicionado log
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // adicona o provider do log 
            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            })); ;

            //adiciona o middleware de tratamentos de erros
            //app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            // adiciona o middleware de roteamento
            app.UseRouting();

            //Adiciona o middleware de auatentica��o para o Identity
            app.UseAuthentication();

            //Adiciona o middleware que habilita a autoriza��o para o Identity
            app.UseAuthorization();

            //middleware de autoriza��o
            app.UseAuthorization();

            // permite que minha API, receba requisi��o deste endere�o
            //app.UseCors(opt => opt
            //    .WithOrigins("https://www.apirequest.io")
            //        .WithMethods("GET")); // Apenas metodos Get

            // permite que minha API, receba requisi��o deste endere�o
            app.UseCors();

            //Swagger
            app.UseSwagger();

            //SwaggerUI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cat�logo de Produtos e Categorias");
            });

            app.UseEndpoints(endpoints =>
            {
                //adiciona os endpoinst para as Actions
                // dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}
