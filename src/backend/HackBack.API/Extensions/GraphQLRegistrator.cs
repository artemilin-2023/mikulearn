using HackBack.API.GraphQl.Abstractions;
using HackBack.API.GraphQL.Abstractions;
using HotChocolate.Execution.Configuration;
using System.Reflection;

namespace HackBack.API.Extensions
{
    public static class GraphQLRegistrator
    {
        public static IServiceCollection AddGraphQL(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                .AddAuthorization()
                .LoadQueryTypes()
                .LoadMutationTypes();

            return services;
        }

        private static IRequestExecutorBuilder LoadQueryTypes(this IRequestExecutorBuilder requestBuilder)
        {
            var queryTypeBuilder = requestBuilder.AddQueryType(t => t.Name(GraphQlTypes.Query));

            var queryTypes = GetRegistrationTypes(GraphQlTypes.Query);

            foreach (var query in queryTypes)
                queryTypeBuilder.AddTypeExtension(query);

            return queryTypeBuilder;
        }


        private static IEnumerable<Type> GetRegistrationTypes(string graphQlType)
        {
            var types = Assembly.GetExecutingAssembly()!
                .GetTypes()
                .Where(t => t.GetCustomAttribute(typeof(ExtendObjectTypeAttribute)) != null)
                .Where(t =>
                    ((ExtendObjectTypeAttribute)t.GetCustomAttribute(typeof(ExtendObjectTypeAttribute))!).Name == graphQlType);

            return types;
        }

        private static IRequestExecutorBuilder LoadMutationTypes(this IRequestExecutorBuilder requestBuilder)
        {
            var mutationTypeBuilder = requestBuilder.AddMutationType(t => t.Name(GraphQlTypes.Mutation));

            var mutationTypes = GetRegistrationTypes(GraphQlTypes.Mutation);

            foreach (var mutation in mutationTypes)
                mutationTypeBuilder.AddTypeExtension(mutation);

            return mutationTypeBuilder;
        }
    }
}
