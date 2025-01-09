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

namespace FlickServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetLocalIP();
        }

        public void GetLocalIP()
        {
            string hostName = Dns.GetHostName();

            IPHostEntry localhost = Dns.GetHostEntry(hostName);

            foreach (IPAddress entry in localhost.AddressList)
            {
                MenuItem option = new MenuItem();
                option.Header = entry;
                option.Click += IPOption_Click;

                LocalIpMenu.Items.Add(option);
            }
        }

        private void IPOption_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            IPInput.Text = menuItem.Header.ToString();
        }

        bool IsRunServer = false;

        private void RunServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(IPInput.Text);
                int port = Int32.Parse(PortInput.Text);
                Task.Run(() => RunServer(ip, port));
                RunButton.Visibility = Visibility.Collapsed;
                StopButton.Visibility = Visibility.Visible;
                IsRunServer = true;
                ServerOutput.Text += "Server Startup...";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Invalid Run data");
            }
        }

        private async void RunServer(IPAddress IP, int port)
        {
            string hostName = Dns.GetHostName();

            IPEndPoint iPEndPoint = new IPEndPoint(IP, port); 

            Socket server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Bind(iPEndPoint);
                server.Listen(10);

                while (IsRunServer)
                { 
                    Socket client = server.Accept();
                    int bytes = 0;
                    byte[] buffer = new byte[1024];
                    do
                    {
                        bytes = client.Receive(buffer);
                        Dispatcher.Invoke(() => { ServerOutput.Text += $"\nMessage: '{Encoding.Unicode.GetString(buffer)}'"; });
                    } while (client.Available > 0);
                    string message = $"\nMessage Recieved {DateTime.Now}";
                    buffer = Encoding.Unicode.GetBytes(message);
                    client.Send(buffer);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Problem {ex.Message}");
            }
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            IsRunServer = false;
            RunButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Collapsed;
            ServerOutput.Text = "";
        }

        ~MainWindow() {
            IsRunServer = false;
        }
    }
}