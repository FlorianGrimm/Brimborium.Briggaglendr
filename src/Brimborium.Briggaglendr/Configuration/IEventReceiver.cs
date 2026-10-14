namespace Brimborium.Briggaglendr.Configuration;

public interface IBriggaglendrEventReceiver {
    void OnChangedConfiguration(BriggaglendrConfigurationChange change);
    void OnChangedRoute(BGRouteConfigurationChange change);
    void OnChangedCluster(BGClusterConfigurationChange change);
    void OnChangedClusterDestination(BGClusterDestinationConfigurationChange change);
    void OnChangedEndpoint(BGEndpointConfigurationChange change);
    void OnChangedTransport(BGTransportConfigurationChange change);
}

public class BriggaglendrEventReceiver : IBriggaglendrEventReceiver {
    private ImmutableArray<IBriggaglendrEventReceiver> _EventReceivers= ImmutableArray<IBriggaglendrEventReceiver>.Empty;

    public void AddEventReceiver(IBriggaglendrEventReceiver eventReceiver) {
        if (ReferenceEquals(eventReceiver, this)) { throw new ArgumentException("Cycle", nameof(eventReceiver)); }
        this._EventReceivers = this._EventReceivers.Add(eventReceiver);
    }

    public void RemoveEventReceiver(IBriggaglendrEventReceiver eventReceiver) {
        this._EventReceivers = this._EventReceivers.Remove(eventReceiver);
    }

    public void OnChangedConfiguration(BriggaglendrConfigurationChange change) {
        foreach (var eventReceiver in this._EventReceivers) { 
            eventReceiver.OnChangedConfiguration(change);
        }
    }

    public void OnChangedRoute(BGRouteConfigurationChange change) {
        foreach (var eventReceiver in this._EventReceivers) {
            eventReceiver.OnChangedRoute(change);
        }
    }

    public void OnChangedCluster(BGClusterConfigurationChange change) {
        foreach (var eventReceiver in this._EventReceivers) {
            eventReceiver.OnChangedCluster(change);
        }
    }

    public void OnChangedClusterDestination(BGClusterDestinationConfigurationChange change) {
        foreach (var eventReceiver in this._EventReceivers) {
            eventReceiver.OnChangedClusterDestination(change);
        }
    }

    public void OnChangedEndpoint(BGEndpointConfigurationChange change) {
        foreach (var eventReceiver in this._EventReceivers) {
            eventReceiver.OnChangedEndpoint(change);
        }
    }

    public void OnChangedTransport(BGTransportConfigurationChange change) {
        foreach (var eventReceiver in this._EventReceivers) {
            eventReceiver.OnChangedTransport(change);
        }
    }
}

public interface IBGConfigurationChange {
    void OnChanged(IBriggaglendrEventReceiver eventReceiver);
}

public abstract record BGConfigurationChange<T>(
    T? Before,
    T? Current,
    bool HasChanged
    ) : IBGConfigurationChange
    where T : class {
    public abstract void OnChanged(IBriggaglendrEventReceiver eventReceiver);
}