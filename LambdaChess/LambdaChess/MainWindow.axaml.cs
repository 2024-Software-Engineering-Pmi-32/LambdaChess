using System.IO;
using Avalonia.Controls;

namespace LambdaChess;

// Raw window. Will Adjust everything later.

public partial class MainWindow : Window {
    private const string LoginDataFilePath = @"D:\Programs\JetBrains Rider 2023.3.2\Projects\LamdaChess\LambdaChess\LambdaChess\LambdaChess\UserCache\loginData.json";

    public MainWindow() {
        InitializeComponent();
    }
    
    private void OnLogoutButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        File.WriteAllText(LoginDataFilePath, "");
        
        // Return to login page
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        Close(); // Close the main window
    }
}