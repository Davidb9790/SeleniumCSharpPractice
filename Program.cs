using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Support.UI;

class Program
{
    // Configurable variables
    static string searchQuery = "Selenium C# Tutorial";
    static string urlBing = "https://www.bing.com";
    static int defaultWait = 10;

    static void Main()
    {
        // Setup ChromeDriver automatically
        new DriverManager().SetUpDriver(new ChromeConfig());
        var options = new ChromeOptions();
        options.AddExcludedArgument("enable-automation");
        options.LeaveBrowserRunning = true; // keeps browser open for inspection

        IWebDriver driver = new ChromeDriver(options);

        // Desktop path for screenshots
        string screenshotFolder = @"C:\Users\david\OneDrive\Desktop\Selenium";
        string screenshotPath = System.IO.Path.Combine(screenshotFolder, "screenshot.png");
        if (!System.IO.Directory.Exists(screenshotFolder))
        {
            System.IO.Directory.CreateDirectory(screenshotFolder);
        }

        try
        {
            NavigateTo(driver, urlBing);

            var searchBox = WaitForElement(driver, By.Name("q"), defaultWait);
            HandleRecaptcha(driver);

            TypeAndSubmit(searchBox, searchQuery);

            var firstResult = WaitForElement(driver, By.CssSelector("h3"), defaultWait);
            ClickElement(driver, firstResult);

            TakeScreenshot(driver, screenshotPath);

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

    static IWebElement WaitForElement(IWebDriver driver, By by, int waitSeconds)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitSeconds));
        return wait.Until(d => d.FindElement(by));
    }

    static void HandleRecaptcha(IWebDriver driver)
    {
        if (driver.FindElements(By.CssSelector("iframe[src*='recaptcha'], .g-recaptcha, #recaptcha")).Count > 0)
        {
            Console.WriteLine("reCAPTCHA detected. Please solve it manually in the opened browser, then press Enter...");
            Console.ReadLine();
        }
    }

    static void TypeAndSubmit(IWebElement element, string text)
    {
        Console.WriteLine($"Typing: {text}");
        element.Clear();
        element.SendKeys(text);
        element.Submit();
    }

    static void ClickElement(IWebDriver driver, IWebElement element)
    {
        Console.WriteLine("Clicking element...");
        try
        {
            element.Click();
        }
        catch (ElementClickInterceptedException)
        {
            Console.WriteLine("Click intercepted. Retrying with Enter key...");
            element.SendKeys(Keys.Enter);
        }
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
