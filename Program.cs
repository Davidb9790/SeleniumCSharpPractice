using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

class Program
{
    // Configurable variables
    static string searchQuery = "Selenium (software)";
    static string urlWikipedia = "https://en.wikipedia.org";
    static int defaultWait = 20; // longer wait for dynamic pages

    static void Main()
    {
        // Setup ChromeDriver automatically
        new DriverManager().SetUpDriver(new ChromeConfig());
        var options = new ChromeOptions();
        options.AddExcludedArgument("enable-automation");
        options.LeaveBrowserRunning = true; // keeps browser open

        IWebDriver driver = new ChromeDriver(options);

        // Maximize window to avoid interactable issues
        driver.Manage().Window.Maximize();

        // Screenshot path
        string screenshotFolder = @"C:\Users\david\Desktop\Selenium";
        string screenshotPath = System.IO.Path.Combine(screenshotFolder, "screenshot.png");
        if (!System.IO.Directory.Exists(screenshotFolder))
        {
            System.IO.Directory.CreateDirectory(screenshotFolder);
        }

        try
        {
            // Navigate to Wikipedia
            NavigateTo(driver, urlWikipedia);

            // Wait for search box to be clickable
            var searchBox = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultWait))
                .Until(ExpectedConditions.ElementToBeClickable(By.Name("search")));

            // Scroll and click to focus
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", searchBox);
            searchBox.Click();

            // Type query
            searchBox.Clear();
            searchBox.SendKeys(searchQuery);

            // Submit the search form using JavaScript
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].closest('form').submit();", searchBox);

            // Wait for the first heading on the results page to ensure page loaded
            var firstHeading = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultWait))
                .Until(ExpectedConditions.ElementIsVisible(By.Id("firstHeading")));

            // Take screenshot
            TakeScreenshot(driver, screenshotPath);

            // Print page info
            PrintPageInfo(driver);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("Automation finished. Press Enter to close browser...");
            Console.ReadLine();
            driver.Quit();
        }
    }

    // ---------------- Helper Methods ----------------

    static void NavigateTo(IWebDriver driver, string url)
    {
        Console.WriteLine($"Navigating to {url}...");
        driver.Navigate().GoToUrl(url);
        Console.WriteLine("Page title: " + driver.Title);
    }

    static void TakeScreenshot(IWebDriver driver, string path)
    {
        Console.WriteLine($"Taking screenshot: {path}");
        ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(path);
    }

    static void PrintPageInfo(IWebDriver driver)
    {
        Console.WriteLine("Page title after click: " + driver.Title);
        Console.WriteLine("Page URL: " + driver.Url);
    }
}
