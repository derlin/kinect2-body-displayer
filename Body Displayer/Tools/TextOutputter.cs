using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Kinect;
using System.Text;
using System.Windows.Controls;

namespace Kinect2.BodyDisplayer.tools
{
    /// <summary>
    /// Class used to intercept all the messages printed to the console
    /// and display them in a graphical textbox.
    /// </summary>
    public class TextBoxOutputter : TextWriter
    {
        TextBox textBox = null;

        public TextBoxOutputter(TextBox output)
        {
            textBox = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                textBox.AppendText(value.ToString());
                textBox.ScrollToEnd();
            }));
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
