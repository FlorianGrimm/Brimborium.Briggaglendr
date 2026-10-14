namespace Brimborium.Briggaglendr.Internal;

public class BGEndpointDataSource : EndpointDataSource {
    private readonly BriggaglendrService _BriggaglendrService;
    private List<Endpoint> _Endpoints = new();
    private ReadOnlyCollection<Endpoint> _EndpointsAsReadOnly;
    private CancellationTokenSource _CancellationTokenSource;
    private CancellationChangeToken _ChangeToken;

    public BGEndpointDataSource(
        BriggaglendrService briggaglendrService
        ) {
        this._EndpointsAsReadOnly = this._Endpoints.AsReadOnly();
        this._BriggaglendrService = briggaglendrService;
        this._CancellationTokenSource = new CancellationTokenSource();
        this._ChangeToken = new CancellationChangeToken(this._CancellationTokenSource.Token);
    }

    public void Initialize() {
        this._BriggaglendrService.Initialize();
        //https://www.mariusgundersen.net/article/2021-dynamic-endpoint-routing/
        foreach (var (key, endpoint) in this._BriggaglendrService.Configuration.Endpoint) {
            //endpoint.Pattern
        }
    }

    private static Endpoint CreateEndpoint(
        RequestDelegate requestDelegate,
        string pattern,
        int order = 0
        ) =>
       new RouteEndpointBuilder(
           requestDelegate: requestDelegate,
           routePattern: RoutePatternFactory.Parse(pattern),
           order: order)
       .Build();

    public override IReadOnlyList<Endpoint> Endpoints => this._EndpointsAsReadOnly;

    public override IChangeToken GetChangeToken() => this._ChangeToken;

    public void AddEndpoint(Endpoint endpoint) {
        lock (this) {
            var endpoints = new List<Endpoint>(this._Endpoints);
            endpoints.Add(endpoint);

            var endpointsAsReadOnly = this._Endpoints.AsReadOnly();
            this._Endpoints = endpoints;
            this._EndpointsAsReadOnly = endpointsAsReadOnly;

            var oldCancellationTokenSource = this._CancellationTokenSource;
            this._CancellationTokenSource = new CancellationTokenSource();
            this._ChangeToken = new CancellationChangeToken(this._CancellationTokenSource.Token);
            oldCancellationTokenSource?.Cancel();
        }
    }

    public void Change(List<Endpoint> listEndpoints) {
        lock (this) {
            var endpoints = new List<Endpoint>(listEndpoints);
            var endpointsAsReadOnly = this._Endpoints.AsReadOnly();

            this._Endpoints = endpoints;
            this._EndpointsAsReadOnly = endpointsAsReadOnly;

            var oldCancellationTokenSource = this._CancellationTokenSource;
            this._CancellationTokenSource = new CancellationTokenSource();
            oldCancellationTokenSource?.Cancel();
        }
    }
}