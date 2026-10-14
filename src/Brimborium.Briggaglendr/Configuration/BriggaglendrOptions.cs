namespace Brimborium.Briggaglendr.Configuration;

public class BriggaglendrOptions {
    public Dictionary<string, BriggaglendrRouteOptions> Route { get; set; } = new();
    public Dictionary<string, BriggaglendrClusterOptions> Cluster { get; set; } = new();
    public Dictionary<string, BriggaglendrEndpointOptions> Endpoint { get; set; } = new();
    public Dictionary<string, BriggaglendrTransportOptions> Transport { get; set; } = new();
}

public record BGConfiguration(
    ImmutableDictionary<string, BGRouteConfiguration> Route,
    ImmutableDictionary<string, BGClusterConfiguration> Cluster,
    ImmutableDictionary<string, BGEndpointConfiguration> Endpoint,
    ImmutableDictionary<string, BGTransportConfiguration> Transport
    ) {
    public static BGConfiguration Empty => new BGConfiguration(
        Route: ImmutableDictionary<string, BGRouteConfiguration>.Empty,
        Cluster: ImmutableDictionary<string, BGClusterConfiguration>.Empty,
        Endpoint: ImmutableDictionary<string, BGEndpointConfiguration>.Empty,
        Transport: ImmutableDictionary<string, BGTransportConfiguration>.Empty
        );

    public BGConfiguration AddRoute(string key, BGRouteConfiguration routeConfiguration)
        => this with { Route = this.Route.Add(key, routeConfiguration) };

    public BGConfiguration AddCluster(string key, BGClusterConfiguration clusterConfiguration)
        => this with { Cluster = this.Cluster.Add(key, clusterConfiguration) };

    public BGConfiguration AddEndpoint(string key, BGEndpointConfiguration endpointConfiguration)
        => this with { Endpoint = this.Endpoint.Add(key, endpointConfiguration) };

    public BGConfiguration AddEndpoint(string key, BGTransportConfiguration transportConfiguration)
        => this with { Transport = this.Transport.Add(key, transportConfiguration) };
}

public static partial class BGConfigurationConverter {
    public static BGConfiguration ConvertConfiguration(
        List<BriggaglendrOptions> listNextOptions) {
        BGConfiguration result = BGConfiguration.Empty;
        {
            var listGroupNextOptions = listNextOptions
                .SelectMany(item => item.Endpoint)
                .GroupBy(item => item.Key, item => item.Value)
                ;
            foreach (var groupNextOptions in listGroupNextOptions) {
                var key = groupNextOptions.Key;
                var count = groupNextOptions.Count();
                if (0 == count) { throw new Exception($"Empty Endpoint {key}"); }
                if (1 < count) { throw new Exception($"Multiple {count} Endpoint {key}"); }
                var endpointConfiguration = ConvertEndpoint(key, groupNextOptions.Single());
                result = result.AddEndpoint(key, endpointConfiguration);
            }
        }
        {
            var listGroupNextOptions = listNextOptions
                .SelectMany(item => item.Cluster)
                .GroupBy(item => item.Key, item => item.Value)
                ;
            var resultCluster = ImmutableDictionary<string, BGClusterConfiguration>.Empty;
            foreach (var groupNextOptions in listGroupNextOptions) {
                var key = groupNextOptions.Key;
                var count = groupNextOptions.Count();
                if (0 == count) { throw new Exception($"Empty Cluster {key}"); }
                if (1 < count) { throw new Exception($"Multiple {count} Cluster {key}"); }
                var clusterConfiguration = ConvertCluster(key, groupNextOptions.Single(), result.Endpoint);
                resultCluster = resultCluster.Add(key, clusterConfiguration);
                //result = result.AddCluster(key, clusterConfiguration);
            }
            result = result with { Cluster = resultCluster };
        }
        {
            var listGroupNextOptions = listNextOptions
                .SelectMany(item => item.Route)
                .GroupBy(item => item.Key, item => item.Value)
                ;
            var resultRoute = ImmutableDictionary<string, BGRouteConfiguration>.Empty;
            foreach (var groupNextOptions in listGroupNextOptions) {
                var key = groupNextOptions.Key;
                var count = groupNextOptions.Count();
                if (0 == count) { throw new Exception($"Empty Route {key}"); }
                if (1 < count) { throw new Exception($"Multiple {count} Route {key}"); }
                var routeConfiguration = ConvertRoute(key, groupNextOptions.Single(), result.Cluster);
                resultRoute = resultRoute.Add(key, routeConfiguration);
            }
            result = result with { Route = resultRoute };
        }
        return result;
    }
}

public record BriggaglendrConfigurationChange(
    BGConfiguration? Before,
    BGConfiguration? Current,
    bool HasChanged
    ) : BGConfigurationChange<BGConfiguration>(Before, Current, HasChanged) {
    public override void OnChanged(IBriggaglendrEventReceiver eventReceiver) {
        eventReceiver.OnChangedConfiguration(this);
    }
}