namespace LambdaChess;

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
using Helpers;
using Models;

public partial class LoginWindow : Window {
    private readonly ApplicationContext _dbContext;
    private readonly string _loginDataFilePath = GetLoginDataFilePath();
    
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
    
    #region INTERACTION METHODS
    
    private void OnInputFieldsChanged(object? sender, TextChangedEventArgs e) => UpdateButtonStates();
    private void OnAgreementChecked(object? sender, RoutedEventArgs e) => UpdateButtonStates();
    
    // ReSharper disable UnusedParameter.Local
    // Because it is required to use sender and TextChangedArgs in those method they cannot be removed
    // But they still give a warning
    private void OnPasswordInput(object? sender, TextChangedEventArgs e) {
        var currentPassword = PasswordBox.Text;
        
        if (currentPassword == null || currentPassword.Length < 9) {
            FlyoutMessage.Text = "Password must be at least 9 characters long";
        }
        else if (!PasswordRegex().IsMatch(currentPassword)) {
            FlyoutMessage.Text = "Password must contain letters and numbers";
        }
        else {
            FlyoutMessage.Text = "Password seems good to me!";
        }
        
        FlyoutBase.ShowAttachedFlyout(PasswordBox);
    }
    
    private void OnUserAgreementLinkClick(object? sender, TappedEventArgs e) {
        Process.Start(
            new ProcessStartInfo {
            FileName = "https://github.com/2024-Software-Engineering-Pmi-32/LambdaChess-docs/blob/main/LICENSE",
            UseShellExecute = true
        });
    }

    private void OnPrivacyPolicyLinkClick(object? sender, TappedEventArgs e) {
        Process.Start(
            new ProcessStartInfo {
            FileName = "https://github.com/2024-Software-Engineering-Pmi-32/LambdaChess-docs/blob/main/LICENSE",
            UseShellExecute = true
        });
    }
    
    private void OnRegisterButtonClick(object sender, RoutedEventArgs e) {
        var username = UsernameBox.Text;
        var password = PasswordBox.Text;
        var email = EmailBox.Text;
        
        // If any of fields are empty
        if (username == null || password == null || email == null) {
            ShowMessage("Please fill all the field!");
            return;
        }
        
        if (!IsValidEmail(email)) {
            // Email doesn't exists;
            // Messages will pop up in validation functions themself
            return;
        }
        
        // Check for Email duplicates
        var potentialUser = _dbContext.Players.FirstOrDefault(p => p.email == email);
        if (potentialUser != null) {
            ShowMessage("User with this Email already exists");
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
            EncryptionHelper.SaveUserData(
                new UserData {
                    UserName = newUser.username, UserEmail = newUser.email, Password = password
                }, 
                _loginDataFilePath);
        }
        
        // Navigate to the main application page
        var mainWindow = new MainWindow();
        mainWindow.Show();
        
        // mainWindow.ShowMessage("Successfully registered!"); <- will be in future (hopefully I won't forget about this)
        Close();
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
        if (!IsValidEmail(email) || !IsValidUsername(username)) {
            // Bad Email or username
            // Messages will pop up in validation functions themself
            return;
        }
        
        var user = _dbContext.Players.FirstOrDefault(p => p.email == email && p.username == username);
        
        // Redirect to main page
        if (user != null && VerifyPassword(password, user.password_hash)) {
            if (RememberMeCheckBox.IsChecked == true) {
                EncryptionHelper.SaveUserData(
                    new UserData {
                        UserEmail = email, UserName = user.username, Password = password
                    }, 
                    _loginDataFilePath);
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
    
    // ReSharper restore UnusedParameter.Local
    #endregion

    #region LOCAL METHODS

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
        if (userData.UserName == null || userData.UserEmail == null || userData.Password == null) {
            ShowMessage("Some cached data is missing!");
            return;
        }
        
        if (!IsValidEmail(userData.UserEmail)) {
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
        }
    }
    
    private static bool VerifyPassword(string password, string storedHash) {
        return BCrypt.Net.BCrypt.Verify(password.Trim(), storedHash.Trim());
    }

    private static string HashPassword(string password) {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    private void ShowMessage(string message) {
        var horizontalOffset = (int)(Width - LoginPanel.Width) / 2;
        const int buttonWidth = 200;
        NotificationHelper.TryDisplayNotification(message, LoginPanel, horizontalOffset - buttonWidth);
    }

    #endregion
    
    // ALL OTHER METHODS SHOULD BE MOVED OUTSIDE OF THIS CLASS
    private bool IsValidEmail(string email) {
        if (EmailRegex().IsMatch(email)) {
            return true;
        }

        ShowMessage("Bad email pattern");
        return false;
    }
    
    private bool IsValidPassword(string password) {
        if (PasswordRegex().IsMatch(password)) {
            return true;
        }

        ShowMessage("Insecure password! Try making it longer or add numbers");
        return false;
    }
    
    private bool IsValidUsername(string username) {
        string[] reservedWords = [
            "admin", "administrator", "root", "system", "support", "moderator",
            "owner", "manager", "staff", "server", "webmaster", "null", 
            "undefined", "anonymous", "guest", "unknown", "user", "test",
            "default", "new", "backup", "help", "contact", "security", 
            "feedback", "info", "account", "billing", "service", "update",
            "LambdaChess"
        ];
        
        switch (username.Length) {
            case < 3:
                ShowMessage("Your username is too short");
                return false;
            case > 20:
                ShowMessage("Your username is too long");
                return false;
        }
        
        if (reservedWords.Contains(username.ToLower())) {
            ShowMessage("Your username contains prohibited words");
            return false;
        }

        // ReSharper disable once InvertIf
        // This code is already spaghetti enough, so we will keep this one
        if (!UsernameRegex().IsMatch(username)) {
            ShowMessage("Username cannot contain special symbols");
            return false;
        }

        return true;
    }

    private static string GetLoginDataFilePath() {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "../../.."));
        var relativePath = Path.Combine(projectRoot, "UserCache", "loginData.json");

        return relativePath;
    }

    private static string GetIconFilePath() {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "../../.."));
        var relativePath = Path.Combine(projectRoot, "Assets", "LambdaChessIcon.png");

        return relativePath;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
    
    [GeneratedRegex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{9,}$")]
    private static partial Regex PasswordRegex();
    
    [GeneratedRegex("^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$")]
    private static partial Regex UsernameRegex();
}