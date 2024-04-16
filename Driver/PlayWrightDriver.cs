using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace talentsuite_regression_tests.Driver;

public class PlaywrightDriver : IDisposable
{
    private readonly Task<IAPIRequestContext?>? _requestContext;

    public PlaywrightDriver()
    {
        _requestContext = CreateApiContext();
    }

    public IAPIRequestContext? ApiRequestContext => _requestContext?.GetAwaiter().GetResult();

    public void Dispose()
    {
        _requestContext?.Dispose();
    }


    private async Task<IAPIRequestContext?> CreateApiContext()
    {
        var playwright = await Playwright.CreateAsync();

        return await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = "https://localhost:7289/",
            IgnoreHTTPSErrors = true
        });
    }
}