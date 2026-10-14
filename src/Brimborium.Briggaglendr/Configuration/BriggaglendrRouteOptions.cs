namespace Brimborium.Briggaglendr.Configuration;

public class BriggaglendrRouteOptions {
    public string Pattern { get; set; } = string.Empty;
    public string Cluster { get; set; } = string.Empty;
}

public record BGRouteConfiguration(
    string Pattern,
    BGClusterConfiguration Cluster
    );

public record BGRouteConfigurationChange(
    string Key,
    BGRouteConfiguration? Before,
    BGRouteConfiguration? Current,
    bool HasChanged
    ) : BGConfigurationChange<BGRouteConfiguration>(Before, Current, HasChanged) {
    public override void OnChanged(IBriggaglendrEventReceiver eventReceiver) {
        eventReceiver.OnChangedRoute(this);
    }
}

public static partial class BGConfigurationConverter {
    public static BGRouteConfiguration ConvertRoute(
        string keyRoute,
        BriggaglendrRouteOptions nextRouteOption,
        ImmutableDictionary<string, BGClusterConfiguration> targetDictCluster) {
        if (!targetDictCluster.TryGetValue(nextRouteOption.Cluster, out var targetCluster)) {
            throw new Exception($"Cluster({nextRouteOption.Cluster}) missing.");
        }
        BGRouteConfiguration result = new(
            Pattern: nextRouteOption.Pattern,
            Cluster: targetCluster
            );
        return result;
    }
}
