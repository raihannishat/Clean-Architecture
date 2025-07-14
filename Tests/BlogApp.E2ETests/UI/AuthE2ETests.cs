using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using FluentAssertions;
using Xunit;

namespace BlogApp.E2ETests.UI;

public class AuthE2ETests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private const string BaseUrl = "http://localhost:4200";

    public AuthE2ETests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        
        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void LoginPage_ShouldDisplayLoginForm()
    {
        // Arrange & Act
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/login");

        // Assert
        _wait.Until(d => d.FindElement(By.Id("email"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("password"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("login-button"))).Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_ShouldDisplayRegistrationForm()
    {
        // Arrange & Act
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/register");

        // Assert
        _wait.Until(d => d.FindElement(By.Id("firstName"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("lastName"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("email"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("userName"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("password"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("confirmPassword"))).Should().NotBeNull();
        _wait.Until(d => d.FindElement(By.Id("register-button"))).Should().NotBeNull();
    }

    [Fact]
    public void Login_WithValidCredentials_ShouldRedirectToDashboard()
    {
        // Arrange
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/login");
        var emailInput = _wait.Until(d => d.FindElement(By.Id("email")));
        var passwordInput = _driver.FindElement(By.Id("password"));
        var loginButton = _driver.FindElement(By.Id("login-button"));

        // Act
        emailInput.SendKeys("test@example.com");
        passwordInput.SendKeys("Password123!");
        loginButton.Click();

        // Assert
        _wait.Until(d => d.Url.Contains("/dashboard") || d.Url.Contains("/blog"));
    }

    [Fact]
    public void Login_WithInvalidCredentials_ShouldShowError()
    {
        // Arrange
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/login");
        var emailInput = _wait.Until(d => d.FindElement(By.Id("email")));
        var passwordInput = _driver.FindElement(By.Id("password"));
        var loginButton = _driver.FindElement(By.Id("login-button"));

        // Act
        emailInput.SendKeys("invalid@example.com");
        passwordInput.SendKeys("WrongPassword123!");
        loginButton.Click();

        // Assert
        _wait.Until(d => d.FindElement(By.ClassName("error-message"))).Should().NotBeNull();
    }

    [Fact]
    public void Register_WithValidData_ShouldCreateAccount()
    {
        // Arrange
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/register");
        var firstNameInput = _wait.Until(d => d.FindElement(By.Id("firstName")));
        var lastNameInput = _driver.FindElement(By.Id("lastName"));
        var emailInput = _driver.FindElement(By.Id("email"));
        var userNameInput = _driver.FindElement(By.Id("userName"));
        var passwordInput = _driver.FindElement(By.Id("password"));
        var confirmPasswordInput = _driver.FindElement(By.Id("confirmPassword"));
        var registerButton = _driver.FindElement(By.Id("register-button"));

        var uniqueEmail = $"test{DateTime.Now.Ticks}@example.com";
        var uniqueUserName = $"user{DateTime.Now.Ticks}";

        // Act
        firstNameInput.SendKeys("John");
        lastNameInput.SendKeys("Doe");
        emailInput.SendKeys(uniqueEmail);
        userNameInput.SendKeys(uniqueUserName);
        passwordInput.SendKeys("Password123!");
        confirmPasswordInput.SendKeys("Password123!");
        registerButton.Click();

        // Assert
        _wait.Until(d => d.Url.Contains("/auth/login") || d.FindElement(By.ClassName("success-message"))).Should().NotBeNull();
    }

    [Fact]
    public void Register_WithMismatchedPasswords_ShouldShowError()
    {
        // Arrange
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/register");
        var firstNameInput = _wait.Until(d => d.FindElement(By.Id("firstName")));
        var lastNameInput = _driver.FindElement(By.Id("lastName"));
        var emailInput = _driver.FindElement(By.Id("email"));
        var userNameInput = _driver.FindElement(By.Id("userName"));
        var passwordInput = _driver.FindElement(By.Id("password"));
        var confirmPasswordInput = _driver.FindElement(By.Id("confirmPassword"));
        var registerButton = _driver.FindElement(By.Id("register-button"));

        // Act
        firstNameInput.SendKeys("John");
        lastNameInput.SendKeys("Doe");
        emailInput.SendKeys("test@example.com");
        userNameInput.SendKeys("testuser");
        passwordInput.SendKeys("Password123!");
        confirmPasswordInput.SendKeys("DifferentPassword123!");
        registerButton.Click();

        // Assert
        _wait.Until(d => d.FindElement(By.ClassName("error-message"))).Should().NotBeNull();
    }

    [Fact]
    public void Navigation_FromLoginToRegister_ShouldWork()
    {
        // Arrange
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/login");
        var registerLink = _wait.Until(d => d.FindElement(By.LinkText("Register")));

        // Act
        registerLink.Click();

        // Assert
        _wait.Until(d => d.Url.Contains("/auth/register"));
    }

    [Fact]
    public void Navigation_FromRegisterToLogin_ShouldWork()
    {
        // Arrange
        _driver.Navigate().GoToUrl($"{BaseUrl}/auth/register");
        var loginLink = _wait.Until(d => d.FindElement(By.LinkText("Login")));

        // Act
        loginLink.Click();

        // Assert
        _wait.Until(d => d.Url.Contains("/auth/login"));
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
} 