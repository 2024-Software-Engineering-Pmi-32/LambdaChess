using System.IO;
using Avalonia.Controls;

namespace LambdaChess;

public partial class MainWindow : Window {
    private readonly string loginDataFilePath =
        @"D:\Programs\JetBrains Rider 2023.3.2\Projects\LamdaChess\LambdaChess\LambdaChess\LambdaChess\UserCache\loginData.json";
    
    public MainWindow() {
        InitializeComponent();
    }

    private void OnLogoutButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        File.WriteAllText(loginDataFilePath, "");
        
        // Return to login page
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        Close(); // Close the main window
    }
}