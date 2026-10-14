namespace Brimborium.Briggaglendr.Configuration;

public class BriggaglendrClusterDestinationOptions {
    public string Address { get; set; } = string.Empty;
    public string Health { get; set; } = string.Empty;
}

public record BGClusterDestinationConfiguration(
    string Address,
    string Health
    );

public record BGClusterDestinationConfigurationChange(
    string Key,
    BGClusterDestinationConfiguration? Before,
    BGClusterDestinationConfiguration? Current,
    bool HasChanged
    ) : BGConfigurationChange<BGClusterDestinationConfiguration>(Before, Current, HasChanged) {
    public override void OnChanged(IBriggaglendrEventReceiver eventReceiver) {
        eventReceiver.OnChangedClusterDestination(this);
    }
}

public static partial class BGConfigurationConverter {
    public static BGClusterDestinationConfiguration ConvertClusterDestination(
        BriggaglendrClusterDestinationOptions nextClusterDestinationOption) {
        BGClusterDestinationConfiguration result = new(
            Address: nextClusterDestinationOption.Address,
            Health: nextClusterDestinationOption.Health
            );
        return result;
    }
}
