using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Enums;
using RtMidi.Core.Messages;

namespace Sender.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string iP;
        private string message;
        private int port;
        public string IP
        {

            get { return this.iP; }
            set
            {

                this.iP = value;
            }
        }
        public int Port
        {

            get { return this.port; }
            set
            {
                this.port = value;
            }
        }
        public string Message
        {

            get { return this.message; }
            set
            {
                this.message = value;
            }
        }
        public void OnClickCommand()
        {
            string ipNum = IP;
            string msg = Message;
            int port = Port;
            try
            {
                var devices = new List<IMidiInputDevice>();
                try
                {
                    foreach (var inputDeviceInfo in MidiDeviceManager.Default.InputDevices)
                    {
                        Console.WriteLine($"Opening {inputDeviceInfo.Name}");

                        var inputDevice = inputDeviceInfo.CreateDevice();
                        devices.Add(inputDevice);

                        //inputDevice.ControlChange += ControlChangeHandler;
                        inputDevice.Open();
                       inputDevice.NoteOn += ControlChangeHandler;
                    }

                    //Console.WriteLine("Press any key to stop...");
                    //Console.ReadKey();

                }
                catch { }
                //finally
                //{
                //    foreach (var device in devices)
                //    {
                //        device.NoteOn -= ControlChangeHandler;
                //        device.Dispose();
                //    }
                //}
                
                
                
            }
            catch (ArgumentNullException e)
            {
                //Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                //Console.WriteLine("SocketException: {0}", e);
            }

            //Console.WriteLine("\n Press Enter to continue...");
            //Console.Read();
        }
        public  void ControlChangeHandler(IMidiInputDevice sender, in NoteOnMessage msg)
        {
            //System.IO.File.WriteAllText(@"C:\Users\AndreaLazzi\Downloads\WriteLines.txt", msg.ToString());

            //Console.WriteLine($"[{sender.Name}] ControlChange: Channel:{msg.Channel} Control:{msg.Velocity} Value:{msg.Velocity}");
            //string msg2 = msg.Key.ToString();

            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            TcpClient client = new TcpClient(IP, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg.Key.ToString() + "," + msg.Velocity.ToString() + "," + RtMidi.Core.Enums.Channel.Channel1.ToString());
            NoteOnMessage message = new NoteOnMessage((Channel)Enum.Parse(typeof(Channel), data[2].ToString()), (Key)Enum.Parse(typeof(Key), data[0].ToString()), Convert.ToInt32(data[1]));
            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            //Console.WriteLine("Sent: {0}", msg);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //Console.WriteLine("Received: {0}", responseData);

            // Close everything.
            stream.Close();
            client.Close();

        } 
    }

   


}


