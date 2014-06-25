using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Threading;

namespace guiTest {
    public partial class MainWindow : Window {
        KinectSensor sensor;
        byte[] colorPixelData;
        WriteableBitmap outPutImage;
        List<Patient> patients = new List<Patient>();
        EventLog eventLog;
        public MainWindow() {
            InitializeComponent();
        }
        private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e) {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame()) {
                if (colorFrame != null) {
                    colorPixelData = new byte[colorFrame.PixelDataLength];
                    colorFrame.CopyPixelDataTo(colorPixelData);
                    outPutImage = new WriteableBitmap(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                    outPutImage.WritePixels(new Int32Rect(0, 0, colorFrame.Width, colorFrame.Height), colorPixelData, colorFrame.Width * 4, 0);
                    kinectImage.Source = outPutImage;
                }
            }
            time.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e) {
            try {
                sensor = KinectSensor.KinectSensors[0];
            }
            catch {
                MessageBox.Show("No kinect found");
            }
            for (int i = 0; i < KinectSensor.KinectSensors.Count; i++) {
                patients.Add(new Patient(i, 2000 + i, "Patient Name"));
                kinectChooser.Items.Add("Room " + patients[i].roomNumber);
            }
            privacyMode.Items.Add("Off");
            privacyMode.Items.Add("15 minutes");
            privacyMode.Items.Add("30 minutes");
            privacyMode.Items.Add("1 hour");
            privacyMode.Items.Add("2 hours");
            privacyMode.SelectedIndex = 0;
            UpdateInterfacePatientInfo();
            sensor.ColorFrameReady += sensor_ColorFrameReady;
            sensor.ColorStream.Enable();
            sensor.Start();
        }
        private void UpdateInterfacePatientInfo() {
            PatientName.Text = patients[kinectChooser.SelectedIndex].name;
            RoomNumber.Text = "Room " + patients[kinectChooser.SelectedIndex].roomNumber;
            nameEdit.Text = patients[kinectChooser.SelectedIndex].name;
            roomEdit.Text = ""+patients[kinectChooser.SelectedIndex].roomNumber;
        }
        private void kinectChooser_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                sensor.Stop();
                sensor = KinectSensor.KinectSensors[kinectChooser.SelectedIndex];
                sensor.ColorFrameReady += sensor_ColorFrameReady;
                sensor.ColorStream.Enable();
                sensor.Start();
                UpdateInterfacePatientInfo();
            }
            catch {

            }
        }
        private void Options_Click(object sender, RoutedEventArgs e) {
            if (!OptionsWindow.IsEnabled) {
                OptionsWindow.IsEnabled = true;
                OptionsWindow.Visibility = System.Windows.Visibility.Visible;
                Options.IsEnabled = false;
                Options.Visibility = System.Windows.Visibility.Hidden;
                for (int i = 0; i < 500; i++) {
                    OptionsWindow.Margin = new Thickness(0, 500 - i, 0, 0);
                }
            }
        }
        private void cancel_Click(object sender, RoutedEventArgs e) {
            //OptionsWindow.IsEnabled = false;
            //OptionsWindow.Visibility = System.Windows.Visibility.Hidden;
            Options.IsEnabled = true;
            Options.Visibility = System.Windows.Visibility.Visible;
            OptionsWindow.IsEnabled = false;
        }
        private void Update_Click(object sender, RoutedEventArgs e) {
            patients[kinectChooser.SelectedIndex].name = nameEdit.Text;
            patients[kinectChooser.SelectedIndex].roomNumber = int.Parse(roomEdit.Text);
            UpdateInterfacePatientInfo();
            int temp = kinectChooser.SelectedIndex;
            kinectChooser.Items[kinectChooser.SelectedIndex] = "Room " + roomEdit.Text;
            kinectChooser.SelectedIndex = temp;
            MessageBox.Show("Successfully update patient info");
        }

        private void dispLog_Checked(object sender, RoutedEventArgs e) {
            eventLog = new EventLog();
            eventLog.Activate();
            eventLog.Visibility = System.Windows.Visibility.Visible;
            eventLog.IsEnabled = true;
            eventLog.events.Items.Add(DateTime.Now.ToString());
        }

        private void dispLog_Unchecked(object sender, RoutedEventArgs e) {
            eventLog.IsEnabled = false;
            eventLog.Visibility = System.Windows.Visibility.Hidden;
            eventLog.Close();
            eventLog = null;
        }

        private void close_Click(object sender, RoutedEventArgs e) {
            try {
                eventLog.Close();
            }
            catch{

            }
            Close();
        }
              
    }
}
