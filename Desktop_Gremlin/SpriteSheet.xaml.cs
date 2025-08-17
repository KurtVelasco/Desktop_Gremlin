using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;


namespace Desktop_Gremlin
{
    public partial class SpriteSheet : Window
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
        private NotifyIcon TRAY_ICON;

        private int FRAME_WIDTH = 300;
        private int FRAME_HEIGHT = 300;
        private int IDLE_FRAME_COUNT = 41;
        private int GRAB_FRAME_COUNT = 154;
        private int WALK_DOWN_FRAME = 17;
        private int WALK_UP_FRAME = 17;
        private int WALK_LEFT_FRAME = 18;
        private int WALK_RIGHT_FRAME = 18;
        private int EMOTE1_FRAME_COUNT = 61;
        private int EMOTE2_FRAME_COUNT = 24;
        private int EMOTE3_FRAME_COUNT = 95;

        private int CURRENT_WALK_DOWN_FRAME = 0;
        private int CURRENT_WALK_UP_FRAME = 0;  
        private int CURRENT_WALK_RIGHT_FRAME = 0;   
        private int CURRENT_WALK_UP_LEFT_FRAME = 0;
        private int CURRENT_IDLE_FRAME = 0;
        private int CURRENT_GRAB_FRAME = 0;
        private int CURRENT_WALK_LEFT_FRAME = 0;
        private int CURRENT_EMOTE_1 = 0;
        private int CURRENT_EMOTE_2 = 0;
        private int CURRENT_EMOTE_3 = 0;

        private int FRAME_RATE = 30;

        private BitmapImage WALK_DOWN_SHEET;
        private BitmapImage IDLE_SHEET;
        private BitmapImage GRAB_SHEET;
        private BitmapImage WALK_UP_SHEET;  
        private BitmapImage WALK_LEFT_SHEET;
        private BitmapImage WALK_RIGHT_SHEET;   
        private BitmapImage EMOTE1_SHEET;
        private BitmapImage EMOTE2_SHEET;
        private BitmapImage EMOTE3_SHEET;   


        private DispatcherTimer CLOSE_TIMER;
        private DispatcherTimer MASTER_TIMER;

        private bool IS_INTRO = true;
        private bool IS_IDLE = true;
        private bool IS_WALKING = false;
        private bool IS_DRAGGING = false;
        private bool LAST_DRAG_STATE = false;
        private bool LAST_STATE_DRAG_OR_WALK = false;
        private bool IS_EMOTING1 = true;
        private bool IS_EMOTING2 = false;
        private bool IS_EMOTING3 = false;

        private bool FOLLOW_CURSOR = false;
        private System.Drawing.Point LAST_CURSOR_POSITION;
        private double FOLLOW_SPEED = 5.0;
        private DateTime LAST_CURSOR_MOVE_TIME;
        private double MOUSE_DELTAX = 0;
        private double MOUSE_DELTAY = 0;

        private Direction CURRENT_DIRECTION;
        private double SPEED = 10.0;
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
        public struct POINT
        {
            public int X;
            public int Y;
        }
        private bool MOVE_LEFT = false;
        private bool MOVE_RIGHT = false;
        private bool MOVE_UP = false;
        private bool MOVE_DOWN = false;
        public SpriteSheet()
        {
            InitializeComponent();
            this.ShowInTaskbar = false; 
            SetupTrayIcon();
            LoadConfig();
            LoadSpritesSheet();
            InitializeAnimations();
        }

        private BitmapImage LoadSprite(string fileName)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sprites", "SpriteSheet", fileName);

            if (!File.Exists(path))
            {
                System.Windows.MessageBox.Show(
                  $"Missing sprite file / Wrong File name Format:\n{fileName}\n\nPath: {path}",
                  "Sprite Load Error",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error
                );
                System.Windows.Application.Current.Shutdown();
            }

            var image = new BitmapImage();
            image.BeginInit();              
            image.UriSource = new Uri(path);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
        private void LoadSpritesSheet()
        {
            IDLE_SHEET = LoadSprite("idle.png");
            GRAB_SHEET = LoadSprite("grab.png");
            WALK_DOWN_SHEET = LoadSprite("run_down.png");
            WALK_UP_SHEET = LoadSprite("run_up.png");
            WALK_LEFT_SHEET = LoadSprite("run_left.png");
            WALK_RIGHT_SHEET = LoadSprite("run_right.png");
            EMOTE1_SHEET = LoadSprite("emote1.png");
            EMOTE2_SHEET = LoadSprite("emote2.png");
            EMOTE3_SHEET = LoadSprite("emote3.png");
        }
        private int PlayAnimationFrame(BitmapImage sheet, int currentFrame, int frameCount, int columns = 5)
        {
            if (sheet == null)
                return currentFrame;

            int x = (currentFrame % columns) * FRAME_WIDTH;
            int y = (currentFrame / columns) * FRAME_HEIGHT;

            if (x + FRAME_WIDTH > sheet.PixelWidth || y + FRAME_HEIGHT > sheet.PixelHeight)
            {
                return currentFrame;
            }

            SpriteImage.Source = new CroppedBitmap(sheet, new Int32Rect(x, y, FRAME_WIDTH, FRAME_HEIGHT));

            return (currentFrame + 1) % frameCount;
        }
       
       private void InitializeAnimations()
{
            MASTER_TIMER = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(FRAME_RATE) };
            MASTER_TIMER.Tick += (s, e) =>
            {

                if (IS_EMOTING1)
                {
                    if (IS_WALKING || IS_DRAGGING || FOLLOW_CURSOR)
                    {
                        IS_EMOTING1 = false;
                    }
                    else
                    {
                        CURRENT_EMOTE_1 = PlayAnimationFrame(EMOTE1_SHEET, CURRENT_EMOTE_1, EMOTE1_FRAME_COUNT);
                        if (CURRENT_EMOTE_1 == 0)
                        {
                            IS_EMOTING1 = false;
                        }
                    }
                }

                // This plays EMOTE2 indefinetely while IS_EMOTING2 == true and interrupts it on other actions
                if (IS_EMOTING2)
                {
                    if (IS_WALKING || IS_DRAGGING || FOLLOW_CURSOR)
                    {
                        IS_EMOTING2 = false;
                    }
                    else
                    {
                        CURRENT_EMOTE_2 = PlayAnimationFrame(EMOTE2_SHEET, CURRENT_EMOTE_2, EMOTE2_FRAME_COUNT);
                    }
                }

                if (!IS_DRAGGING && !IS_WALKING && !IS_EMOTING1 && !FOLLOW_CURSOR && !IS_EMOTING2)
                {
                    CURRENT_IDLE_FRAME = PlayAnimationFrame(IDLE_SHEET, CURRENT_IDLE_FRAME, IDLE_FRAME_COUNT);
                }

                if (IS_DRAGGING)
                {
                    CURRENT_GRAB_FRAME = PlayAnimationFrame(GRAB_SHEET, CURRENT_GRAB_FRAME, GRAB_FRAME_COUNT);
                }

                if (FOLLOW_CURSOR && !IS_DRAGGING)
                {
                    POINT cursorPos;
                    GetCursorPos(out cursorPos);

                    double targetX = cursorPos.X - (FRAME_WIDTH / 2);
                    double targetY = cursorPos.Y - (FRAME_HEIGHT / 2);

                    double dx = targetX - this.Left;
                    double dy = targetY - this.Top;

                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (distance > 50)
                    {
                        dx = (dx / distance) * SPEED;
                        dy = (dy / distance) * SPEED;

                        this.Left += dx;
                        this.Top += dy;

                        if (Math.Abs(dx) > Math.Abs(dy))
                        {
                            if (dx < 0)
                                CURRENT_WALK_LEFT_FRAME = PlayAnimationFrame(WALK_LEFT_SHEET, CURRENT_WALK_LEFT_FRAME, WALK_LEFT_FRAME);
                            else
                                CURRENT_WALK_RIGHT_FRAME = PlayAnimationFrame(WALK_RIGHT_SHEET, CURRENT_WALK_RIGHT_FRAME, WALK_RIGHT_FRAME);
                        }
                        else
                        {
                            if (dy > 0)
                                CURRENT_WALK_DOWN_FRAME = PlayAnimationFrame(WALK_DOWN_SHEET, CURRENT_WALK_DOWN_FRAME, WALK_DOWN_FRAME);
                            else
                                CURRENT_WALK_UP_FRAME = PlayAnimationFrame(WALK_UP_SHEET, CURRENT_WALK_UP_FRAME, WALK_UP_FRAME);
                        }
                    }
                    else
                    {
                        CURRENT_EMOTE_2 = PlayAnimationFrame(EMOTE2_SHEET, CURRENT_EMOTE_2, EMOTE2_FRAME_COUNT);
                    }
                }
                else if (IS_WALKING)
                {
                    MOUSE_DELTAX = 0;
                    MOUSE_DELTAY = 0;

                    if (MOVE_LEFT) MOUSE_DELTAX -= SPEED;
                    if (MOVE_RIGHT) MOUSE_DELTAX += SPEED;
                    if (MOVE_UP) MOUSE_DELTAY -= SPEED;
                    if (MOVE_DOWN) MOUSE_DELTAY += SPEED;

                    if (MOUSE_DELTAX != 0 && MOUSE_DELTAY != 0)
                    {
                        double length = Math.Sqrt(MOUSE_DELTAX * MOUSE_DELTAX + MOUSE_DELTAY * MOUSE_DELTAY);
                        MOUSE_DELTAX = (MOUSE_DELTAX / length) * SPEED;
                        MOUSE_DELTAY = (MOUSE_DELTAY / length) * SPEED;
                    }

                    this.Left += MOUSE_DELTAX;
                    this.Top += MOUSE_DELTAY;

                    if (Math.Abs(MOUSE_DELTAX) > Math.Abs(MOUSE_DELTAY))
                    {
                        if (MOUSE_DELTAX < 0)
                            CURRENT_WALK_LEFT_FRAME = PlayAnimationFrame(WALK_LEFT_SHEET, CURRENT_WALK_LEFT_FRAME, WALK_LEFT_FRAME);
                        else
                            CURRENT_WALK_RIGHT_FRAME = PlayAnimationFrame(WALK_RIGHT_SHEET, CURRENT_WALK_RIGHT_FRAME, WALK_RIGHT_FRAME);
                    }
                    else
                    {
                        if (MOUSE_DELTAY > 0)
                            CURRENT_WALK_DOWN_FRAME = PlayAnimationFrame(WALK_DOWN_SHEET, CURRENT_WALK_DOWN_FRAME, WALK_DOWN_FRAME);
                        else
                            CURRENT_WALK_UP_FRAME = PlayAnimationFrame(WALK_UP_SHEET, CURRENT_WALK_UP_FRAME, WALK_UP_FRAME);
                    }
                }
            };

            MASTER_TIMER.Start();
       }


        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IS_DRAGGING = true;
            DragMove();

            FOLLOW_CURSOR = !FOLLOW_CURSOR;

            // A hack to make sure IDLE animation plays after FOLLOW_CURSOR behaivour is turned off
            if (!FOLLOW_CURSOR)
            {
                IS_EMOTING2 = false;
            }
            else
            {
                IS_EMOTING2 = !IS_EMOTING2;
            }

            IS_DRAGGING = false;
        }

        // Simply toggling on RMB click to signal if we want the EMOTE2 to play
        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IS_EMOTING2 = !IS_EMOTING2;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                MOVE_LEFT = true;
                IS_WALKING = true;
                CURRENT_DIRECTION = Direction.Left;
            }
            else if (e.Key == Key.Right)
            {
                MOVE_RIGHT = true;
                IS_WALKING = true;
                CURRENT_DIRECTION = Direction.Right;
            }
            else if (e.Key == Key.Up)
            {
                MOVE_UP = true;
                IS_WALKING = true;
                CURRENT_DIRECTION = Direction.Up;
            }
            else if (e.Key == Key.Down)
            {
                MOVE_DOWN = true;
                IS_WALKING = true;
                CURRENT_DIRECTION = Direction.Down;
            }
            else if (e.Key == Key.C)
            {
                FOLLOW_CURSOR = !FOLLOW_CURSOR;
            }
        }
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Left) MOVE_LEFT = false;
            if (e.Key == Key.Right) MOVE_RIGHT = false;
            if (e.Key == Key.Up) MOVE_UP = false;
            if (e.Key == Key.Down) MOVE_DOWN = false;
            IS_WALKING = MOVE_LEFT || MOVE_RIGHT || MOVE_DOWN || MOVE_UP;
        }

        private void LoadConfig()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            if (!File.Exists(path))
            {
                System.Windows.MessageBox.Show(
                   "Where the Fuu is the config file!?",
                   "No Config File",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error
               );
            }
               
            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
                    continue;

                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (!int.TryParse(value, out int intValue))
                    continue;

                switch (key.ToUpper())
                {
                    case "FRAME_WIDTH": FRAME_WIDTH = intValue; break;
                    case "FRAME_HEIGHT": FRAME_HEIGHT = intValue; break;
                    case "IDLE_FRAME_COUNT": IDLE_FRAME_COUNT = intValue; break;
                    case "GRAB_FRAME_COUNT": GRAB_FRAME_COUNT = intValue; break;
                    case "WALK_DOWN_FRAME": WALK_DOWN_FRAME = intValue; break;
                    case "WALK_UP_FRAME": WALK_UP_FRAME = intValue; break;
                    case "WALK_LEFT_FRAME": WALK_LEFT_FRAME = intValue; break;
                    case "WALK_RIGHT_FRAME": WALK_RIGHT_FRAME = intValue; break;
                    case "EMOTE1_FRAME_COUNT": EMOTE1_FRAME_COUNT = intValue; break;
                    case "EMOTE2_FRAME_COUNT": EMOTE2_FRAME_COUNT = intValue; break;
                    case "EMOTE3_FRAME_COUNT": EMOTE3_FRAME_COUNT = intValue; break;
                    case "FRAME_RATE": FRAME_RATE = intValue; break;
                }
            }
        }
        private void SetupTrayIcon()
        {
            TRAY_ICON = new NotifyIcon();
            TRAY_ICON.Icon = new Icon("icon.ico");
            TRAY_ICON.Visible = true;
            TRAY_ICON.Text = "Desktop Gremlin";

            var menu = new ContextMenuStrip();
            menu.Items.Add("Reappear", null, (s, e) => ResetApp());
            menu.Items.Add("Explode (Close)", null, (s, e) => CloseApp());


            TRAY_ICON.ContextMenuStrip = menu;
        }
        private void ResetApp()
        {
            TRAY_ICON.Visible = false;
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(exePath);
            System.Windows.Application.Current.Shutdown();
        }

        private void CloseApp()
        {
            MASTER_TIMER.Stop();
            CLOSE_TIMER = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(FRAME_RATE) };
            CLOSE_TIMER.Tick += (s, e) =>
            {
                CURRENT_EMOTE_3 = PlayAnimationFrame(EMOTE3_SHEET, CURRENT_EMOTE_3, EMOTE3_FRAME_COUNT);
                if (CURRENT_EMOTE_3 == 0)
                {
                    CLOSE_TIMER.Stop();
                    TRAY_ICON.Visible = false;
                    TRAY_ICON?.Dispose();
                    System.Windows.Application.Current.Shutdown();
                }
            };
            CLOSE_TIMER.Start();
        }

    }
}
