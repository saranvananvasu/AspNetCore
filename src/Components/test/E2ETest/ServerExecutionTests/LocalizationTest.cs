// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BasicTestApp;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.E2ETesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Components.E2ETest.ServerExecutionTests
{
    // For now this is limited to server-side execution because we don't have the ability to set the
    // culture in client-side Blazor.
    public class LocalizationTest : ServerTestBase<AspNetSiteServerFixture>
    {
        public LocalizationTest(
            BrowserFixture browserFixture,
            AspNetSiteServerFixture serverFixture,
            ITestOutputHelper output)
            : base(browserFixture, serverFixture, output)
        {
            serverFixture.BuildWebHostMethod = TestServer.Program.BuildWebHost<InternationalizationStartup>;
        }

        protected override void InitializeAsyncCore()
        {
            // On WebAssembly, page reloads are expensive so skip if possible
            Navigate(ServerPathBase);
            MountTestComponent<CulturePicker>();
            WaitUntilExists(By.Id("culture-selector"));
        }

        [Theory]
        [InlineData("en-US", "Hello!")]
        [InlineData("fr-FR", "Bonjour!")]
        public void CanSetCultureAndReadLocalizedResources(string culture, string message)
        {
            var selector = new SelectElement(Browser.FindElement(By.Id("culture-selector")));
            selector.SelectByValue(culture);

            // That should have triggered a redirect, wait for the main test selector to come up.
            MountTestComponent<LocalizedText>();

            var cultureDisplay = WaitUntilExists(By.Id("culture-name-display"));
            Assert.Equal($"Culture is: {culture}", cultureDisplay.Text);

            var messageDisplay = Browser.FindElement(By.Id("message-display"));
            Assert.Equal(message, messageDisplay.Text);
        }
    }
}
