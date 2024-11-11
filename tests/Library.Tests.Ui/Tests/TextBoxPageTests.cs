using Library.Test.Utils.Tests.Ui.Fixtures;
using Library.Test.Utils.Tests.Ui.PageObjects;
using static Library.Test.Utils.Tests.Ui.Fixtures.BrowserType;
using BrowserType = Microsoft.Playwright.BrowserType;

namespace Library.Tests.Ui.Tests;

[TestFixture]
public class TextBoxPageTests
{
    private readonly BrowserSetUpBuilder _browserSetUp = new();
    private MainPage Page { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Page = await _browserSetUp
            .WithBrowser(Chromium)
            .InHeadlessMode(true)
            .WithChannel("chrome")
            .WithSlowMo(100)
            .WithTimeout(10000)
            .WithArgs("--start-maximized")
            .OpenNewPage<MainPage>();
    }

    [Test]
    public async Task OpenTextBoxPage()
    {
        await Page.OpenAsync();
        await Page.Elements.ClickAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _browserSetUp.Page!.CloseAsync();
        await _browserSetUp.Context!.CloseAsync();
    }
}