using ahis.template.identity.Contexts;
using ahis.template.identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IdentityContext>(
                options => options.UseSqlServer(connectionString)
                );
            services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                options.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            return services;
        }
    }
}
