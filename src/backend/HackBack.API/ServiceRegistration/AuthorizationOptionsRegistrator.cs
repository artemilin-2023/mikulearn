using HackBack.API.Abstractions;
using HackBack.Infrastructure.Auth;
using System.Reflection;

namespace HackBack.API.Extensions
{
    public static class AuthorizationOptionsRegistrator
    {
        // блять какой же говнокод я тут устроил 
        public static IServiceCollection AddAuthorizationPermissionRequirements(this IServiceCollection services)
        {
            var members = Assembly.GetExecutingAssembly()
                .GetTypes();

            var controllerAttributes = members.GetAttributes<PermissionRequiredAttribute>();

            var authorizationAttributes = controllerAttributes
                .GroupBy(attr => attr.Policy) // группируем по политике, где политика - строка в формате "Permission1 Permission2 Permission3"
                .Select(group => group.First()); // берем по одному аттрибуту на каждую политику, остальные - дубликаты

            var builder = services.AddAuthorizationBuilder();

            foreach (var attribute in authorizationAttributes)
            {
                builder.AddPolicy(attribute.Policy!, policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement(attribute!.Permissions));
                });
            }

            return services;
        }

        private static IEnumerable<IPermissionRequiredAttribute> GetAttributes<TAttribute>(this IEnumerable<Type> members) where TAttribute : Attribute, IPermissionRequiredAttribute
            => members.GetAttributesFromMethods<TAttribute>()
                .Concat(members.GetAttributesFromClasses<TAttribute>());

        private static IEnumerable<IPermissionRequiredAttribute> GetAttributesFromMethods<TAttribute>(this IEnumerable<Type> members) where TAttribute: Attribute, IPermissionRequiredAttribute
            => members
                .SelectMany(c => c.GetMethods())
                .FilterAttributes<TAttribute>();

        private static IEnumerable<TAttribute> FilterAttributes<TAttribute>(this IEnumerable<MemberInfo> memberInfos) where TAttribute: Attribute
            => memberInfos
                .Where(m => m.GetCustomAttribute<TAttribute>() != null)
                .Select(m => m.GetCustomAttribute<TAttribute>()!)
                ?? [];

        private static IEnumerable<IPermissionRequiredAttribute> GetAttributesFromClasses<TAttribute>(this IEnumerable<Type> controllerTypes) where TAttribute: Attribute, IPermissionRequiredAttribute
            => controllerTypes.FilterAttributes<TAttribute>();
    }
}
