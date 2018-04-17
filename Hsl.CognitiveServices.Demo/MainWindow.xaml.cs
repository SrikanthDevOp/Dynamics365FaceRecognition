using Hsl.CognitiveServices.Demo.UserControl;
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

namespace Hsl.CognitiveServices.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> _childTabs = new Dictionary<string, string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public void FaceIdentify_Click(object sender, RoutedEventArgs e)
        {
            D365Login cntrlD365Login = new D365Login();
            cntrlD365Login.SetValue(DockPanel.DockProperty, Dock.Top);
            cntrlD365Login.CloseInitiated += new Close(ClosePanel);
            dockPanel.Children.Clear();
            dockPanel.Children.Add(cntrlD365Login);
            dockPanel.Visibility = Visibility.Visible;
        }

        public void ClosePanel(object sender,EventArgs e)
        {
            dockPanel.Children.Clear();
            dockPanel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Adjust the tab height and weight during load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_Loaded(object sender, RoutedEventArgs e)
        {
            //tabControl.Width = this.ActualWidth;
            //tabControl.Height = this.ActualHeight - 10;
        }
    }

}
