using System.Net;

namespace Brimborium.Briggaglendr;

public sealed class BGConnectionListenerFactory
    : IConnectionListenerFactorySelector
    , IConnectionListenerFactory {
    private readonly BriggaglendrService _BriggaglendrService;

    public BGConnectionListenerFactory(
        BriggaglendrService briggaglendrService
        ) {
        this._BriggaglendrService = briggaglendrService;
    }

    public bool CanBind(EndPoint endpoint) {
        return false;
    }

    public ValueTask<IConnectionListener> BindAsync(
        EndPoint endpoint, 
        CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }

}