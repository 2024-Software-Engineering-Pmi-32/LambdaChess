using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using LambdaChess.Helpers;
using LambdaChess.Models;

namespace LambdaChess;

public partial class LoginWindow : Window {
    private readonly ApplicationContext _dbContext;
    private readonly string _loginDataFilePath = GetLoginDataFilePath();
    private readonly NotificationHelper _notificationHelper = new();
    
    public LoginWindow() {
        InitializeComponent();
        _dbContext = new ApplicationContext();
        Icon = new WindowIcon(GetIconFilePath());
        LoadUserData();
        // Attach the event to all input fields
        EmailBox.TextChanged += OnInputFieldsChanged;
        UsernameBox.TextChanged += OnInputFieldsChanged;
        PasswordBox.TextChanged += OnInputFieldsChanged;
        
        UserAgreementCheckBox.IsCheckedChanged += OnAgreementChecked;
        PrivacyPolicyCheckBox.IsCheckedChanged += OnAgreementChecked;
        // Initially check the button states
        UpdateButtonStates();
    }

    private void OnInputFieldsChanged(object? sender, TextChangedEventArgs e) => UpdateButtonStates();
    private void OnAgreementChecked(object? sender, RoutedEventArgs e) => UpdateButtonStates();
    
    private void OnPasswordInput(object? sender, TextChangedEventArgs e) {
        var passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{9,}$";
        var currentPassword = PasswordBox.Text;
        
        if (currentPassword == null || currentPassword.Length < 9) {
            FlyoutMessage.Text = "Password must be at least 9 characters long";
        }
        else if (!Regex.IsMatch(currentPassword, passwordPattern)) {
            FlyoutMessage.Text = "Password must contain letters and numbers";
        }
        else {
            FlyoutMessage.Text = "Password seems good to me!";
        }
        FlyoutBase.ShowAttachedFlyout(PasswordBox);
    }
    
    private void OnUserAgreementLinkClick(object? sender, TappedEventArgs e) {
        Process.Start(new ProcessStartInfo {
            FileName = "https://github.com/2024-Software-Engineering-Pmi-32/LambdaChess-docs/blob/main/LICENSE",
            UseShellExecute = true
        });
    }

    private void OnPrivacyPolicyLinkClick(object? sender, TappedEventArgs e) {
        Process.Start(new ProcessStartInfo {
            FileName = "https://github.com/2024-Software-Engineering-Pmi-32/LambdaChess-docs/blob/main/LICENSE",
            UseShellExecute = true
        });
    }

    private void UpdateButtonStates() {
        // Check if any field is empty and if agreements are checked
        var isAnyFieldEmpty = string.IsNullOrWhiteSpace(EmailBox.Text) ||
                              string.IsNullOrWhiteSpace(UsernameBox.Text) ||
                              string.IsNullOrWhiteSpace(PasswordBox.Text);

        var isAgreementsChecked = UserAgreementCheckBox.IsChecked == true && 
                                  PrivacyPolicyCheckBox.IsChecked == true;

        // Enable or disable buttons based on conditions
        LoginButton.IsEnabled = !isAnyFieldEmpty && isAgreementsChecked;
        RegisterButton.IsEnabled = !isAnyFieldEmpty && isAgreementsChecked;
    }
    
    private async void LoadUserData() {
        // Try load cache from file.
        var userData = (UserData?)EncryptionHelper.LoadUserData<UserData>(_loginDataFilePath);
        if (userData == null) {
            // No cached data
            return;
        }

        UsernameBox.Text = userData.UserName;
        EmailBox.Text = userData.UserEmail;
        PasswordBox.Text = userData.Password;
        
        // Verify cached data integrity
        
        if (!IsValidEmail(userData.UserEmail).Result) {
            ShowMessage("Saved email is no longer valid!");
            return;
        }
        
        var playerData = _dbContext.Players.FirstOrDefault(p => p.email == userData.UserEmail);
        // Auto login if data is valid
        if (playerData != null && VerifyPassword(userData.Password, playerData.password_hash)) {
            // Small delay to let pages load
            await Task.Delay(500); 
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        else {
            ShowMessage("Error occured while trying to login. Please try latter");
            Console.WriteLine("Error occured while trying to login. Please try latter");
        }
    }

    private void OnLoginButtonClick(object sender, RoutedEventArgs e) {
        var username = UsernameBox.Text;
        var password = PasswordBox.Text;
        var email = EmailBox.Text;
        
        // If any of field is empty
        if (username == null || password == null || email == null) {
            ShowMessage("Please fill all the field!");
            Console.WriteLine("Please fill all the field!");
            return;
        }
        
        // Verify user
        if (!IsValidEmail(email).Result || !IsValidUsername(username)) {
            // Bad Email or username
            // Messages will pop up in validation functions themself
            return;
        }
        
        var user =  _dbContext.Players.FirstOrDefault(p => p.email == email && p.username == username);
        // Redirect to main page
        if (user != null && VerifyPassword(password, user.password_hash)) {
            if (RememberMeCheckBox.IsChecked == true) {
                EncryptionHelper.SaveUserData(new UserData{UserEmail = email, 
                    UserName = user.username, Password = password}, _loginDataFilePath);
            }
            
            var mainWindow = new MainWindow();
            mainWindow.Show();
            // mainWindow.ShowMessage("Successfully logged in!"); <- will be in future (hopefully I won't forget about this)
            Close();
        }
        else {
            ShowMessage("Invalid email, username or password");
            Console.WriteLine("Invalid email, username or password");
        }
    }

    private void OnRegisterButtonClick(object sender, RoutedEventArgs e) {
        var username = UsernameBox.Text;
        var password = PasswordBox.Text;
        var email = EmailBox.Text;
        
        // If any of fields are empty
        if (username == null || password == null || email == null) {
            ShowMessage("Please fill all the field!");
            Console.WriteLine("Please fill all the field!");
            return;
        }
        if (!IsValidEmail(email).Result) {
            // Email doesn't exists;
            // Messages will pop up in validation functions themself
            return;
        }
        // Check for Email duplicates
        var potentialUser = _dbContext.Players.FirstOrDefault(p => p.email == email);
        if (potentialUser != null) {
            ShowMessage("User with this Email already exists");
            Console.WriteLine("User with this Email already exists");
            return;
        }
        if (!IsValidUsername(username) || !IsValidPassword(password)) {
            // Bad username or password
            // Messages will pop up in validation functions themself
            return;
        }
        // After verifying all data - register user:
        var passwordHash = HashPassword(password);
        var newUser = new players {
            username = username,
            password_hash = passwordHash,
            email = email,
            created_at = DateTime.Now.ToString(CultureInfo.InvariantCulture),
            rating = 800
        };

        var newPlayerStatistics = new player_statistics {
            draws = 0,
            games_played = 0,
            losses = 0,
            wins = 0
        };

        _dbContext.Players.Add(newUser);
        _dbContext.PlayerStatistics.Add(newPlayerStatistics);
        _dbContext.SaveChanges();
        
        // Cache login data if "Remember me" is checked
        if (RememberMeCheckBox.IsChecked == true) {
            EncryptionHelper.SaveUserData(new UserData{UserName = newUser.username, UserEmail = newUser.email,
                Password = password}, _loginDataFilePath);
        }
        
        // Navigate to the main application page
        var mainWindow = new MainWindow();
        mainWindow.Show();
        // mainWindow.ShowMessage("Successfully registered!"); <- will be in future (hopefully I won't forget about this)
        Close();
    }
    
    private static bool VerifyPassword(string password, string storedHash) {
        return BCrypt.Net.BCrypt.Verify(password.Trim(), storedHash.Trim());
    }

    private static string HashPassword(string password) {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private Task<bool> IsValidEmail(string email) {
        const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (Regex.IsMatch(email, emailPattern)) {
            return Task.FromResult(false);
        }

        ShowMessage("Bad email pattern");
        Console.WriteLine("Bad email pattern");
        return Task.FromResult(false);
    }
    
    private bool IsValidPassword(string password) {
        var passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{9,}$";
        if (Regex.IsMatch(password, passwordPattern)) {
            return true;
        }

        ShowMessage("Insecure password! Try making it longer or add numbers");
        Console.WriteLine("Insecure password! Try making it longer or add numbers");
        return false;
    }
    
    private bool IsValidUsername(string username) {
        var isValid = true;
            
        if (username.Length < 3) {
            ShowMessage("Your username is too short");
            Console.WriteLine("Your username is too short");
            isValid = false;
        }
        if (username.Length > 20 || !isValid) {
            ShowMessage("Your username is too long");
            Console.WriteLine("Your username is too long");
            isValid = false;
        }
            
        string[] reservedWords = [
            "admin", "administrator", "root", "system", "support", "moderator",
            "owner", "manager", "staff", "server", "webmaster", "null", 
            "undefined", "anonymous", "guest", "unknown", "user", "test",
            "default", "new", "backup", "help", "contact", "security", 
            "feedback", "info", "account", "billing", "service", "update",
            "LambdaChess"
        ];
        if (reservedWords.Contains(username.ToLower()) || !isValid) {
            ShowMessage("Your username contains prohibited words");
            Console.WriteLine("Your username contains prohibited words");
            isValid = false;
        }

        const string usernamePattern = @"^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$";
        if (!Regex.IsMatch(username, usernamePattern) || !isValid) {
            ShowMessage("Username cannot contain special symbols");
            Console.WriteLine("Username cannot contain special symbols");
            isValid = false;
        }
            
        return isValid;
    }

    private static string GetLoginDataFilePath() {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, @"../../.."));
        var relativePath = Path.Combine(projectRoot, "UserCache", "loginData.json");

        return relativePath;
    }

    private static string GetIconFilePath() {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, @"../../.."));
        var relativePath = Path.Combine(projectRoot, "Assets", "LambdaChessIcon.png");

        return relativePath;
    }
    
    private async void ShowMessage(string message) {
        _notificationHelper.TryDisplayNotification(message, LoginPanel, (int)(Width - LoginPanel.Width) / 2 - 200);
    }
}