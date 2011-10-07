using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace CollectData
{
    class HeadsetAudio
    {
        // WaveIn Streams for recording
        WaveIn waveInStream;
        WaveFileWriter writer;
        string outputFilename;
        int deviceNumber = -1;

        public HeadsetAudio(string deviceName, string fileName)
        {
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                deviceInfo.ProductName.Contains(deviceName);
                deviceNumber = waveInDevice;
            }

            if (deviceNumber == -1)
                throw new Exception("Headset device not found");


            outputFilename = fileName;

            waveInStream = new WaveIn();
            waveInStream.DeviceNumber = deviceNumber;
            writer = new WaveFileWriter(outputFilename, new WaveFormat(44100, 2));

            waveInStream.DataAvailable += new EventHandler<WaveInEventArgs>(waveInStream_DataAvailable);
        }

        void waveInStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.WriteData(e.Buffer, 0, e.BytesRecorded);
            int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);

        }
        public void start()
        {
            waveInStream.StartRecording();
        }

        public void stop()
        {
            waveInStream.StopRecording();
            waveInStream.Dispose();
            waveInStream = null;
            writer.Close();
            writer = null;
        }
    }
}
