using System;
using System.CodeDom.Compiler;
using System.IO.Ports;
using System.Threading;

namespace SerialPortChat
{
    internal class Program
    {
        private static bool _continue;
        private static SerialPort _serialPort;

        private static void Main(string[] args)
        {
            string name;
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new Thread(Read);

            //Create a new SerialPort
            _serialPort = initializeSerialPort();

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _continue = true;
            readThread.Start();

            Console.Write("Name: ");
            name = Console.ReadLine();

            Console.WriteLine("Type QUIT to exit");

            while (_continue)
            {
                message = Console.ReadLine();

                if (stringComparer.Equals("quit", message))
                {
                    _continue = false;
                }
                else
                {
                    _serialPort.WriteLine(String.Format("<{0}>: {1}", name, message));
                }
            }

            readThread.Join();
            _serialPort.Close();
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }

        private static SerialPort initializeSerialPort()
        {
            SerialPort serialPort = new SerialPort();

            serialPort.PortName = SetPortName(serialPort.PortName);
            serialPort.BaudRate = SetPortBaudRate(serialPort.BaudRate);
            serialPort.Parity = SetPortParity(serialPort.Parity);
            serialPort.DataBits = SetPortDataBits(serialPort.DataBits);
            serialPort.StopBits = SetPortStopBits(serialPort.StopBits);
            serialPort.Handshake = SetPortHandskake(serialPort.Handshake);

            return serialPort;
        }

        //Display Port Values and allow user to enter a port.
        private static string SetPortName(string serialPortPortName)
        {
            string portName;
            Console.WriteLine("Available Ports:");
            foreach (string name in SerialPort.GetPortNames())
            {
                Console.WriteLine($"{name,5}");
            }
            Console.Write($"Enter COM port value (Default: {serialPortPortName}): ");
            portName = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(portName) || !portName.ToLower().StartsWith("com"))
            {
                portName = serialPortPortName;
            }

            return portName;
        }

        private static int SetPortBaudRate(int serialPortBaudRate)
        {
            string baudRate;

            Console.Write($"Baud Rate(default:{serialPortBaudRate}): ");
            baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = serialPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

        private static Parity SetPortParity(Parity serialPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine($"{s,5}");
            }

            Console.Write($"Enter Parity value (Default: {serialPortParity.ToString()}):", true);
            parity = Console.ReadLine();

            if (parity == "")
            {
                parity = serialPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity, true);
        }

        private static int SetPortDataBits(int serialPortDataBits)
        {
            string dataBits;

            Console.Write($"Enter DataBits value (Default: {serialPortDataBits}): ");
            dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = serialPortDataBits.ToString();
            }

            return int.Parse(dataBits.ToUpperInvariant());
        }

        private static StopBits SetPortStopBits(StopBits serialPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine($"{s}");
            }

            Console.Write($"Enter StopBits value (None is not supported and \n" +
                          "raises an ArgumentOutOfRangeException. \n (Default: {serialPortStopBits.ToString()}):");
            stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = serialPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }

        private static Handshake SetPortHandskake(Handshake serialPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write($"Enter Handshake value (Default: {serialPortHandshake.ToString()}):");
            handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = serialPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }
    }
}