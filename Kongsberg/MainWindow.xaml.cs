using JsonParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace SimulatorUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly AlarmMonitor.ViewModel viewModel;

        public MainWindow() {
            InitializeComponent();

            this.viewModel = new AlarmMonitor.ViewModel();
            //DataGridTelegrams.DataContext = viewModel.obsSensValues1;
            DataGridTelegrams1.ItemsSource = viewModel.obsSensValues1;
            DataGridTelegrams2.ItemsSource = viewModel.obsSensValues2;
            DataGridTelegrams3.ItemsSource = viewModel.obsSensValues3;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            // Attach all receivers to sensors
            foreach (var r in viewModel.receiverList)
                foreach (var s in r.Sensors)
                    viewModel.sensorsDict[s].Attach(r);

            // Start "sensoring" for all sensors
            //foreach (var s in viewModel.sensorsDict)
            //    s.Value.RunSensor(ref viewModel.obsSensValues1);

            viewModel.sensorsDict[1].RunSensor(ref viewModel.obsSensValues1);
            viewModel.sensorsDict[2].RunSensor(ref viewModel.obsSensValues2);
            viewModel.sensorsDict[3].RunSensor(ref viewModel.obsSensValues3);

            this.start.IsEnabled = false;
            this.stop.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            // Stop "sensoring" for all sensors
            foreach (var s in viewModel.sensorsDict)
                s.Value.StopSensor();

            this.start.IsEnabled = true;
            this.stop.IsEnabled = false;
        }
    }
}
