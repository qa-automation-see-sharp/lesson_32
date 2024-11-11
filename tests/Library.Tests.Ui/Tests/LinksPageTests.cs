using Library.Test.Utils.Tests.Ui.Fixtures;
using Library.Test.Utils.Tests.Ui.PageObjects;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using static Library.Test.Utils.Tests.Ui.Fixtures.BrowserType;

namespace Library.Tests.Ui.Tests;

//TODO: cover with tests
[TestFixture]
public class LinksPageTests : PageTest
{
    private readonly BrowserSetUpBuilder _browserSetUp = new();
    private LinksPage? linksPage { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        linksPage = await _browserSetUp
            .WithBrowser(Chromium)
            .WithChannel("chrome")
            .InHeadlessMode(true)
            .WithSlowMo(100)
            .WithTimeout(10000)
            .WithViewportSize(1900, 1080)
            .WithVideoSize(1900, 1080)
            .SaveVideo("videos/")
            .WithArgs("--start-maximized")
            .OpenNewPage<LinksPage>();
        _browserSetUp.AddRequestResponseLogger();
        await linksPage!.Open();
    }

    [SetUp]
    public async Task SetUp()
    {
        var traceName = TestContext.CurrentContext.Test.ClassName + "/" + TestContext.CurrentContext.Test.Name;
        await _browserSetUp.StartTracing(traceName);
    }

    [Test]
    public async Task OpenLinksPage()
    {
        var title = await linksPage!.Title.TextContentAsync();

        Assert.That(title, Is.EqualTo(linksPage!.ExpectedTitle));
    }

    [Test]
    public async Task LinksNotBeEmpty()
    {
        var linksList = linksPage!.Links;
        var linksCount = await linksList.CountAsync();
        List<string> links = new();
        for (var i = 0; i < linksCount; i++)
        {
            var url = await linksList.Nth(i).GetAttributeAsync("href") ?? string.Empty;
            links.Add(url);
        }
        Assert.Multiple(() =>
        {
            Assert.That(linksCount, Is.EqualTo(links.Count));
        });
    }
    
    [Test]
    public async Task ClickOnHomeLink(){
        
        await linksPage!.HomeLink.ClickAsync();
        await linksPage!.Page!.WaitForLoadStateAsync();
        var url = linksPage.Page.Url;

        Assert.That(url, Is.EqualTo(linksPage.Url));
        
    } 

    [Test]
    public async Task CheckResponseLink()
    {
        await linksPage.CreatedLink.ClickAsync();
        await Expect(linksPage.Response).ToHaveTextAsync("Link has responded with staus 201 and status text Created");
        await linksPage.NoContentLink.ClickAsync();
        await Expect(linksPage.Response).ToHaveTextAsync("Link has responded with staus 204 and status text No Content");
        await linksPage.NotFoundLink.ClickAsync();
        await Expect(linksPage.Response).ToHaveTextAsync("Link has responded with staus 404 and status text Not Found");
    }


    [TearDown]
    public async Task TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            await _browserSetUp.Screenshot(
                TestContext.CurrentContext.Test.ClassName,
                TestContext.CurrentContext.Test.Name);
        }

        var tracePAth = Path.Combine(
            "playwright-traces",
            $"{TestContext.CurrentContext.Test.ClassName}",
            $"{TestContext.CurrentContext.Test.Name}.zip");
        await _browserSetUp.StopTracing(tracePAth);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _browserSetUp.Page!.CloseAsync();
        await _browserSetUp.Context!.CloseAsync();
    }
}