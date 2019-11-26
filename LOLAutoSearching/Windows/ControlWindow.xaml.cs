using LOLAutoSearching.ViewModel;
using System.Diagnostics;
using System.Windows;

namespace LOLAutoSearching.Windows
{
    public partial class ControlWindow : Window
    {
        public ControlWindow()
        {
            InitializeComponent();
            ControlViewModel controlViewModel = new ControlViewModel();
            DataContext = controlViewModel;
            Closed += (o, e) => { Process.GetCurrentProcess().Kill(); };
        }
    }
}
