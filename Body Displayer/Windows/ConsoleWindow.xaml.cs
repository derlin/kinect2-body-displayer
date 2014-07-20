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
using System.Windows.Shapes;
using Kinect2.BodyDisplayer.tools;

namespace Kinect2.BodyDisplayer.gui
{
    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        /// <summary>
        /// Object used to redirect the content of the console
        /// onto the textbox
        /// </summary>
        private TextBoxOutputter outputter;

        private string OnOpenMessage = "Console started\nUse Console.Write wherever in your code to display text into this Window.";
        public ConsoleWindow()
        {
            InitializeComponent();
            outputter = new TextBoxOutputter(ConsoleBox);
            Console.SetOut(outputter);
            Console.WriteLine(OnOpenMessage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // clear the console
            ConsoleBox.Text = "";
        }
    }
}
