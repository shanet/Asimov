//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace ClientDebugger
{
    using System.Windows;

    using AsimovClient.Create;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICreateController roomba;

        public MainWindow()
        {
            this.roomba = new AsimovController(new TcpCreateCommunicator("127.0.0.1", 4545));

            this.InitializeComponent();
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

        private void DriveButton_Click(object sender, RoutedEventArgs e)
        {
            DriveProperties settings = this.Resources["DriveSettings"] as DriveProperties;

            if (settings == null)
            {
                MessageBox.Show("Settings is null!");
                return;
            }

            if (settings.IsIndefinate)
            {
                if (settings.IsStraight)
                {
                    this.roomba.Drive(settings.Velocity);
                }
                else if (settings.IsSpin)
                {
                    this.roomba.Spin(settings.Velocity);
                }
                else
                {
                    this.roomba.Drive(settings.Velocity, settings.Radius);                    
                }
            }
            else if (settings.IsDistance)
            {
                if (settings.IsStraight)
                {
                    this.roomba.DriveDistance(settings.Velocity, settings.Distance);
                }
                else if (settings.IsSpin)
                {
                    MessageBox.Show("Cannot spin for a distance.");
                }
                else
                {
                    this.roomba.DriveDistance(settings.Velocity, settings.Radius, settings.Distance);
                }
            }
            else if (settings.IsAngle)
            {
                if (settings.IsStraight)
                {
                    MessageBox.Show("Cannot drive straight for an angle.");
                }
                else if (settings.IsSpin)
                {
                    this.roomba.SpinAngle(settings.Velocity, settings.Angle);
                }
                else
                {
                    MessageBox.Show("Cannot drive for an angle.");
                }
            }
            else if (settings.IsTime)
            {
                if (settings.IsStraight)
                {
                    this.roomba.DriveTime(settings.Velocity, settings.Time);
                }
                else if (settings.IsSpin)
                {
                    this.roomba.SpinTime(settings.Velocity, settings.Time);
                }
                else
                {
                    this.roomba.DriveTime(settings.Velocity, settings.Radius, settings.Time);
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.roomba.Stop();
        }
    }
}
