using Brimborium.Briggaglendr.Configuration;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class BriggaglendrCollectionExtensions {
    //Add
    public static IServiceCollection AddBriggaglendr(
        this IServiceCollection services,
        IConfiguration? configuration,
        Action<BriggaglendrOptions>? configureOptions
        ) {
        services.AddBriggaglendrCore();
        var optionsBuilder = services.AddOptions<BriggaglendrOptions>();
        if (configuration is { }) { _ = optionsBuilder.Bind(configuration); }
        if (configureOptions is { }) { optionsBuilder.Configure(configureOptions); }
        return services;
    }

    public static IServiceCollection AddBriggaglendrCore(this IServiceCollection services) {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<BriggaglendrService>();
        services.TryAddSingleton<BGEndpointDataSource>();
        services.TryAddSingleton<BGConnectionListenerFactory>();

#if false
        // Required for IMeterFactory dependency.
        services.AddMetrics();

        services.TryAddTransient<IInlineConstraintResolver, DefaultInlineConstraintResolver>();
        services.TryAddTransient<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton<ObjectPool<UriBuildingContext>>(s => {
            var provider = s.GetRequiredService<ObjectPoolProvider>();
            return provider.Create<UriBuildingContext>(new UriBuilderContextPooledObjectPolicy());
        });

        // The TreeRouteBuilder is a builder for creating routes, it should stay transient because it's
        // stateful.
        services.TryAdd(ServiceDescriptor.Transient<TreeRouteBuilder>(s => {
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var objectPool = s.GetRequiredService<ObjectPool<UriBuildingContext>>();
            var constraintResolver = s.GetRequiredService<IInlineConstraintResolver>();
            return new TreeRouteBuilder(loggerFactory, objectPool, constraintResolver);
        }));

        services.TryAddSingleton(typeof(RoutingMarkerService));

        // Setup global collection of endpoint data sources
        var dataSources = new ObservableCollection<EndpointDataSource>();
        services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<RouteOptions>, ConfigureRouteOptions>(
            serviceProvider => new ConfigureRouteOptions(dataSources)));

        // Allow global access to the list of endpoints.
        services.TryAddSingleton<EndpointDataSource>(s => {
            // Call internal ctor and pass global collection
            return new CompositeEndpointDataSource(dataSources);
        });

        //
        // Default matcher implementation
        //
        services.TryAddSingleton<ParameterPolicyFactory, DefaultParameterPolicyFactory>();
        services.TryAddSingleton<MatcherFactory, DfaMatcherFactory>();
        services.TryAddTransient<DfaMatcherBuilder>();
        services.TryAddSingleton<DfaGraphWriter>();
        services.TryAddTransient<DataSourceDependentMatcher.Lifetime>();
        services.TryAddSingleton<EndpointMetadataComparer>(services => {
            // This has no public constructor.
            return new EndpointMetadataComparer(services);
        });

        // Link generation related services
        services.TryAddSingleton<LinkGenerator, DefaultLinkGenerator>();
        services.TryAddSingleton<IEndpointAddressScheme<string>, EndpointNameAddressScheme>();
        services.TryAddSingleton<IEndpointAddressScheme<RouteValuesAddress>, RouteValuesAddressScheme>();
        services.TryAddSingleton<LinkParser, DefaultLinkParser>();

        //
        // Endpoint Selection
        //
        services.TryAddSingleton<EndpointSelector, DefaultEndpointSelector>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, HttpMethodMatcherPolicy>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, HostMatcherPolicy>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, AcceptsMatcherPolicy>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, ContentEncodingNegotiationMatcherPolicy>());

        //
        // Misc infrastructure
        //
        services.TryAddSingleton<TemplateBinderFactory, DefaultTemplateBinderFactory>();
        services.TryAddSingleton<RoutePatternTransformer, DefaultRoutePatternTransformer>();
        services.TryAddSingleton<RoutingMetrics>();

        // Set RouteHandlerOptions.ThrowOnBadRequest in development
        services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<RouteHandlerOptions>, ConfigureRouteHandlerOptions>());
#endif

        return services;
    }
}
