using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Chain3D
{
    public partial class MainFrm : Form
    {
        public static MainFrm Instance { get; private set; }

        #region GameValues

        public Location Bonus { get; set; }

        public bool BonusCatched { get; set; }

        public Direction WalkingDirection = Direction.DirectionY;

        public int Xmax = 50;

        public int Ymax = 50;

        public int Zmax = 50;

        public int ControlLookupTimeout = 100;

        public int SnakeSpeed = 200;

        public bool CancelRequest = false;

        #endregion

        public List<Location> Snake = new List<Location>();

        public Dictionary<String, Thread> runningThreads = new Dictionary<String, Thread>();

        #region Constant

        public const String LookupThreadName = "Lookup";

        public const String RefreshThreadName = "Refresh";

        public const String WalkThreadName = "Walk";

        public const String BonusThreadName = "Bonus";

        #endregion

        public MainFrm()
        {
            InitializeComponent();
            Instance = this;
            Snake.Add(new Location(25, 25, 26));
            Snake.Add(new Location(25, 25, 25));
            Snake.Add(new Location(25, 25, 24));
            AppendDelegates();
        }

        #region Draw

        private void DrawTopView(Graphics gra, Rectangle area)
        {
            System.Drawing.Size pixelSize = new Size(area.Width / Xmax, area.Height / Zmax);
            double zJump = ((double)area.Height) / ((double)Zmax);
            double xJump = ((double)area.Width) / ((double)Xmax);
            lock (Snake)
            {
                foreach (Location cur in Snake)
                {
                    gra.DrawRectangle(Pens.Black, new Rectangle(new Point((int)(xJump * cur.X), area.Height - (int)(zJump * cur.Z)), pixelSize));
                }
            }
            if (Bonus != null)
            {
                lock (Bonus)
                {

                    gra.DrawRectangle(Pens.Red, new Rectangle(new Point((int)(xJump * Bonus.X), area.Height - (int)(zJump * Bonus.Z)), pixelSize));
                }
            }
        }

        private void DrawFrontView(Graphics gra, Rectangle area)
        {
            System.Drawing.Size pixelSize = new Size(area.Width / Xmax, area.Height / Ymax);
            double yJump = ((double)area.Height) / ((double)Ymax);
            double xJump = ((double)area.Width) / ((double)Xmax);
            lock (Snake)
            {
                foreach (Location cur in Snake)
                {
                    gra.DrawRectangle(Pens.Black, new Rectangle(new Point((int)(xJump * cur.X), area.Height - (int)(yJump * cur.Y)), pixelSize));
                }
            }
            if (Bonus != null)
            {
                lock (Bonus)
                {
                    gra.DrawRectangle(Pens.Red, new Rectangle(new Point((int)(xJump * Bonus.X), area.Height - (int)(yJump * Bonus.Y)), pixelSize));
                }
            }
        }

        #endregion

        private void AppendDelegates()
        {
            topPnt.DrawSpecial = new Draw(DrawTopView);
            frontPnt.DrawSpecial = new Draw(DrawFrontView);
        }

        #region Threaded methods

        private void BonusLookup()
        {
            while (!MainFrm.Instance.CancelRequest)
            {
                Thread.Sleep(MainFrm.Instance.SnakeSpeed);
                if (MainFrm.Instance.Bonus == null)
                {
                    MainFrm.Instance.Bonus = GetNewLocation();
                }
                lock (MainFrm.Instance.Snake)
                {
                    foreach (Location curLoc in MainFrm.Instance.Snake)
                    {
                        if (MainFrm.Instance.CancelRequest)
                        {
                            ClearThread(Thread.CurrentThread.Name);
                            return;
                        }
                        if (curLoc.Equals(MainFrm.Instance.Bonus))
                        {
                            MainFrm.Instance.Bonus = GetNewLocation();
                            MainFrm.Instance.BonusCatched = true;
                        }
                    }
                }

            }
            ClearThread(Thread.CurrentThread.Name);
        }

        private Location GetNewLocation()
        {
            Random rand = new Random();
            return new Location(rand.Next(Xmax), rand.Next(Ymax), rand.Next(Zmax));
        }

        private void Walk()
        {
            while (!MainFrm.Instance.CancelRequest)
            {
                Thread.Sleep(MainFrm.Instance.SnakeSpeed);
                lock (MainFrm.Instance.Snake)
                {
                    Location loc = MainFrm.Instance.Snake[0];
                    switch (MainFrm.Instance.WalkingDirection)
                    {
                        case Direction.DirectionX:
                            MainFrm.Instance.Snake.Insert(0, new Location(loc.X + 1, loc.Y, loc.Z));
                            break;
                        case Direction.DirectionY:
                            MainFrm.Instance.Snake.Insert(0, new Location(loc.X, loc.Y + 1, loc.Z));
                            break;
                        case Direction.DirectionZ:
                            MainFrm.Instance.Snake.Insert(0, new Location(loc.X, loc.Y, loc.Z + 1));
                            break;
                        case Direction.AntiDirectionX:
                            MainFrm.Instance.Snake.Insert(0, new Location(loc.X - 1, loc.Y, loc.Z));
                            break;
                        case Direction.AntiDirectionY:
                            MainFrm.Instance.Snake.Insert(0, new Location(loc.X, loc.Y - 1, loc.Z));
                            break;
                        case Direction.AntiDirectionZ:
                            MainFrm.Instance.Snake.Insert(0, new Location(loc.X, loc.Y, loc.Z - 1));
                            break;
                    }
                    if (!MainFrm.Instance.BonusCatched)
                    {
                        MainFrm.Instance.Snake.RemoveAt(MainFrm.Instance.Snake.Count - 1);
                    }
                    else
                    {
                        MainFrm.Instance.BonusCatched = false;
                    }
                }

            }
            ClearThread(Thread.CurrentThread.Name);
        }

        private void ClearThread(String readyThreadName)
        {
            runningThreads.Remove(readyThreadName);
        }

        private void Lookup()
        {
            while (!MainFrm.Instance.CancelRequest)
            {
                Thread.Sleep(MainFrm.Instance.ControlLookupTimeout);
                lock (MainFrm.Instance.Snake)
                {
                    int outer = 0;
                    foreach (Location curOuter in MainFrm.Instance.Snake)
                    {
                        int inner = 0;
                        foreach (Location curInner in MainFrm.Instance.Snake)
                        {
                            if (MainFrm.Instance.CancelRequest)
                            {
                                ClearThread(Thread.CurrentThread.Name);
                                return;
                            }
                            if (outer != inner && curOuter.Equals(curInner))
                            {
                                MainFrm.Instance.GameOver("You cut yourself");
                            }
                            inner++;
                        }
                        outer++;
                    }
                }
            }
            ClearThread(Thread.CurrentThread.Name);
        }

        #endregion

        internal void GameOver(String message, Location loc)
        {
            GameOver(message);
        }

        private void GameOver(string message)
        {
            CancelRequest = true;
            MessageBox.Show(message, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Start()
        {
            if (runningThreads.Count > 0)
            {
                this.CancelRequest = true;
                while (runningThreads.Count > 0)
                {
                    Thread.Sleep(50);
                }
                this.CancelRequest = false;
            }
            //bonus
            Thread bonus = new Thread(new ThreadStart(BonusLookup));
            bonus.Name = BonusThreadName;
            bonus.Start();
            runningThreads.Add(BonusThreadName, bonus);
            //lookup
            Thread lookup = new Thread(new ThreadStart(Lookup));
            lookup.Name = LookupThreadName;
            lookup.Start();
            runningThreads.Add(LookupThreadName, lookup);
            //refresh
            Thread refresh = new Thread(new ThreadStart(delegate()
            {
                while (!MainFrm.Instance.CancelRequest)
                {
                    Thread.Sleep(50);
                    MainFrm.Instance.Invoke(new MethodInvoker(Refresh));
                }
                ClearThread(Thread.CurrentThread.Name);
            }));
            refresh.Name = RefreshThreadName;
            refresh.Start();
            runningThreads.Add(RefreshThreadName, refresh);
            //walk
            Thread walk = new Thread(new ThreadStart(Walk));
            walk.Name = WalkThreadName;
            walk.Start();
            runningThreads.Add(WalkThreadName, walk);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.CancelRequest = true;
            if (runningThreads.Count > 0)
            {//warte dialog anzeigen
                e.Cancel = true;
                WaitDialog.Show(this, "Wird geschlossen...", new WaitDialog.IsExcecutionReady(delegate()
                {
                    return runningThreads.Count == 0;
                }), new WaitDialog.CallWhenReady(delegate()
                {
                    this.Close();
                }));
            }
        }

        private void HandleKey(Keys p)
        {
            switch (p)
            {
                case Keys.A:
                case Keys.Left:
                    WalkingDirection = Direction.AntiDirectionX;
                    break;
                case Keys.D:
                case Keys.Right:
                    WalkingDirection = Direction.DirectionX;
                    break;
                case Keys.W:
                    WalkingDirection = Direction.DirectionY;
                    break;
                case Keys.S:
                    WalkingDirection = Direction.AntiDirectionY;
                    break;
                case Keys.Up:
                    WalkingDirection = Direction.DirectionZ;
                    break;
                case Keys.Down:
                    WalkingDirection = Direction.AntiDirectionZ;
                    break;
            }
        }

        private void MainFrm_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKey(e.KeyCode);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            CancelRequest = true;
            Snake.Add(new Location(25, 25, 26));
            Snake.Add(new Location(25, 25, 25));
            Snake.Add(new Location(25, 25, 24));
            WalkingDirection = Direction.AntiDirectionZ;
        }

    }
}
