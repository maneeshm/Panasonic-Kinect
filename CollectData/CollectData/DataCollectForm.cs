using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Research.Kinect.Nui;
using System.Threading;
using Microsoft.Research.Kinect.Audio;
using Coding4Fun.Kinect.WinForm;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Phidgets;
using Phidgets.Events;

namespace CollectData
{
    public partial class DataCollectForm : Form
    {
        Microsoft.Research.Kinect.Nui.Runtime kinect = new Runtime();

        // Store images to avi file
        AviManager aviColorManager;
        VideoStream aviColerStream;
        Boolean firstColorFrame;

        Boolean rawDepth;
        // Open file for reading
        BinaryWriter rawDepthBinaryWriter;
        AviManager aviDepthManager;
        VideoStream aviDepthStream;
        Boolean firstDepthFrame;

        // time and timer
        private DateTime CurrTime;
        private DateTime startTime;
        private int recordLength;
        private int recordCounter = 0;
        private System.Windows.Forms.Timer timer;

        private static String prefix;

        //Thread recordAudioThread;
        private static Boolean audioRecord;

        // GPS
        ReadGPS readGPS;

        // Kinect Audio
        System.Diagnostics.Process kinectAudioRecordProcess;

        // Headset Audio
        private TextWriter tw;
        HeadsetAudio headsetAudio;

        // Phidgets Sensor
        private Boolean phidgets;
        private Spatial spatial;
        private TextWriter twPhidgetTimestamp;

        // Writer
        private TextWriter twColorTimestamp;
        private TextWriter twDepthTimestamp;

        public DataCollectForm()
        {
            InitializeComponent();
            kinect.Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseDepth);

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartButton.Enabled = false;
            try
            {
                recordLength = System.Convert.ToInt32(timeTextBox.Text.ToString());
            }
            catch (Exception _Exception)
            {
                messageTextBox.Text = _Exception.ToString();
                return;
            }

            if (recordLength <= 0)
            {
                messageTextBox.Text = "Please input a valid record time";
                return;
            }

            DateTime CurrTime = DateTime.Now;
            String format = "MMM-ddd-d-HH-mm-ss-yyyy";
            prefix = CurrTime.ToString(format);
            String newPath = System.IO.Path.Combine(prefix);
            System.IO.Directory.CreateDirectory(newPath);

            // Phidgets Sensor
            phidgets = phidgetsCheckBox.Checked;
            if (phidgets)
            {
                // Phidgets Sensor
                spatial = new Spatial();
                //Hook the basic event handlers
                spatial.Attach += new AttachEventHandler(accel_Attach);
                spatial.Detach += new DetachEventHandler(accel_Detach);
                spatial.Error += new Phidgets.Events.ErrorEventHandler(accel_Error);

                //hook the phidget specific event handlers
                spatial.SpatialData += new SpatialDataEventHandler(spatial_SpatialData);

                //open the acclerometer object for device connections
                spatial.open();

                //get the program to wait for an spatial device to be attached
                messageTextBox.Text += "Waiting for spatial to be attached....";

                spatial.waitForAttachment();

                //Set the data rate so the events aren't crazy
                spatial.DataRate = 496; //multiple of 8

                String phidgetTimestamp = System.IO.Path.Combine(newPath, prefix + "-phidget.txt");
                twPhidgetTimestamp = new StreamWriter(phidgetTimestamp);
            }

            // GPS
            if (gpsCheckBox.Checked == true)
            {
                String gpsFileName = System.IO.Path.Combine(newPath, prefix + "-gps.txt");
                readGPS = new ReadGPS("COM" + gpsCom.Text, gpsFileName);
                readGPS.start();
            }

            // Color
            firstColorFrame = true;
            String colorFileName = System.IO.Path.Combine(newPath, prefix + "-color.avi");
            String colorTimestamp = System.IO.Path.Combine(newPath, prefix + "-color.txt");
            aviColorManager = new AviManager(colorFileName, false);
            twColorTimestamp = new StreamWriter(colorTimestamp);

            // Depth
            rawDepth = rawDepthCheckBox.Checked;
            firstDepthFrame = true;
            String depthFileName;
            if (rawDepth)
            {
                depthFileName = System.IO.Path.Combine(newPath, prefix + "-depth.data");
                rawDepthBinaryWriter = new BinaryWriter(File.OpenWrite(depthFileName));
            }
            else
            {
                depthFileName = System.IO.Path.Combine(newPath, prefix + "-depth.avi");
                aviDepthManager = new AviManager(depthFileName, false);
            }
            String depthTimestamp = System.IO.Path.Combine(newPath, prefix + "-depth.txt");
            twDepthTimestamp = new StreamWriter(depthTimestamp);

            kinect.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinect_VideoFrameReady);
            kinect.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinect_DepthFrameReady);

            kinect.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            kinect.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution640x480, ImageType.Depth);

            // Kinect Audio
            String kinectAudioFileName = System.IO.Path.Combine(newPath, prefix + "-kinect-audio.wav");
            String kinectAudioTimestamp = System.IO.Path.Combine(newPath, prefix + "-kinect-audio.txt");
            kinectAudioRecordProcess = new System.Diagnostics.Process();
            //p.StartInfo.WorkingDirectory = @"C:\whatever";
            kinectAudioRecordProcess.StartInfo.FileName = @"AudioCaptureRaw.exe";
            kinectAudioRecordProcess.StartInfo.CreateNoWindow = true;
            kinectAudioRecordProcess.StartInfo.Arguments = recordLength + " " + kinectAudioFileName + " " + kinectAudioTimestamp;
            kinectAudioRecordProcess.Start();

            // Headset Audio
            if (headsetAudioCheckBox.Checked == true)
            {
                String headsetAudioFileName = System.IO.Path.Combine(newPath, prefix + "-headset-audio.wav");
                String headsetAudioTimestamp = System.IO.Path.Combine(newPath, prefix + "-headset-audio.txt");
                headsetAudio = new HeadsetAudio("Shure Digital", headsetAudioFileName);
                tw = new StreamWriter(headsetAudioTimestamp);
                CurrTime = DateTime.Now;
                tw.WriteLine(CurrTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
                headsetAudio.start();
                tw.Close();
            }

            this.startTime = DateTime.Now;
            messageTextBox.Text = "Started";

            this.timer = new System.Windows.Forms.Timer();
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // one channel audio record
            //audioRecord = true;
            //recordAudioThread = new Thread(new ThreadStart(recordAudio));
            //recordAudioThread.Start();
        }

        private void stop()
        {
            if (gpsCheckBox.Checked == true)
            {
                readGPS.stop();
            }
            if (headsetAudioCheckBox.Checked == true)
            {
                headsetAudio.stop();
            }
            kinect.VideoFrameReady -= kinect_VideoFrameReady;
            kinect.DepthFrameReady -= kinect_DepthFrameReady;
            kinect.Uninitialize();
            aviColorManager.Close();
            if (rawDepth)
            {
                rawDepthBinaryWriter.Close();
            }
            else
            {
                aviDepthManager.Close();
            }
            if (phidgets)
            {
                spatial.SpatialData -= spatial_SpatialData;
                spatial.Attach -= accel_Attach;
                spatial.Detach -= accel_Detach;
                spatial.Error -= accel_Error;

                //spatial = null;
                twPhidgetTimestamp.Flush();

            }
            twColorTimestamp.Flush();
            twDepthTimestamp.Flush();
            messageTextBox.Text = "Done";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            twColorTimestamp.Close();
            twDepthTimestamp.Close();
            twPhidgetTimestamp.Close();
            Environment.Exit(0);
        }

        // Record Orientation

        //spatial data handler - all spatial data tied together.
        void spatial_SpatialData(object sender, SpatialDataEventArgs e)
        {
            CurrTime = DateTime.Now;
            twPhidgetTimestamp.WriteLine(CurrTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
            twPhidgetTimestamp.WriteLine("SpatialData event time:" + e.spatialData[0].Timestamp.TotalSeconds.ToString());
            if (e.spatialData[0].Acceleration.Length > 0)
                twPhidgetTimestamp.WriteLine(" Acceleration: " + e.spatialData[0].Acceleration[0] + ", " + e.spatialData[0].Acceleration[1] + ", " + e.spatialData[0].Acceleration[2]);
            if (e.spatialData[0].AngularRate.Length > 0)
                twPhidgetTimestamp.WriteLine(" Angular Rate: " + e.spatialData[0].AngularRate[0] + ", " + e.spatialData[0].AngularRate[1] + ", " + e.spatialData[0].AngularRate[2]);
            if (e.spatialData[0].MagneticField.Length > 0)
                twPhidgetTimestamp.WriteLine(" Magnetic Field: " + e.spatialData[0].MagneticField[0] + ", " + e.spatialData[0].MagneticField[1] + ", " + e.spatialData[0].MagneticField[2]);
        }

        //Attach event handler...Display the serial number of the attached 
        //spatial to the console
        void accel_Attach(object sender, AttachEventArgs e)
        {
            messageTextBox.Text += "Spatial " + e.Device.SerialNumber.ToString() + " attached!";
        }

        //Detach event handler...Display the serial number of the detached spatial
        //to the console
        void accel_Detach(object sender, DetachEventArgs e)
        {
            messageTextBox.Text += "Spatial " + e.Device.SerialNumber.ToString() + " detached!";
        }

        //Error event handler...Display the description of the error to the console
        void accel_Error(object sender, Phidgets.Events.ErrorEventArgs e)
        {
            messageTextBox.Text += e.Description.ToString();
        }

        // timer
        private void timer_Tick(object sender, EventArgs e)
        {
            int timeLeft = recordLength - recordCounter;
            Time.Text = timeLeft.ToString();
            if (timeLeft == 0)
            {
                this.timer.Tick -= this.timer_Tick;
                stop();
                return;
            }
            recordCounter++;
        }


        void kinect_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            CurrTime = DateTime.Now;
            twColorTimestamp.WriteLine(CurrTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
            Bitmap bitmap = e.ImageFrame.ToBitmap();
            if (previewCheckBox.Checked == true)
                preview.Image = bitmap;

            if (firstColorFrame)
            {
                aviColerStream = aviColorManager.AddVideoStream(false, 30, bitmap);
                firstColorFrame = false;
            }
            else
            {
                aviColerStream.AddFrame(bitmap);
            }
        }

        void kinect_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            CurrTime = DateTime.Now;
            twDepthTimestamp.WriteLine(CurrTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));

            //PlanarImage Image = e.ImageFrame.Image;
            //byte[] convertedDepthFrame = convertDepthFrame(Image.Bits);

            //Bitmap bitmap = new Bitmap(Image.Width, Image.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            ////Create a BitmapData and Lock all pixels to be written 
            //BitmapData bmpData = bitmap.LockBits(
            //                     new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            //                     ImageLockMode.WriteOnly, bitmap.PixelFormat);

            //Marshal.Copy(convertedDepthFrame, 0, bmpData.Scan0, convertedDepthFrame.Length);

            //bitmap.UnlockBits(bmpData);

            if (rawDepth)
            {
                PlanarImage Image = e.ImageFrame.Image;
                rawDepthBinaryWriter.Write(Image.Bits);
                rawDepthBinaryWriter.Flush();
            }
            else
            {
                Bitmap bitmap = e.ImageFrame.ToBitmap();
                if (firstDepthFrame)
                {
                    aviDepthStream = aviDepthManager.AddVideoStream(false, 30, bitmap);
                    firstDepthFrame = false;
                }
                else
                {
                    aviDepthStream.AddFrame(bitmap);
                }
            }
        }



        // deprecated code


        // allocate memory for depth image (converted from kinect PlanarImage)
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;
        byte[] depthFrame32 = new byte[320 * 240 * 4];

        //Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        //that displays different players in different colors
        byte[] convertDepthFrame(byte[] depthFrame16)
        {
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
            {
                //int player = depthFrame16[i16] & 0x07;
                int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 2);
            }
            return depthFrame32;
        }



        // one channel audio record

        static void recordAudio()
        {
            var buffer = new byte[4096];
            int recordTime = 60; //max seconds
            int recordingLength = recordTime * 2 * 16000; //10 seconds, 16 bits per sample, 16khz
            string outputFileName = prefix + "-audio.wav";

            //We need to run in high priority to avoid dropping samples 
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            //Instantiate the KinectAudioSource to do audio capture
            using (var source = new KinectAudioSource())
            {
                source.SystemMode = SystemMode.OptibeamArrayOnly;

                //Register for beam tracking change notifications
                source.BeamChanged += source_BeamChanged;

                using (var fileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    WriteWavHeader(fileStream, recordingLength);

                    Console.WriteLine("Recording for {0} seconds", recordTime);

                    //Start capturing audio                               
                    using (var audioStream = source.Start())
                    {
                        //Simply copy the data from the stream down to the file
                        int count, totalCount = 0;
                        while ((count = audioStream.Read(buffer, 0, buffer.Length)) > 0 && totalCount < recordingLength && audioRecord)
                        {
                            fileStream.Write(buffer, 0, count);
                            totalCount += count;

                            //If we have high confidence, print the position
                            if (source.SoundSourcePositionConfidence > 0.9)
                                Console.Write("Sound source position (radians): {0}\t\tBeam: {1}\r", source.SoundSourcePosition, source.MicArrayBeamAngle);
                        }
                    }
                }

                Console.WriteLine("Recording saved to {0}", outputFileName);
            }
        }

        static void source_BeamChanged(object sender, BeamChangedEventArgs e)
        {
            Console.WriteLine("\nBeam direction changed (radians): {0}", e.Angle);
        }



        /// <summary>
        /// A bare bones WAV file header writer
        /// </summary>        
        static void WriteWavHeader(Stream stream, int dataLength)
        {
            //We need to use a memory stream because the BinaryWriter will close the underlying stream when it is closed
            using (MemoryStream memStream = new MemoryStream(64))
            {
                int cbFormat = 18; //sizeof(WAVEFORMATEX)
                WAVEFORMATEX format = new WAVEFORMATEX()
                {
                    wFormatTag = 1,
                    nChannels = 1,
                    nSamplesPerSec = 16000,
                    nAvgBytesPerSec = 32000,
                    nBlockAlign = 2,
                    wBitsPerSample = 16,
                    cbSize = 0
                };

                using (var bw = new BinaryWriter(memStream))
                {
                    //RIFF header
                    WriteString(memStream, "RIFF");
                    bw.Write(dataLength + cbFormat + 4); //File size - 8
                    WriteString(memStream, "WAVE");
                    WriteString(memStream, "fmt ");
                    bw.Write(cbFormat);

                    //WAVEFORMATEX
                    bw.Write(format.wFormatTag);
                    bw.Write(format.nChannels);
                    bw.Write(format.nSamplesPerSec);
                    bw.Write(format.nAvgBytesPerSec);
                    bw.Write(format.nBlockAlign);
                    bw.Write(format.wBitsPerSample);
                    bw.Write(format.cbSize);

                    //data header
                    WriteString(memStream, "data");
                    bw.Write(dataLength);
                    memStream.WriteTo(stream);
                }
            }
        }

        static void WriteString(Stream stream, string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
        }

        struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }
    }
}
