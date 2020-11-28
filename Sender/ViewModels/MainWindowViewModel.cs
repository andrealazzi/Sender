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
        private TcpClient client;
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
            client = new TcpClient(IP, port);
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
                        inputDevice.PolyphonicKeyPressure += PolyphonicHandler;
                        inputDevice.PitchBend += PitchBendHandler;
                    }

                }
                catch { }
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                //Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                //Console.WriteLine("SocketException: {0}", e);
            }
        }
        public void ControlChangeHandler(IMidiInputDevice sender, in NoteOnMessage msg)
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("NoteOnMessage" + "," + msg.Key.ToString() + "," + msg.Velocity.ToString() + "," + RtMidi.Core.Enums.Channel.Channel1.ToString());
            NoteOnMessage message = new NoteOnMessage((Channel)Enum.Parse(typeof(Channel), data[2].ToString()), (Key)Enum.Parse(typeof(Key), data[0].ToString()), Convert.ToInt32(data[1]));

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

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

        }



        public void PolyphonicHandler(IMidiInputDevice sender, in PolyphonicKeyPressureMessage msg)
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("PolyphonicKeyPressureMessage" + "," + msg.Key.ToString() + "," + msg.Pressure.ToString() + "," + RtMidi.Core.Enums.Channel.Channel1.ToString());
            PolyphonicKeyPressureMessage message = new PolyphonicKeyPressureMessage((Channel)Enum.Parse(typeof(Channel), data[2].ToString()), (Key)Enum.Parse(typeof(Key), data[0].ToString()), Convert.ToInt32(data[1]));

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

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

        }

        public void PitchBendHandler(IMidiInputDevice sender, in PitchBendMessage msg)
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("PitchBendMessage" + "," + msg.Channel.ToString() + "," + msg.Value.ToString());
            //PitchBendMessage message = new PitchBendMessage((Channel)Enum.Parse(typeof(Channel), data[2].ToString()), (Key)Enum.Parse(typeof(Key), data[0].ToString()), Convert.ToInt32(data[1]));

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

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

        }
    }




}


