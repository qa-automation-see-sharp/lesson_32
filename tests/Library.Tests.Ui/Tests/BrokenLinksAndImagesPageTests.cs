using Library.Test.Utils.Tests.Ui.Fixtures;
using Library.Test.Utils.Tests.Ui.PageObjects;
using NUnit.Framework.Interfaces;
using static Library.Test.Utils.Tests.Ui.Fixtures.BrowserType;
using BrowserType = Microsoft.Playwright.BrowserType;

namespace Library.Tests.Ui.Tests;

//TODO: cover with tests
[TestFixture]
public class BrokenLinksAndImagesPageTests
{
    private readonly BrowserSetUpBuilder _browserSetUp = new();
    private BrokenPage? Page { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Page = await _browserSetUp
            .WithBrowser(Chromium)
            .WithChannel("chrome")
            .InHeadlessMode(true)
            .WithSlowMo(100)
            .WithTimeout(10000)
            .WithViewportSize(1900, 1080)
            .WithVideoSize(1900, 1080)
            .SaveVideo("videos/")
            .WithArgs("--start-maximized")
            .OpenNewPage<BrokenPage>();
    }

    [SetUp]
    public async Task SetUp()
    {
        _browserSetUp.AddRequestResponseLogger();
        await Page!.Open();

        var traceName = TestContext.CurrentContext.Test.ClassName + "/" + TestContext.CurrentContext.Test.Name;
        await _browserSetUp.StartTracing(traceName);
    }

    [Test]
    public async Task OpenBrokenPage()
    {
        var title = await Page!.Title.TextContentAsync();

        Assert.That(title, Is.EqualTo(Page!.ExpectedTitle));
    }

    [Test]
    public async Task VisibleElementsOnPage()
    {
        var imgAndLinksVisible = await Page.ImgsAndLinksVisible();
        var textVisible = await Page.TextForElementsVisible();

        Assert.Multiple(() =>
        {
            Assert.That(imgAndLinksVisible, Is.True);
            Assert.That(textVisible, Is.True);
        });
    }

    [Test]
    public async Task ClickOnValidLink()
    {
        await Page.ValidLink.ClickAsync();
        Assert.That(_browserSetUp.Page.Url, Is.EqualTo("https://demoqa.com/"));
    }

    [Test]
    public async Task ClickBrokenLink()
    {
        await Page.BrokenLink.ClickAsync();
        Assert.That(Page.Page.Url, Is.EqualTo("http://the-internet.herokuapp.com/status_codes/500"));
        await Page.Page.GoBackAsync();
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