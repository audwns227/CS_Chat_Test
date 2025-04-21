using client.Commands;
using client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace client.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TextModel _textModel;
        private TcpClient client = null;
        public ICommand SendCommand { get; }

        public TextModel textModel
        {
            get => _textModel;
            set { _textModel = value; OnPropertyChanged(nameof(textModel)); }
        }

        private ObservableCollection<string> messageList = new ObservableCollection<string>();

        public ObservableCollection<string> MessageList
        {
            get => messageList;
            set { messageList = value; OnPropertyChanged(nameof(MessageList)); }
        }

        private void Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 1234);
                MessageList.Add("[LOG] : Connected");
            }
            catch (Exception ex)
            {
                MessageList.Add("[LOG] : Connection failed - " + ex.Message);
            }
        }

        private void SendMessage()
        {
            if (string.IsNullOrEmpty(textModel.Text))
            {
                Debug.WriteLine("Empty Message");
                return;
            }
            string message = textModel.Text;
            byte[] byteData = Encoding.Default.GetBytes(message);

            try
            {
                client.GetStream().Write(byteData, 0, byteData.Length);
                _sendAckT.TrySetResult(true);
                MessageList.Add($"[Client] : {message}");
            }
            catch(Exception ex)
            {
                MessageList.Add("[LOG] : Send failed - " + ex.Message);
            }
            textModel.Text = string.Empty;
        }

        private void ReceivedMessage()
        {
            byte[] message = new byte[1024];
            client.GetStream().Read(message, 0, message.Length);

            try
            {
                string strData = Encoding.Default.GetString(message);

                Debug.WriteLine($"Server Message : {strData}");
                int endPoint = strData.IndexOf('\0');
                string parsedMessage = strData.Substring(0, endPoint + 1);
                //Debug.WriteLine($"Server Message : {parsedMessage}");

                MessageList.Add($"[Server] : {parsedMessage}");
            }
            catch(Exception ex)
            {
                MessageList.Add("[LOG] : Received failed - " + ex.Message);
            }

        }

        public MainViewModel()
        {
            _textModel = new TextModel();
            Connect();
            SendCommand = new Command(async (_) => await SendAsync());
        }

        private TaskCompletionSource<bool> _sendAckT;

        public async Task<bool> SendTextCommandAsync(int timeout = 3000)
        {
            _sendAckT = new TaskCompletionSource<bool>();

            //TODO
            //서버에 전송 후 수신
            SendMessage();
           
            var TimeoutTask = Task.Delay(timeout);
            var completeTask = await Task.WhenAny(_sendAckT.Task, TimeoutTask);

            if(completeTask == _sendAckT.Task)
                return _sendAckT.Task.Result;
            else
                return false;
        }
        private async Task SendAsync()
        {
            bool result = await SendTextCommandAsync(timeout: 3000);

            Debug.WriteLine("[LOG] : Finish Send");
            if (result)
            {
                Debug.WriteLine("Send Complete");
                ReceivedMessage();
            }
            else
            {
                Debug.WriteLine("Send Error");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
