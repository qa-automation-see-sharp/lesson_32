using Library.Test.Utils.Tests.Ui.Fixtures;
using Library.Test.Utils.Tests.Ui.PageObjects;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using static Library.Test.Utils.Tests.Ui.Fixtures.BrowserType;

namespace Library.Tests.Ui.Tests;

//TODO: cover with tests
[TestFixture]
public class DynamicPropertiesPageTests : PageTest
{
    private readonly BrowserSetUpBuilder _browserSetUp = new();
    private DynamicPropertiesPage? Page { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Page = await _browserSetUp
            .WithBrowser(Chromium)
            .WithChannel("chrome")
            .InHeadlessMode(true)
            .WithSlowMo(100)
            .WithTimeout(10000)
            .WithVideoSize(1900, 1080)
            .SaveVideo("videos/")
            .WithArgs("--start-maximized")
            .OpenNewPage<DynamicPropertiesPage>();
            _browserSetUp.AddRequestResponseLogger();
        await Page!.Open();
    }

    [SetUp]
    public async Task SetUp()
    {
        var traceName = TestContext.CurrentContext.Test.ClassName + "/" + TestContext.CurrentContext.Test.Name;
        await _browserSetUp.StartTracing(traceName);
        await _browserSetUp.Page.ReloadAsync();
    }

    [Test]
    public async Task OpenDynamicPropertiesPage()
    {
        var title = await Page!.Title.TextContentAsync();

        Assert.That(title, Is.EqualTo(Page!.ExpectedTitle));
    }

    [Test]
    public async Task CheckEnabledButton()
    {
        await Expect(Page.EnableButton).ToBeDisabledAsync(new() { Timeout = 4900 });
        await Expect(Page.EnableButton).ToBeEnabledAsync(); // Time to retry the assertion for in milliseconds. Defaults to 5000.
    }

     [Test]
    public async Task CheckColorChangeButton()
    {
        await Expect(Page!.ColorButton).ToHaveCSSAsync("color", "rgb(255, 255, 255)");
        await Expect(Page!.ColorButton).ToHaveCSSAsync("color", "rgb(220, 53, 69)");
    }

    [Test]
    public async Task CheckAppearingButton()
    {
        await Expect(Page!.VisibleAfterButton).Not.ToBeVisibleAsync(new() {Timeout = 4900});
        await Expect(Page!.VisibleAfterButton).ToBeVisibleAsync();
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