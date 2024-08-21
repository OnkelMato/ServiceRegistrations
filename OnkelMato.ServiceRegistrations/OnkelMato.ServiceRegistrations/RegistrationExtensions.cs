using System.ComponentModel.DataAnnotations;

namespace OnkelMato.ServiceRegistrations
{
    public static class RegistrationExtensions
    {
        private static readonly List<Type> Middlewares = new();
        private static readonly List<Type> UsedMiddlewares = new();

        public static WebApplicationBuilder AddRegistrations<T>(this WebApplicationBuilder builder)
        {
            // read all registrations
            var implementations = typeof(T).Assembly.GetTypes()
                .Where(x => x.IsAssignableTo(typeof(IRegistration)) && !x.IsAbstract)
                .ToArray();

            // configure services
            implementations
                .Where(x => x.IsAssignableTo(typeof(IServiceRegistration)))
                .Select(x => (IServiceRegistration)Activator.CreateInstance(x)!)
                .ToList()
                .ForEach(x => x.Configure(builder));

            // add middlewares
            implementations
                .Where(x => x.IsAssignableTo(typeof(IMiddlewareRegistration)))
                .ToList()
                .ForEach(x => { Middlewares.Add(x); });

            return builder;
        }

        // maybe add params here
        public static WebApplication UseRegistration<TMiddleware>(this WebApplication app)
            where TMiddleware : IMiddlewareRegistration
        {
            // throw exception when done twice
            if (UsedMiddlewares.Contains(typeof(TMiddleware)))
                throw new InvalidOperationException($"Middleware {typeof(TMiddleware).Name} is already used.");

            Activator.CreateInstance<TMiddleware>().Use(app);

            UsedMiddlewares.Add(typeof(TMiddleware));

            return app;
        }

        public static WebApplication ValidateRegistration(this WebApplication app)
        {
            var notUsed = Middlewares.Except(UsedMiddlewares).ToArray();

            if (notUsed.Any())
                throw new ValidationException("The following middlewares were registered but not used: " +
                                              string.Join(", ", notUsed.Select(x => x.FullName)));

            return app;
        }

    }
}
