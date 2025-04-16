using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main()
    {
        IPAddress ipAddress = IPAddress.Any; // 모든 네트워크 인터페이스에서 수신 대기
        TcpListener listener = new TcpListener(ipAddress, 1234); // 포트 1234에서 수신 대기
        listener.Start();
        Console.WriteLine("서버 시작");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient(); // 클라이언트 연결 수락
            Console.WriteLine("클라이언트 연결됨");

            // 각 클라이언트 연결을 처리할 새로운 스레드 생성
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        try
        {
            using (StreamReader sr = new StreamReader(stream))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                string message = sr.ReadLine();
                Console.WriteLine("클라이언트가 보낸 메시지: " + message);

                sw.WriteLine("서버 응답: " + message);
                sw.Flush();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("오류: " + ex.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("클라이언트 연결 종료");
        }
    }
}
