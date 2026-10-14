using Brimborium.Briggaglendr.Internal;

namespace Microsoft.AspNetCore.Builder;

public static class BriggaglendrBuilderExtensions {
    public static IApplicationBuilder UseBriggaglendr(
        this IApplicationBuilder applicationBuilder
        ) {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        var briggaglendrService =  VerifyBriggaglendrAreRegistered(applicationBuilder);

        BriggaglendrBuilder briggaglendrBuilder;
        if (applicationBuilder.Properties.TryGetValue(nameof(BriggaglendrBuilder), out var obj)
            && obj is { }) {
            briggaglendrBuilder = (BriggaglendrBuilder)obj;

            return applicationBuilder;
        } else {
            briggaglendrBuilder = new BriggaglendrBuilder(applicationBuilder, briggaglendrService);
            applicationBuilder.Properties[nameof(BriggaglendrBuilder)] = briggaglendrBuilder;
            return applicationBuilder.UseMiddleware<BGMiddleware>(briggaglendrBuilder);
        }
    }

    private static BriggaglendrService VerifyBriggaglendrAreRegistered(IApplicationBuilder builder) {
        if (builder.ApplicationServices.GetService(typeof(BriggaglendrService)) is not BriggaglendrService result) {
            throw new InvalidOperationException(
                $"Unable to find the required services. Please add all the required services by calling '{nameof(IServiceCollection)}.{nameof(BriggaglendrCollectionExtensions.AddBriggaglendr)}' in the application startup code."
                );
        }
        return result;
    }

    public static IEndpointRouteBuilder MapBriggaglendr(this IEndpointRouteBuilder endpointRouteBuilder) {
        var dataSource = endpointRouteBuilder.ServiceProvider.GetService<BGEndpointDataSource>();

        if (dataSource is null) {
            throw new Exception("Did you call AddBriggaglendr()?");
        }
        dataSource.Initialize();
        endpointRouteBuilder.DataSources.Add(dataSource);
        return endpointRouteBuilder;
    }
}
