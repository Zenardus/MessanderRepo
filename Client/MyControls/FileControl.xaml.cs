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
using System.Windows.Forms;

namespace Client.MyControls
{
    /// <summary>
    /// Логика взаимодействия для FileControl.xaml
    /// </summary>
    public partial class FileControl : System.Windows.Controls.UserControl
    {
        NetworkStream stream;
        public new string Name { get; private set; }

        public FileControl(string name, string size, string time, NetworkStream stream)
        {
            InitializeComponent();

            textBlock_Name.Text = name;
            Name = name;
            textBlock_Size.Text = size;
            textBlock_Time.Text = time;
            this.stream = stream;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Instruction instr = new Instruction(Operation.GetFile, Name, sfd.FileName, null);
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
