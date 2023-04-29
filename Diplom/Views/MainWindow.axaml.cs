using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Diplom.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace Diplom.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowDialogFFT.RegisterHandler(DoShowDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowDialogSettings.RegisterHandler(DoShowDialogSettingsAsync)));
        }
        private async Task DoShowDialogAsync(InteractionContext<FFTWindowViewModel, TestViewModel?> interaction)
        {
            var dialog = new FFTWindow
            {
                DataContext = new FFTWindowViewModel(),
            };
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<TestViewModel?>(this);
            interaction.SetOutput(result);
        }
        private async Task DoShowDialogSettingsAsync(InteractionContext<SettingsWindow, TestViewModel?> interaction)
        {
            var dialog = new SettingsWindow
            {
                DataContext = new TestViewModel(),
            };
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<TestViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}