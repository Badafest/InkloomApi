﻿using Inkloom.Api.Email;

namespace Inkloom.Api.Extensions
{
    public static class EmailServiceExtensions
    {
        public static IServiceCollection AddEmailService<T>(this IServiceCollection services, IConfiguration configuration) where T : EmailService
        {
            services.AddSingleton<IEmailService, T>().ConfigureSmtpOptions<T>(configuration);
            return services;
        }

        public static void ConfigureSmtpOptions<T>(this IServiceCollection services, IConfiguration config) where T : EmailService
        {
            ConfigureSmtpOptions<T>(services, options =>
            {
                var from = config.GetSection("Smtp:From");
                options.From = new(from["Name"], from["Address"]);
                options.Host = config["Smtp:Host"] ?? "";
                var isPortParsed = int.TryParse(config["Smtp:Port"] ?? "", out var port);
                if (isPortParsed)
                {
                    options.Port = port;
                }
                options.UseSsl = config["Smtp:Ssl"] == "true";
                options.Password = config["Smtp:Password"] ?? "";
            });
        }

        public static void ConfigureSmtpOptions<T>(this IServiceCollection services, Action<SmtpOptions> configure) where T : EmailService
        {
            services.Configure(configure);
        }
    }
}
