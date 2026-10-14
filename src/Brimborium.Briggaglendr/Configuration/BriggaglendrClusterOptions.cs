namespace Brimborium.Briggaglendr.Configuration;

public class BriggaglendrClusterOptions {
    public Dictionary<string, BriggaglendrClusterDestinationOptions> Destination { get; set; } = new();
    public Dictionary<string, BriggaglendrEndpointOptions> Endpoint { get; set; } = new();
}

public record BGClusterConfiguration(
    ImmutableDictionary<string, BGClusterDestinationConfiguration> Destination,
    ImmutableDictionary<string, BGEndpointConfiguration> Endpoint
    ) {
    public static BGClusterConfiguration Empty => new(
        Destination: ImmutableDictionary<string, BGClusterDestinationConfiguration>.Empty,
        Endpoint: ImmutableDictionary<string, BGEndpointConfiguration>.Empty
        );
}

public record BGClusterConfigurationChange(
    string Key,
    BGClusterConfiguration? Before,
    BGClusterConfiguration? Current,
    bool HasChanged
    ) : BGConfigurationChange<BGClusterConfiguration>(Before, Current, HasChanged) {
    public override void OnChanged(IBriggaglendrEventReceiver eventReceiver) {
        eventReceiver.OnChangedCluster(this);
    }
}

public static partial class BGConfigurationConverter {
    public static BGClusterConfiguration ConvertCluster(
        string keyCluster,
        BriggaglendrClusterOptions nextClusterOption,
        ImmutableDictionary<string, BGEndpointConfiguration> targetDictEndpoint) {
        var resultDictEndpoint = new Dictionary<string, BGEndpointConfiguration>();
        foreach (var (optionKey, optionEndpoint) in nextClusterOption.Endpoint) {
            if (!targetDictEndpoint.TryGetValue(optionKey, out var targetEndpointConfiguration)) {
                throw new Exception($"({optionKey})");
            }
            resultDictEndpoint.Add(optionKey, targetEndpointConfiguration);
        }
        BGClusterConfiguration result = new(resultDictEndpoint.ToImmutableDictionary());
        return result;
    }
}
