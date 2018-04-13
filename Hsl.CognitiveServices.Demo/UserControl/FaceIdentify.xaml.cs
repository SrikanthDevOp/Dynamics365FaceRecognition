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

namespace Hsl.CognitiveServices.Demo.UserControl
{ 
   /// <summary>
    /// Interaction logic for FaceIdentify.xaml
    /// </summary>
    public partial class FaceIdentify :ITabbed
    {
        public FaceIdentify()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This event will be fired when user will click close button
        /// </summary>
        public event Close CloseInitiated;

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseInitiated?.Invoke(this, new EventArgs());
        }

    }
}
