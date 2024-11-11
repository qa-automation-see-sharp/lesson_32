using Microsoft.Playwright;

namespace Library.Test.Utils.Tests.Ui.PageObjects;

public class LinksPage : IBasePage
{
    public IPage? Page { get; set; }

    public string Url { get; } = "https://demoqa.com/links";

    public string ExpectedTitle { get; } = "Links";

    public ILocator Title => Page!.Locator("xpath=//h1[text()='Links']");
    public ILocator HomeLink => Page!.Locator("#simpleLink");
    public ILocator HomexUWGkLink => Page!.Locator("#dynamicLink");
    public ILocator CreatedLink => Page!.Locator("#created");
    public ILocator NoContentLink =>Page!.Locator("#no-content");
    public ILocator MovedLink => Page!.Locator("#moved");
    public ILocator BedRequestLink => Page!.Locator("#bad-request");
    public ILocator UnauthorizedLink => Page!.Locator("#unauthorized");
    public ILocator ForbiddenLink => Page!.Locator("#forbidden");
    public ILocator NotFoundLink => Page!.Locator("#invalid-url");
    public ILocator Links => Page!.Locator("#linkWrapper a");
    public ILocator Response => Page!.Locator("#linkResponse");

    public async Task Open()
    {
        await Page!.GotoAsync(Url);
    }
}
