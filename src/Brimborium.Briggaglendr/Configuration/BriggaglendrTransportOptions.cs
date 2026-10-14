namespace Brimborium.Briggaglendr.Configuration;

public class BriggaglendrTransportOptions {
    public string Mode { get; set; } = string.Empty;
}

public record BGTransportConfiguration(
    string Mode
    ) {
}

public record BGTransportConfigurationChange(
    string Key,
    BGTransportConfiguration? Before,
    BGTransportConfiguration? Current,
    bool HasChanged
    ) : BGConfigurationChange<BGTransportConfiguration>(Before, Current, HasChanged) {
    public override void OnChanged(IBriggaglendrEventReceiver eventReceiver) {
        eventReceiver.OnChangedTransport(this);
    }
}

public static partial class BGConfigurationConverter {
    public static BGTransportConfiguration ConvertTransport(
        string keyCluster,
        BriggaglendrTransportOptions option) {
        return new BGTransportConfiguration(
            Mode: option.Mode
            );
    }
}
