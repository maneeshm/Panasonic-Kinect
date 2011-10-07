using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CollectData
{
    class ReadGPS
    {

        // Local variables used to hold the present
        // position as latitude and longitude
        public string Latitude;
        public string Longitude;

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;

        private string fileName = "temp.txt";
        private string portName = "COM3";
        private TextWriter tw;

        public ReadGPS(string pName, string fName)
        {
            this.portName = pName;
            this.fileName = fName;

            this.serialPort1 = new System.IO.Ports.SerialPort();
            this.timer1 = new System.Windows.Forms.Timer();
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 4800;

            this.serialPort1.Close();
            this.serialPort1.PortName = this.portName;
            this.serialPort1.Open();

            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 100;
        }

        public void start()
        {
            tw = new StreamWriter(fileName);
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
        }

        public void stop()
        {
            tw.Close();
            this.timer1.Tick -= this.timer1_Tick;
        }

        /// <summary>
        /// Try to update present position if the port is configured correctly
        /// and the GPS device is returning values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                string data = serialPort1.ReadExisting();
                string[] strArr = data.Split('$');
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strTemp = strArr[i];
                    string[] lineArr = strTemp.Split(',');
                    if (lineArr[0] == "GPGGA")
                    {

                        try
                        {
                            //Latitude
                            Double dLat = Convert.ToDouble(lineArr[2]);
                            dLat = dLat / 100;
                            string[] lat = dLat.ToString().Split('.');
                            Latitude = lineArr[3].ToString() + lat[0].ToString() + "." + ((Convert.ToDouble(lat[1]) / 60)).ToString("#######");

                            //Longitude
                            Double dLon = Convert.ToDouble(lineArr[4]);
                            dLon = dLon / 100;
                            string[] lon = dLon.ToString().Split('.');
                            Longitude = lineArr[5].ToString() + lon[0].ToString() + "." + ((Convert.ToDouble(lon[1]) / 60)).ToString("#######");

                            //Display
                            DateTime CurrTime = DateTime.Now;
                            tw.WriteLine(CurrTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
                            tw.WriteLine("Latitude: " + Latitude);
                            tw.WriteLine("Longitude: " + Longitude);
                            tw.WriteLine();
                        }
                        catch
                        {
                            DateTime CurrTime = DateTime.Now;
                            tw.WriteLine(CurrTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
                            tw.WriteLine("GPS Unavailable");
                            tw.WriteLine();
                        }
                    }
                }

            }
            else
            {
                DateTime CurrTime = DateTime.Now;
                tw.WriteLine(CurrTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
                tw.WriteLine("COM Port Closed");
                tw.WriteLine();
            }
        }
    }
}
