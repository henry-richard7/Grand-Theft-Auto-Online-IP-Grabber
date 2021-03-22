using PcapDotNet.Core;
using PcapDotNet.Packets;
using System;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;


namespace GTA_Online_IP_Grabber
{
    public partial class Form1 : Form
    {
        public List<String> ipAddresses = new List<string>();
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public List<String> getGeoInfo(String ip2Geo)
        {

            IP2Location.IPResult oIPResult = new IP2Location.IPResult();
            IP2Location.Component oIP2Location = new IP2Location.Component();

            List<String> ipInfos = new List<string>();

            try {
                oIP2Location.IPDatabasePath = Environment.CurrentDirectory + @"\\Location DB\\IP2LOCATION-LITE-DB11.BIN";
                oIPResult = oIP2Location.IPQuery(ip2Geo);

                if (oIPResult.Status.ToString().Equals("OK"))
                {
                    ipInfos.Add(oIPResult.CountryLong);
                    ipInfos.Add(oIPResult.City);
                    ipInfos.Add(oIPResult.Region);
                    ipInfos.Add(oIPResult.ZipCode);
                    ipInfos.Add(oIPResult.TimeZone);

                    return ipInfos;

                }
                else
                {
                    List<String> errorLists = new List<string>();

                    errorLists.Add(oIPResult.Status.ToString());
                    errorLists.Add(oIPResult.Status.ToString());
                    errorLists.Add(oIPResult.Status.ToString());
                    errorLists.Add(oIPResult.Status.ToString());
                    errorLists.Add(oIPResult.Status.ToString());

                    return errorLists;
                }
            }
            catch
            {
                List<String> errorListse = new List<string>();

                errorListse.Add("No Data in DB");
                errorListse.Add("No Data in DB");
                errorListse.Add("No Data in DB");
                errorListse.Add("No Data in DB");
                errorListse.Add("No Data in DB");

                return errorListse;
            }


        }

        private void PacketHandler(Packet packet)
        {
            
            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;

            if (!ipAddresses.Contains(ip.Destination.ToString()))
            {
                ipAddresses.Add(ip.Destination.ToString());
                String[] rows = { ip.Destination.ToString(), getGeoInfo(ip.Destination.ToString())[0],
                    getGeoInfo(ip.Destination.ToString())[1], 
                    getGeoInfo(ip.Destination.ToString())[2],
                    getGeoInfo(ip.Destination.ToString())[3],
                    getGeoInfo(ip.Destination.ToString())[4]
                };
                
                ListViewItem item = new ListViewItem(rows);
                listView1.Items.Add(item);
            }

            
            
        }
        private void DevicePrint(IPacketDevice device)
        {
            
            foreach (DeviceAddress address in device.Addresses)
            {


                
                if (device.Description != null)
                    comboBox1.Items.Add(device.Description);
            }
            
        }
        
        public IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
        private void Form1_Load(object sender, EventArgs e)
            {
            

            for (int i = 0; i < allDevices.Count; i++)
            {
                DevicePrint(allDevices[i]);
            }

        }
        
            public void getUDPPackets()
        {
            

            if (allDevices.Count == 0)
            {
                Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            

            PacketDevice selectedDevice = allDevices[comboBox1.SelectedIndex];

            using (PacketCommunicator communicator =
                selectedDevice.Open(65536,
                                PacketDeviceOpenAttributes.Promiscuous, 
                                1000))
            {
                using (BerkeleyPacketFilter filter = communicator.CreateFilter("udp port 6672"))
                {
                    
                    communicator.SetFilter(filter);
                }



                try {
                    communicator.ReceivePackets(0, PacketHandler);
                }
                catch
                {
                    
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Thread thread = new Thread(getUDPPackets) { IsBackground = true };
            thread.Start();

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

