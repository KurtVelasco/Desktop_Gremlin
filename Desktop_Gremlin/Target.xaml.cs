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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Koyuki
{
    public partial class Target : System.Windows.Window
    {
        public Target()
        {
            InitializeComponent();
            ImageInitialize();
            this.MouseLeftButtonDown += Target_MouseLeftButtonDown;
            this.Height = Settings.ItemHeight;
            this.Width = Settings.ItemWidth;
            this.ShowInTaskbar = Settings.ShowTaskBar;

            if (Settings.FakeTransparent)
            {
                this.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#01000000"));
            }

        }
        private void ImageInitialize()
        {
            Random rng = new Random();
            int index = rng.Next(1, 3);
            string fileName = $"food{index}.png";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "SpriteSheet", "Misc", fileName);

            SpriteFood.Source = new BitmapImage(new Uri(path));
        }
        private void Target_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); 
        }    
        public Point GetCenter()
        {
            return new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
        }

    }
}
