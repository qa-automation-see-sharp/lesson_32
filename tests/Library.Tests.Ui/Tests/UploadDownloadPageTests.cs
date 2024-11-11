using Library.Test.Utils.Tests.Ui.Fixtures;
using Library.Test.Utils.Tests.Ui.PageObjects;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using static Library.Test.Utils.Tests.Ui.Fixtures.BrowserType;

namespace Library.Tests.Ui.Tests;

//TODO: cover with tests
[TestFixture]
public class UploadDownloadPageTests : PageTest
{
    private readonly BrowserSetUpBuilder _browserSetUp = new();
    private UploadDownloadPage? Page { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Page = await _browserSetUp
            .WithBrowser(Chromium)
            .WithChannel("chrome")
            .InHeadlessMode(false)
            .WithSlowMo(100)
            .WithTimeout(10000)
            .WithVideoSize(1900, 1080)
            .SaveVideo("videos/")
            .WithArgs("--start-maximized")
            .OpenNewPage<UploadDownloadPage>();
            _browserSetUp.AddRequestResponseLogger();
        await Page!.Open();
    }

    [SetUp]
    public async Task SetUp()
    {
        var traceName = TestContext.CurrentContext.Test.ClassName + "/" + TestContext.CurrentContext.Test.Name;
        await _browserSetUp.StartTracing(traceName);
    }

    [Test]
    public async Task OpenPage()
    {
        var title = await Page!.Title.TextContentAsync();

        Assert.That(title, Is.EqualTo(Page!.ExpectedTitle));
    }

    [Test]
    public async Task CheckDownLoad()
    {
        var download = await _browserSetUp.Page.RunAndWaitForDownloadAsync(async () =>
        {
            await Page.DownloadButton.ClickAsync();
        });

        Assert.That(download, Is.Not.Null);

        var path = await download.PathAsync();
        Assert.That(File.Exists(path), Is.True);
    }

    [Test]
    public async Task CheckUpload()
    {   
        string sCurrentDirectory = Directory.GetCurrentDirectory();
        string sFile = Path.Combine(sCurrentDirectory, @"sampleFile.jpeg");  
        string sFilePath = Path.GetFullPath(sFile); 
        await Page!.UploadButton.SetInputFilesAsync(sFilePath);
        
        var uploadedFilePath = await Page.FilePath.InnerTextAsync();
        var hardcodedPath = "C:\\fakepath\\sampleFile.jpeg";
        Assert.That(uploadedFilePath, Is.EqualTo(hardcodedPath));
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