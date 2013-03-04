using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AngleRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.AngleTextBox != null)
            {
                this.AngleTextBox.IsEnabled = true;
            }
        }

        private void AngleRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.AngleTextBox != null)
            {
                this.AngleTextBox.IsEnabled = false;
            }
        }

        private void TimeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.TimeTextBox != null)
            {
                this.TimeTextBox.IsEnabled = true;
            }
        }

        private void TimeRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.TimeTextBox != null)
            {
                this.TimeTextBox.IsEnabled = false;
            }
        }

        private void DistanceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DistanceTextBox != null)
            {
                this.DistanceTextBox.IsEnabled = true;
            }
        }

        private void DistanceRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.DistanceTextBox != null)
            {
                this.DistanceTextBox.IsEnabled = false;
            }
        }

        private void RadiusRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.RadiusTextBox != null)
            {
                this.RadiusTextBox.IsEnabled = true;
            }
        }

        private void RadiusRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.RadiusTextBox != null)
            {
                this.RadiusTextBox.IsEnabled = false;
            }
        }
    }
}
