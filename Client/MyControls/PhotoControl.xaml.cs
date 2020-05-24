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
using System.Net.Sockets;
using ChatInstruction;

namespace Client.MyControls
{
    /// <summary>
    /// Логика взаимодействия для PhotoControl.xaml
    /// </summary>
    public partial class PhotoControl : UserControl
    {
        string name;
        NetworkStream stream;

        public PhotoControl(string name, string time, NetworkStream stream)
        {
            InitializeComponent();

            this.textBlock_Time.Text = time;
            this.name = name;
            this.stream = stream;
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
