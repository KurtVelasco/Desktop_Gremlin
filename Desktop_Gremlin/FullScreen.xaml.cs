using System;
using System.Configuration;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Desktop_Gremlin
{
    /// <summary>
    /// Interaction logic for FullScreen.xaml
    /// </summary>
    public partial class FullScreen : Window
    {
        private DispatcherTimer _jumpTimer;
        public FullScreen()
        {
            InitializeComponent();
           
        }
        private int Playanimation(string sheetName)
        {
            BitmapImage sheet = SpriteManager.Get(sheetName);
            if (sheet == null)
            {
                return 0;
            }   
            int x = 0;  
            int y = 0;  


            if(x+ Settings.)
        }


      

        
    }
}
