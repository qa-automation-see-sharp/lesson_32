using Microsoft.Playwright;

namespace Library.Test.Utils.Tests.Ui.PageObjects
{
    public class BrokenPage : IBasePage
    {
        public IPage? Page { get; set; }
        public string Url { get; } = "https://demoqa.com/broken";
        public string ExpectedTitle { get; } = "Broken Links - Images";

        public ILocator Title => Page!.Locator("//h1[text()='Broken Links - Images']");
        public ILocator ValidImageText => Page!.Locator("//p[text()= 'Valid image']");
        public ILocator BrokenImageText => Page!.Locator("//p[text()= 'Broken image']");
        public ILocator ValidLinkText => Page!.Locator("//p[text()= 'Valid Link']");
        public ILocator BrokenLinkText => Page!.Locator("//p[text()= 'Broken Link']");
        public ILocator ValidImage => Page!.Locator("p~img[src='/images/Toolsqa.jpg']");
        public ILocator BrokenImage => Page!.Locator("p~img[src='/images/Toolsqa_1.jpg']");
        public ILocator ValidLink => Page!.GetByRole(AriaRole.Link, new() { Name = "Click Here for Valid Link" });
        public ILocator BrokenLink => Page!.GetByRole(AriaRole.Link, new() { Name = "Click Here for Broken Link" });

        public async Task<BrokenPage> Open()
        {
            await Page!.GotoAsync(Url);
            return this;
        }

        public async Task<bool> TextForElementsVisible()
        {
            var validImageText = await ValidImageText.IsVisibleAsync();
            var brokenImageText = await BrokenImageText.IsVisibleAsync();
            var validLinkText = await ValidLinkText.IsVisibleAsync();
            var brokenLinkText = await BrokenLinkText.IsVisibleAsync();

            return validImageText && brokenImageText && validLinkText && brokenLinkText;
        }

        public async Task<bool> ImgsAndLinksVisible()
        {
            return
                await ValidImage.IsVisibleAsync() &&
                await BrokenImage.IsVisibleAsync() &&
                await ValidLink.IsVisibleAsync() &&
                await BrokenLink.IsVisibleAsync();
        }
    }
}