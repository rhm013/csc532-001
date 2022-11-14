using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project {
  public partial class FrmLevel : Form {
    private Player player;

    private Enemy enemyPoisonPacket;
    private Enemy bossKoolaid;
    private Enemy enemyCheeto;
    private Character[] walls;
    private bool poisionflag = false;
    private bool cheetoflag = false;
    private bool bossflag = false;
    private bool playerflag = false;
    private int charactorchoice;
    private bool restrictkeys = false;
    private DateTime timeBegin;
    private FrmBattle frmBattle;
    private int shifter=1;
        private int weaponchoice;
        private int vehiclechoice;
        private bool pause=true;
        const int PADDING = 7;
        public FrmLevel() {
      InitializeComponent();
    }

    private void FrmLevel_Load(object sender, EventArgs e) {
      
      const int NUM_WALLS = 13;

      player = new Player(CreatePositionPlayer(playerlayout), CreateColliderPlayer(playerlayout, PADDING));
      bossKoolaid = new Enemy(CreatePosition(picBossKoolAid), CreateCollider(picBossKoolAid, PADDING));
      enemyPoisonPacket = new Enemy(CreatePosition(picEnemyPoisonPacket), CreateCollider(picEnemyPoisonPacket, PADDING));
      enemyCheeto = new Enemy(CreatePosition(picEnemyCheeto), CreateCollider(picEnemyCheeto, PADDING));

      bossKoolaid.Img = picBossKoolAid.BackgroundImage;
      enemyPoisonPacket.Img = picEnemyPoisonPacket.BackgroundImage;
      enemyCheeto.Img = picEnemyCheeto.BackgroundImage;

      bossKoolaid.Color = Color.Red;
      enemyPoisonPacket.Color = Color.Green;
      enemyCheeto.Color = Color.FromArgb(255, 245, 161);

      walls = new Character[NUM_WALLS];
      for (int w = 0; w < NUM_WALLS; w++) {
        PictureBox pic = Controls.Find("picWall" + w.ToString(), true)[0] as PictureBox;
        walls[w] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
      }

      Game.player = player;
      timeBegin = DateTime.Now;
    }

    private Vector2 CreatePositionPlayer(FlowLayoutPanel pic)
    {
        return new Vector2(pic.Location.X, pic.Location.Y);
    }
    private Collider CreateColliderPlayer(FlowLayoutPanel pic, int padding)
    {
            Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
            return new Collider(rect);
        }
    private Vector2 CreatePosition(PictureBox pic) {
      return new Vector2(pic.Location.X, pic.Location.Y);
    }

    private Collider CreateCollider(PictureBox pic, int padding) {
      Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
      return new Collider(rect);
    }

    private void FrmLevel_KeyUp(object sender, KeyEventArgs e) {
      player.ResetMoveSpeed();
    }

    private void tmrUpdateInGameTime_Tick(object sender, EventArgs e) {
      TimeSpan span = DateTime.Now - timeBegin;
      string time = span.ToString(@"hh\:mm\:ss");
      lblInGameTime.Text = "Time: " + time.ToString();
    }

    private void tmrPlayerMove_Tick(object sender, EventArgs e) {
            if(pause == false) { player.Move(); }
      // move player
     


            // check collision with walls

            if (HitAWall(player))
            {
                player.MoveBack();
            }

            // check collision with enemies
            if (enemyPoisonPacket.Health < 0 && poisionflag == false)
            {
                SoundPlayer simpleSound = new SoundPlayer(Resources.poison);
                simpleSound.Play();
                picEnemyPoisonPacket.Enabled = false;
                picEnemyPoisonPacket.Visible = false;
                poisionflag = true;
            }
            else if (enemyPoisonPacket.Health > 0)
            {
                if (HitAChar(player, enemyPoisonPacket))
                {
                    Fight(enemyPoisonPacket);
                }
            }
            if (enemyCheeto.Health < 0 && cheetoflag == false)
            {
                picEnemyCheeto.Enabled = false;
                picEnemyCheeto.Visible = false;
                SoundPlayer simpleSound = new SoundPlayer(Resources.cheeto);
                simpleSound.Play();
                cheetoflag = true;

            }
            else if (enemyCheeto.Health > 0)
            {
                if (HitAChar(player, enemyCheeto))
                {
                    Fight(enemyCheeto);
                }
            }
            if (bossKoolaid.Health < 0 && bossflag == false)
            {
                picBossKoolAid.Enabled = false;
                picBossKoolAid.Visible = false;
                SoundPlayer simpleSound = new SoundPlayer(Resources.boss);
                simpleSound.Play();
                bossflag = true;


            }
            else if (bossKoolaid.Health > 0)
            {
                if (HitAChar(player, bossKoolaid))
                {
                    Fight(bossKoolaid);
                }
            }
            if (bossKoolaid.Health < 0 && enemyCheeto.Health < 0 && enemyPoisonPacket.Health < 0 && playerflag == false)
      {
          Thread.Sleep(4000);
          SoundPlayer simpleSound = new SoundPlayer(Resources.wona);
          simpleSound.Play();
          winlosspopup.Enabled = true;
          winlosspopup.Visible = true;
          playerflag = true;
          restartlabel.Enabled = true;
          restartlabel.Visible = true;


      }
            if (player.Health <= 0 && playerflag == false)
      {

          SoundPlayer simpleSound = new SoundPlayer(Resources.losta);
          simpleSound.Play();
          this.winlosspopup.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.lost;
          winlosspopup.Enabled = true;
          winlosspopup.Visible = true;
          playerlayout.Visible = false;
          Thread.Sleep(4000);
          playerflag = true;
          restartlabel.Enabled = true;
          restartlabel.Visible = true;

      }

      // update player's picture box
      playerlayout.Location = new Point((int)player.Position.x, (int)player.Position.y);
    }

    private bool HitAWall(Character c) {
      bool hitAWall = false;
      for (int w = 0; w < walls.Length; w++) {
        if (c.Collider.Intersects(walls[w].Collider)) {
          hitAWall = true;
          break;
        }
      }
      return hitAWall;
    }

    private bool HitAChar(Character you, Character other) {
      return you.Collider.Intersects(other.Collider);
    }

    private void Fight(Enemy enemy) {
      player.ResetMoveSpeed();
      player.MoveBack();
      frmBattle = FrmBattle.GetInstance(enemy, charactorchoice);
      frmBattle.Show();

      if (enemy == bossKoolaid) {
        frmBattle.SetupForBossBattle();
      }
    }

    private void FrmLevel_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.Left:
          player.GoLeft();
          break;

        case Keys.Right:
          player.GoRight();
          break;

        case Keys.Up:
          player.GoUp();
          break;

        case Keys.Down:
          player.GoDown();
          break;
        case Keys.R:
            Restart(sender, e);
            break;
        case Keys.S:
                    changeSpeed();
                    break;
                case Keys.D:
                    Dropweapon();
                    break;
                case Keys.Q:
                    Dropvehicle();
                    break;
                case Keys.M:
                    pause = true;
                    Menu();
                    break;
                default:
          player.ResetMoveSpeed();
          break;
      }
    }

        private void Menu()
        {
           
            if (playcontrolmenu.Visible != true)
            {
                playcontrolmenu.Enabled = true;
                playcontrolmenu.Visible = true;
            }
            else
            {
                playcontrolmenu.Enabled = false;
                playcontrolmenu.Visible = false;

            }
        }

         

        private void Dropvehicle()
        {
            weaponhold.Visible = false;
            weaponhold.Enabled = false;
            this.playerlayout.Controls.Remove(this.vehiclehold);
            player = new Player(CreatePositionPlayer(playerlayout), CreateColliderPlayer(playerlayout, PADDING));
        }

        private void Dropweapon()
        {
          weaponhold.Visible = false;
            weaponhold.Enabled = false;
            this.playerweaponflow.Controls.Remove(this.weaponhold);
            player = new Player(CreatePositionPlayer(playerlayout), CreateColliderPlayer(playerlayout, PADDING));
        }

        private void changeSpeed()
        {
            shifter *= -1;
            if (shifter == -1) { player.GO_INC = 6; }
            else    { player.GO_INC = 3;}
        }

        private void Restart(object sender, KeyEventArgs e)
     {
         playerlayout.Visible = false;
         playerlayout.Enabled = false;
         restartlabel.Enabled = false;
         restartlabel.Visible = false;
         restartpopup.Enabled = true;
         restartpopup.Visible = true;
         winlosspopup.Visible = false;
         winlosspopup.Enabled = false;
         restrictkeys = true;
     }



     private void amoung_Click(object sender, EventArgs e)
     {
         this.picPlayer.BackgroundImage = Properties.Resources.char2;
         charactorchoice = 2;

     }


     private void fox_Click(object sender, EventArgs e)
     {
         this.picPlayer.BackgroundImage = Properties.Resources.char1;
         charactorchoice = 1;

     }

     private void playericon_Click(object sender, EventArgs e)
     {

         this.picPlayer.BackgroundImage = Properties.Resources.player;
         charactorchoice = 0;
     }

     private void charactericon_Click(object sender, EventArgs e)
     {
         if (flowLayoutPanel1.Visible != true)
         {
             flowLayoutPanel1.Visible = true;
             flowLayoutPanel1.Enabled = true;
         }
         else
         {
             flowLayoutPanel1.Visible = false;
             flowLayoutPanel1.Enabled = false;
         }
     }

        private void restart_Click(object sender, EventArgs e)
     {

         MouseEventArgs me = (MouseEventArgs)e;
         Point coordinates = me.Location;
         if (241 < coordinates.X && coordinates.X < 349 && 138 < coordinates.Y && coordinates.Y < 187)
         {
             Application.Restart();

         }
         else if (58 < coordinates.X && coordinates.X < 136 && 140 < coordinates.Y && coordinates.Y < 187)
         {
             restartpopup.Enabled = false;
             restartpopup.Visible = false;
             playerlayout.Visible = true;
             playerlayout.Enabled = true;
             restrictkeys = false;
         }
     }
        private void lblInGameTime_Click(object sender, EventArgs e) {

        }

        private void weaponsbox_Click(object sender, EventArgs e)
        {
            if (weaponsflow.Visible != true)
            {
                weaponsflow.Visible = true;
                weaponsflow.Enabled = true;
            }
            else
            {
                weaponsflow.Visible = false;
                weaponsflow.Enabled = false;
            }
        }

        private void machinegunpic_Click(object sender, EventArgs e)
        {
            this.weaponhold.BackgroundImage = Properties.Resources.machinegun;
            weaponchoice = 2;
            weaponhold.Visible = true;
            weaponhold.Enabled = true;
            //this.playerlayout.Controls.Add(this.playerweaponflow);
            weaponcommon();


        }
        private void weaponcommon()
        {
            this.playerweaponflow.Controls.Add(this.weaponhold);
            restartlabel.Enabled = true;
            restartlabel.Enabled = true;
            restartlabel.Text = "Press D to Quite Vehicle";
        }
        private void ak47_Click(object sender, EventArgs e)
        {
            this.weaponhold.BackgroundImage = Properties.Resources.ak47;
            weaponchoice = 3;
            weaponhold.Visible = true;
            weaponhold.Enabled = true;
            //this.playerlayout.Controls.Add(this.playerweaponflow);
            weaponcommon();

        }

        private void garage_Click(object sender, EventArgs e)
        {
            if (drivechoice.Visible != true)
            {
                drivechoice.Visible = true;
                drivechoice.Enabled = true;
            }
            else
            {
                drivechoice.Visible = false;
                drivechoice.Enabled = false;
            }

        }

        private void racingcar_Click(object sender, EventArgs e)
        {
            this.playerlayout.Controls.Add(this.vehiclehold);
            this.vehiclehold.BackgroundImage = Properties.Resources.racingcar;
            vehiclechoice = 3;
            carcommon();
        }
        private void carcommon()
        {
            restartlabel.Enabled = true;
            restartlabel.Enabled = true;
            vehiclehold.Visible = true;
            vehiclehold.Enabled = true;
            restartlabel.Text = "Press Q to Quite Vehicle";

        }
        private void bike_Click(object sender, EventArgs e)
        {
            this.playerlayout.Controls.Add(this.vehiclehold);
            this.vehiclehold.BackgroundImage = Properties.Resources.motorbike;
            vehiclechoice = 2;
            carcommon();


        }


        private void playcontrolmenu_Click_1(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            if (123 < coordinates.X && coordinates.X < 291 && 180 < coordinates.Y && coordinates.Y < 226)
            {
                pause = false;
                Menu();
            }
            else if (119 < coordinates.X && coordinates.X < 290 && 242 < coordinates.Y && coordinates.Y < 285)
            {
                this.Close();
            }
        }
    }
}
