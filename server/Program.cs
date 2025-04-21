using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        IPAddress ipAddress = IPAddress.Any; // 모든 네트워크 인터페이스에서 수신 대기
        TcpListener listener = new TcpListener(ipAddress, 1234); // 포트 1234에서 수신 대기
        listener.Start();
        Console.WriteLine("서버 시작");

        TcpClient client = listener.AcceptTcpClient(); // 클라이언트 객체를 만들어 1234에 연결한 client 연결 수락
        Console.WriteLine("클라이언트 연결됨");

        NetworkStream stream = client.GetStream();
        while (true)
        {
            // Socket은 byte[] 형식으로 데이터를 주고받음
            byte[] byteData = new byte[1024];
            //client가 write한 정보를 읽어옴
            stream.Read(byteData, 0, byteData.Length);

            //출력을 위해 string형으로 바꿔줌
            string strData = Encoding.Default.GetString(byteData);

            //byteData의 크기는 1024인데 스트림에서 읽어온 데이터가 1024보다 작은 경우
            // 공백이 출력되니 비어있는 문자열을 제거
            int endPoint = strData.IndexOf('\0');
            string parsedMessage = strData.Substring(0, endPoint + 1);

            //파싱된 데이터를 출력해주고 무한 반복
            Console.WriteLine(parsedMessage);

            stream.Write(byteData, 0, byteData.Length);

        }
    }
}
   
