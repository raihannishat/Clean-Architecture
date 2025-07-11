using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.Json;
using Xunit;

namespace BlogApp.E2ETests.UI;

public class BlogAppE2ETests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://localhost:7001";
    private const string AngularBaseUrl = "http://localhost:4200";

    public BlogAppE2ETests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        
        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        _httpClient = new HttpClient();
    }

    [Fact]
    public async Task UserCanLoginAndAccessBlogFeatures()
    {
        // Arrange - Login via API first
        var loginData = new
        {
            operation = "LoginCommand",
            data = new
            {
                email = "admin@blogapp.com",
                password = "Admin123!"
            }
        };

        var loginJson = JsonSerializer.Serialize(loginData);
        var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");
        
        var loginResponse = await _httpClient.PostAsync($"{ApiBaseUrl}/api/dispatcher", loginContent);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadAsStringAsync();
        var loginResponseObj = JsonSerializer.Deserialize<ApiResponse>(loginResult, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act - Navigate to Angular app
        _driver.Navigate().GoToUrl(AngularBaseUrl);

        // Wait for the app to load
        _wait.Until(d => d.FindElement(By.TagName("app-root")));

        // Verify the app loaded
        var appRoot = _driver.FindElement(By.TagName("app-root"));
        appRoot.Should().NotBeNull();

        // Check if login form is present
        var loginForm = _driver.FindElement(By.CssSelector("form"));
        loginForm.Should().NotBeNull();

        // Find email and password fields
        var emailField = _driver.FindElement(By.CssSelector("input[type='email']"));
        var passwordField = _driver.FindElement(By.CssSelector("input[type='password']"));

        // Enter credentials
        emailField.Clear();
        emailField.SendKeys("admin@blogapp.com");

        passwordField.Clear();
        passwordField.SendKeys("Admin123!");

        // Submit the form
        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        submitButton.Click();

        // Wait for navigation or success message
        _wait.Until(d => d.Url != AngularBaseUrl || d.FindElement(By.CssSelector(".success-message")).Displayed);

        // Assert - Verify successful login
        _driver.Url.Should().NotBe(AngularBaseUrl);
    }

    [Fact]
    public async Task UserCanViewBlogPosts()
    {
        // Arrange - Get blog posts via API
        var postsData = new
        {
            operation = "GetBlogPostsQuery",
            data = new
            {
                page = 1,
                pageSize = 10
            }
        };

        var postsJson = JsonSerializer.Serialize(postsData);
        var postsContent = new StringContent(postsJson, System.Text.Encoding.UTF8, "application/json");
        
        var postsResponse = await _httpClient.PostAsync($"{ApiBaseUrl}/api/dispatcher", postsContent);
        postsResponse.EnsureSuccessStatusCode();

        var postsResult = await postsResponse.Content.ReadAsStringAsync();
        var postsResponseObj = JsonSerializer.Deserialize<ApiResponse>(postsResult, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act - Navigate to blog list page
        _driver.Navigate().GoToUrl($"{AngularBaseUrl}/blog");

        // Wait for the page to load
        _wait.Until(d => d.FindElement(By.CssSelector(".blog-list")));

        // Assert - Verify blog posts are displayed
        var blogPosts = _driver.FindElements(By.CssSelector(".blog-post"));
        blogPosts.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UserCanCreateNewBlogPost()
    {
        // Arrange - Login first
        await LoginUser();

        // Act - Navigate to create blog post page
        _driver.Navigate().GoToUrl($"{AngularBaseUrl}/blog/create");

        // Wait for the form to load
        _wait.Until(d => d.FindElement(By.CssSelector(".blog-create-form")));

        // Fill in the form
        var titleField = _driver.FindElement(By.CssSelector("input[name='title']"));
        var contentField = _driver.FindElement(By.CssSelector("textarea[name='content']"));
        var summaryField = _driver.FindElement(By.CssSelector("input[name='summary']"));

        var testTitle = $"Test Blog Post {DateTime.Now:yyyyMMddHHmmss}";
        
        titleField.Clear();
        titleField.SendKeys(testTitle);

        contentField.Clear();
        contentField.SendKeys("This is a test blog post content created via E2E test.");

        summaryField.Clear();
        summaryField.SendKeys("Test summary for E2E test");

        // Submit the form
        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        submitButton.Click();

        // Wait for success message or redirect
        _wait.Until(d => d.FindElement(By.CssSelector(".success-message")).Displayed || d.Url.Contains("/blog"));

        // Assert - Verify success
        var successMessage = _driver.FindElement(By.CssSelector(".success-message"));
        successMessage.Text.Should().Contain("success");
    }

    [Fact]
    public async Task UserCanViewCategories()
    {
        // Arrange - Get categories via API
        var categoriesData = new
        {
            operation = "GetCategoriesQuery",
            data = new { }
        };

        var categoriesJson = JsonSerializer.Serialize(categoriesData);
        var categoriesContent = new StringContent(categoriesJson, System.Text.Encoding.UTF8, "application/json");
        
        var categoriesResponse = await _httpClient.PostAsync($"{ApiBaseUrl}/api/dispatcher", categoriesContent);
        categoriesResponse.EnsureSuccessStatusCode();

        var categoriesResult = await categoriesResponse.Content.ReadAsStringAsync();
        var categoriesResponseObj = JsonSerializer.Deserialize<ApiResponse>(categoriesResult, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act - Navigate to categories page (if exists) or check sidebar
        _driver.Navigate().GoToUrl(AngularBaseUrl);

        // Wait for the page to load
        _wait.Until(d => d.FindElement(By.TagName("app-root")));

        // Look for categories in the UI
        var categories = _driver.FindElements(By.CssSelector(".category-item, .sidebar-category"));
        
        // Assert - Verify categories are displayed
        categories.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ApiEndpointsAreAccessible()
    {
        // Test various API endpoints
        var endpoints = new object[]
        {
            new { Operation = "GetBlogPostsQuery", Data = new { page = 1, pageSize = 5 } },
            new { Operation = "GetCategoriesQuery", Data = new { } },
            new { Operation = "GetTagsQuery", Data = new { } }
        };

        foreach (dynamic endpoint in endpoints)
        {
            var requestData = new
            {
                operation = endpoint.Operation,
                data = endpoint.Data
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{ApiBaseUrl}/api/dispatcher", content);
            
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            
            var result = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<ApiResponse>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            responseObj.Should().NotBeNull();
            responseObj!.IsSuccess.Should().BeTrue();
        }
    }

    private async Task LoginUser()
    {
        var loginData = new
        {
            operation = "LoginCommand",
            data = new
            {
                email = "admin@blogapp.com",
                password = "Admin123!"
            }
        };

        var loginJson = JsonSerializer.Serialize(loginData);
        var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");
        
        var loginResponse = await _httpClient.PostAsync($"{ApiBaseUrl}/api/dispatcher", loginContent);
        loginResponse.EnsureSuccessStatusCode();
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
        _httpClient?.Dispose();
    }

    private class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }
    }
} 