using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerConnector
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

        public async void SendMessage(IPAddress IP, int port, string Message)
        {
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IP, port);
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(serverEndPoint);
                var data = Encoding.Unicode.GetBytes(Message);
                server.Send(data);

                data = new byte[1024];
                int bytes = 0;
                do
                {
                    bytes = server.Receive(data);
                    MessageBox.Show($"Server answered {Encoding.Unicode.GetString(data)}");

                } while (server.Available > 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendMessage(IPAddress.Parse(IPInput.Text), Int32.Parse(PortInput.Text), UserMessageInput.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Invalid Data!");
            }
        }
    }
}