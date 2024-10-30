using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using LambdaChess.Helpers;
using LambdaChess.Models;

namespace LambdaChess;

public partial class LoginWindow : Window {
    private readonly ApplicationContext _dbContext;

    private readonly string loginDataFilePath =
        @"D:\Programs\JetBrains Rider 2023.3.2\Projects\LamdaChess\LambdaChess\LambdaChess\LambdaChess\UserCache\loginData.json";
    
    public LoginWindow() {
        InitializeComponent();
        _dbContext = new ApplicationContext();
        LoadUserData();
    }
    
    private async void LoadUserData() {
        var userData = (UserData?)EncryptionHelper.LoadUserData<UserData>(loginDataFilePath);
        if (userData == null) {
            // Handle event
            return;
        }

        UsernameBox.Text = userData.UserName;
        EmailBox.Text = userData.UserEmail;
        PasswordBox.Text = userData.Password;

        var playerData = _dbContext.Players.FirstOrDefault(p => p.email == userData.UserEmail);
            
        if (playerData != null && VerifyPassword(userData.Password, playerData.password_hash)) {
            // Transition to the main application page
            await Task.Delay(500); 
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        else {
            // Show login failure message
        }
    }

    private void OnLoginButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        var username = UsernameBox.Text;
        var password = PasswordBox.Text;
        var email = EmailBox.Text;
        
        if (username == null || password == null || email == null) {
            // Bad user
            return;
        }
        
        var user =  _dbContext.Players.FirstOrDefault(p => p.email == email && p.username == username);

        if (user != null && VerifyPassword(password, user.password_hash)) {
            EncryptionHelper.SaveUserData(new UserData{UserEmail = email, 
                UserName = user.username, Password = password}, loginDataFilePath);
            // Transition to the main application page
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        else {
            // Show login failure message
        }
    }

    private void OnRegisterButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        var username = UsernameBox.Text;
        var password = PasswordBox.Text;
        var email = EmailBox.Text;

        if (username == null || password == null || email == null) {
            // Bad user
            return;
        }

        var potentialUser = _dbContext.Players.FirstOrDefault(p => p.email == email);
        if (potentialUser != null) {
            // Duplicate
            return;
        }
        
        var passwordHash = HashPassword(password);
        var newUser = new players {
            username = username,
            password_hash = passwordHash,
            email = email,
            created_at = DateTime.Now.ToString(CultureInfo.InvariantCulture),
            rating = 800
        };

        var newPlayerStatistics = new player_statistics {
            player_id = newUser.player_id,
            draws = 0,
            games_played = 0,
            losses = 0,
            wins = 0
        };

        _dbContext.Players.Add(newUser);
        _dbContext.PlayerStatistics.Add(newPlayerStatistics);
        _dbContext.SaveChanges();
        
        EncryptionHelper.SaveUserData(new UserData{UserName = newUser.username, UserEmail = newUser.email,
                Password = password}, loginDataFilePath);

        // After successful registration, navigate to the main application page
        var mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }
    
    private static bool VerifyPassword(string password, string storedHash) {
        return BCrypt.Net.BCrypt.Verify(password.Trim(), storedHash.Trim());
    }

    private static string HashPassword(string password) {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}