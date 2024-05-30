using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.ViewModels
{
    partial class SplashWindowViewModel : ObservableObject
    {
        public static readonly SplashWindowViewModel Instance = new();

        [ObservableProperty]
        private bool _isCloseActivated = false;

        [ObservableProperty]
        private string _splashText = "hello";

        [ObservableProperty]
        private bool _fadeIn = false;

        private SplashWindowViewModel()
        {
        }

        [RelayCommand]
        private async Task Load()
        {
            Debug.Print($"Load");

            FadeIn = true;
            await Task.Delay(200);

            try
            {
                await Setup.Run();
                MainWindow.Create();
                FadeIn = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FadeIn = false;
                MessageBox.Show("Wooo - there was a fatal startup error:\n\n" + ex.ToString(), "Game Direct", MessageBoxButton.OK, MessageBoxImage.Error);
                IsCloseActivated = true;
            }

        }
    }
}
