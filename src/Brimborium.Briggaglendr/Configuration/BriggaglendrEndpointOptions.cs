namespace Brimborium.Briggaglendr.Configuration;

public class BriggaglendrEndpointOptions {
    public string Pattern { get; set; } = string.Empty;
}

public record BGEndpointConfiguration(
    string Pattern
    ) {
    public static BGEndpointConfiguration Empty 
        => new BGEndpointConfiguration(
            Pattern: string.Empty
        );
}

public record BGEndpointConfigurationChange(
    string KeyCluster,
    string KeyEndpoint,
    BGEndpointConfiguration? Before,
    BGEndpointConfiguration? Current,
    bool HasChanged
    ) : BGConfigurationChange<BGEndpointConfiguration>(Before, Current, HasChanged) {
    public override void OnChanged(IBriggaglendrEventReceiver eventReceiver) {
        eventReceiver.OnChangedEndpoint(this);
    }
}


public static partial class BGConfigurationConverter {
    public static BGEndpointConfiguration ConvertEndpoint(
        string keyEndpoint,
        BriggaglendrEndpointOptions source) {
        return new BGEndpointConfiguration(
            Pattern: source.Pattern
            );
    }
}