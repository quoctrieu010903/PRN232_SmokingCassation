using System.Reflection;
using Microsoft.OpenApi.Models;

namespace SmokingCessation.WebAPI.Extensions
{
    public static class SwaggerFeatureExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FPTU_SmokingCessation_API", Version = "v1" });

                // Configure JWT Authentication in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,  // Correct type for Bearer token
                    Scheme = "Bearer", // Capital 'B' to match JWT standard
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                                  "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                                  "Example: \"Bearer 1safsfsdfdfd\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>() // Explicit empty array for scopes
                    }

                });
                // 🟢 Add this to include XML comments
                //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                //c.IncludeXmlComments(xmlPath);
                // 🟢 Load tất cả các file XML tự động
                var baseDir = AppContext.BaseDirectory;

                var xmlFiles = Directory
                    .EnumerateFiles(baseDir, "*.xml", SearchOption.TopDirectoryOnly)
                    .Where(file => Path.GetFileName(file).StartsWith("SmokingCessation"))
                    .ToList();

                foreach (var xmlFile in xmlFiles)
                {
                    Console.WriteLine("✅ XML loaded: " + xmlFile);
                    c.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
                }

            });
          
            return services;
        }

        public static WebApplication WithSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "FPTU_SmokingCessation API V1");
                c.RoutePrefix = "swagger";
            });

            return app;
        }
    }
}