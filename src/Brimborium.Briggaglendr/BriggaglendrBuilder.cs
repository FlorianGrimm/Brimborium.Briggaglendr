namespace Brimborium.Briggaglendr;

public class BriggaglendrBuilder {
    private IApplicationBuilder _Builder;

    public BriggaglendrBuilder(IApplicationBuilder builder, BriggaglendrService briggaglendrService) {
        this._Builder = builder;
    }
}