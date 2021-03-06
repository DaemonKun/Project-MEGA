﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MetroFramework;
using MySql.Data.MySqlClient;

namespace Desktop_IDE
{
    public static class tcpserver
    {
        #region Constants
        private const int c_bufferSize = 5242880;
        #endregion

        #region Members
        //public static List<Socket> _clientSockets = new List<Socket>();
        public static Dictionary<string, string> user = new Dictionary<string, string>(); 
        private delegate void AddClientHandler(IPEndPoint IpEndPoint);
        private delegate void EnableSendHandler();
        public static Main Server { get; set; }
        public static string subject { get; set; }
        public static string exam { get; set; }
        public static IMain main;
        public static int score;
        private static byte[] _buffer = new byte[c_bufferSize];
        private static Socket _serverSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        #endregion


        public static void SetupServer()
        {
            try
            {
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            }
            catch (SocketException ex)
            {
                MetroMessageBox.Show(Server, ex.Message);
                return;
            }
            finally
            {
                _serverSocket.Listen(100);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
            }
            

        }
        private static void AcceptCallBack(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            //_clientSockets.Add(socket);

            IPEndPoint ipEndPoint = socket.RemoteEndPoint as IPEndPoint;

            if ((ipEndPoint) != null)
            {
                Server.BeginInvoke(new AddClientHandler(Server.AddClient), ipEndPoint);
            }

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceivedCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }
        private static void ReceivedCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            try
            {
                IPEndPoint ip = socket.RemoteEndPoint as IPEndPoint;
                int received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                string text = Encoding.ASCII.GetString(dataBuf);


                string response = string.Empty;
                if (text.ToLower() == "get file")
                {
                    #region veeeeeeeeeery loooooooooong loooooooop
                    int i = 0;
                    string temp = string.Empty;
                    for (; i < testData.strQuestion.Count; i++)
                    {
                        if (string.IsNullOrEmpty(testData.strQuestion[i]))
                        {
                            break;
                        }
                        else
                        {
                            temp += "<" + (i + 1).ToString() + ">" + "<Q>:" + testData.strQuestion[i] + "\n";
                            temp += "<" + (i + 1).ToString() + ">" + "<A>:" + testData.strA[i] + "\n";
                            temp += "<" + (i + 1).ToString() + ">" + "<B>:" + testData.strB[i] + "\n";
                            temp += "<" + (i + 1).ToString() + ">" + "<C>:" + testData.strC[i] + "\n";
                            temp += "<" + (i + 1).ToString() + ">" + "<D>:" + testData.strD[i] + "\n";
                        }
                            
                    }
                    response = i.ToString() + "\n" + temp;
                    #endregion
                }
                
                else if (text.Contains("RFID:"))
                {
                    string RFID = text.Substring(text.IndexOf(':') + 1, 12);
                    string cmd = "SELECT * FROM u614761466_mega.users WHERE RFID_tag='" + RFID + "' ;";
                    MySqlConnection conn = mysqlCon.conn();
                    
                    MySqlCommand selectCommand = new MySqlCommand(cmd, conn);
                    try
                    {
                        MySqlDataReader reader;

                        conn.Open();
                        reader = selectCommand.ExecuteReader();
                        int count = 0;
                        while (reader.Read())
                        {
                            count++;
                        }
                        
                        if (count == 1)
                        {
                            user.Add(ip.ToString(), RFID);
                            response = "valid";
                        }
                        else
                        {
                            response = "not valid";
                        }


                        reader.Close();

                    }
                    catch (Exception e)
                    {
                        MetroMessageBox.Show(Server, e.Message);
                    }
                    finally
                    {
                        
                        conn.Close();
                    }
                    
                }
                else if (text.Contains("<Answers>:"))
                {
                    String filename = otherFunctions.TmpFile();
                    StreamWriter sw = new StreamWriter(filename);

                    sw.Write(text);
                    sw.Close();
                    score = otherFunctions.checkAnswers(filename);
                    otherFunctions.DeleteTmpFile(filename);
                    addScore(ip.ToString());
                    
                    
                }
                else if (text.Contains("get score"))
                {
                    response = "<score>:" + score.ToString() +  " over " + main.MAX.ToString();
                }
                else
                {
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceivedCallback), socket);
                }
                byte[] data = Encoding.ASCII.GetBytes(response);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);

                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceivedCallback), socket);

            }
            catch (SocketException)
            {

            }
            
        }

        
        private static void SendCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            //if (SocketExtensions.IsConnected(socket))
            //{

            //}
            socket.EndSend(AR);
        }

        private static void addScore(string ip)
        {
            string query = "SELECT * FROM u614761466_mega.users WHERE RFID_tag='" + user[ip] + "' ;";
            string studentnum = string.Empty;
            string query2 = "UPDATE u614761466_mega." + subject + "SET" + exam + "='"+score.ToString()
                +"' WHERE studentnum='" + studentnum +"'  ;";

            using (var con = mysqlCon.conn())
            {
                using (var cmd = new MySqlCommand(query, con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            studentnum = reader.GetString("studentnum");
                        }
                        reader.Close();
                    }
                    con.Close();
                }
                using (var cmd2 = new MySqlCommand(query2, con))
                {
                    con.Open();
                    using (var reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                        }
                        reader.Close();
                    }
                    con.Close();
                }
            }


        }
        
    }
    static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }
}
