using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Kinect2.BodyDisplayer.tools;

namespace Kinect2.BodyDisplayer.gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// Just open the streams and display the camera image and the 
    /// bodies in filigrane.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region members

        #region status text

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind 
        /// to changeable data (statusText, combo, ...)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get { return this.statusText; }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;
                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));

                }
            }
        }

        #endregion/// <summary>

        #region kinect and skeleton drawing

        /// In charge of drawing the skeleton on the MainWindow canvas
        /// </summary>
        BodyDrawerManager skeletonDrawerManager = null;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Reader for kinect frames (color, depth, ...)
        /// </summary>
        private MultiSourceFrameReader reader = null;

        /// <summary>
        /// Detected skeletons 
        /// </summary>
        private IList<Body> bodies;

        #endregion

        #region console window

        #endregion

        private ConsoleWindow consoleWindow = null;

        #endregion

        #region constructor
        public MainWindow()
        {

            this.Loaded += _Loaded;

            this.Closing += _Closing;

            this.InitializeComponent();
        }

        #endregion

        #region setup
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            // use the window object as the view model
            this.DataContext = this;

            // for Alpha, one sensor is supported
            this.kinectSensor = KinectSensor.Default;

            if (this.kinectSensor != null)
            {
                // open the sensor
                this.kinectSensor.Open();

                // open all the available kinect streams
                this.reader = this.kinectSensor.OpenMultiSourceFrameReader(
                    FrameSourceTypes.Color |
                    FrameSourceTypes.Depth |
                    FrameSourceTypes.Infrared |
                    FrameSourceTypes.Body
                );

                // allocate the space for bodies
                this.bodies = new Body[this.kinectSensor.BodyFrameSource.BodyCount];

                // create the skeleton drawer manager
                this.skeletonDrawerManager = new BodyDrawerManager(kinectSensor);
                bodyDrawingImage.Source = skeletonDrawerManager.BodyDrawingImage; // this line is for both versions

                // attach the event handler for processing streams
                this.reader.MultiSourceFrameArrived += this.Reader_MultiSourceFrameArrived;

                Console.WriteLine("Kinect initialised.");
            }
            else
            {
                // on failure, set the status text
                StatusText = "Could not initialize sensor. \nPlease check that the sensor is plugged and the KinectService is running";
            }

        }
        #endregion

        #region KinectSensor: frames ready handlers
        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // update the camera image
                    cameraDisplayImage.Source = frame.ToBitmap();
                }
            } // end color


            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // copy the info to the buffer
                    frame.GetAndRefreshBodyData(this.bodies);

                    if (skeletonDrawerManager != null)
                    {
                        // draw the skeletons
                        skeletonDrawerManager.DrawBodies(bodies);
                    }
                }
            } // end body
        }
        #endregion

        #region console window
        private void _Console_Button_Click(object sender, RoutedEventArgs e)
        {
            if (consoleWindow == null)
            {
                consoleWindow = new ConsoleWindow();

                CancelEventHandler hideOnCloseHandler = (s, we) =>
                {
                    consoleWindow.Hide();
                    we.Cancel = true;
                };

                // hide the console on close
                consoleWindow.Closing += hideOnCloseHandler;
                // if the main window is closing, be sure that the console one will also exit !
                this.Closing += (s, we) =>
                {
                    consoleWindow.Closing -= hideOnCloseHandler;
                    consoleWindow.Close();
                };

                consoleWindow.Show();
            }
            else
            {
                // already created, just show it
                if (consoleWindow.WindowState == System.Windows.WindowState.Minimized)
                    consoleWindow.WindowState = System.Windows.WindowState.Normal;
                consoleWindow.Show();
            }
        }
        #endregion

        #region on close - cleaning

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        private void _Closing(object sender, CancelEventArgs e)
        {
            // close the reader
            if (this.reader != null)
            {
                // Reader is IDisposable
                this.reader.Dispose();
                this.reader = null;
            }

            // dispose the bodies
            if (this.bodies != null)
            {
                // Body is IDisposable
                foreach (Body body in this.bodies)
                {
                    if (body != null)
                    {
                        body.Dispose();
                    }
                }
            }

            // stop the sensor
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        #endregion
    }
}
