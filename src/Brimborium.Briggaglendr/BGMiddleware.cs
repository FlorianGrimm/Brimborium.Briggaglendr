namespace Brimborium.Briggaglendr.Internal;

public sealed class BGMiddleware {
    private readonly RequestDelegate _Next;
    private readonly BriggaglendrBuilder _BriggaglendrBuilder;

    public BGMiddleware(
        RequestDelegate next,
        BriggaglendrBuilder briggaglendrBuilder
        ) {
        this._Next = next;
        this._BriggaglendrBuilder = briggaglendrBuilder;
    }

    public Task Invoke(HttpContext context) {
        return this._Next(context);
    }
}
