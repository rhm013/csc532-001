using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project {
  public partial class FrmBattle : Form {
    public static FrmBattle instance = null;
    private Enemy enemy;
    private Player player;
        private static int charbattle;
        private FrmBattle() {
      InitializeComponent();
      player = Game.player;
    }

    public void Setup() {
      // update for this enemy
      picEnemy.BackgroundImage = enemy.Img;
      picEnemy.Refresh();
      BackColor = enemy.Color;
      picBossBattle.Visible = false;
            getplayer();
            // Observer pattern
            enemy.AttackEvent += PlayerDamage;
      player.AttackEvent += EnemyDamage;

      // show health
      UpdateHealthBars();
    }
        private void getplayer()
        {
            switch (charbattle)
            {
                case 1:
                    picPlayer.BackgroundImage = Properties.Resources.char1;
                    break;
                case 2:
                    picPlayer.BackgroundImage = Properties.Resources.char2;
                    break;
                case 0:
                    picPlayer.BackgroundImage = Properties.Resources.player;

                    break;
                case 3:
                    picPlayer.BackgroundImage = Properties.Resources.ak47pla;

                    break;
                case 4:
                    picPlayer.BackgroundImage = Properties.Resources.machinegunpla;

                    break;
                case 5:
                    picPlayer.BackgroundImage = Properties.Resources.charak47;

                    break;
                case 6:
                    picPlayer.BackgroundImage = Properties.Resources.machinchar2;

                    break;
            }
        }
        public void SetupForBossBattle() {
      picBossBattle.Location = Point.Empty;
      picBossBattle.Size = ClientSize;
      picBossBattle.Visible = true;

      SoundPlayer simpleSound = new SoundPlayer(Resources.final_battle);
      simpleSound.Play();

      tmrFinalBattle.Enabled = true;
    }

    public static FrmBattle GetInstance(Enemy enemy, int charactorchoice)
    {
            charbattle = charactorchoice;
            if (instance == null) {
        instance = new FrmBattle();
        instance.enemy = enemy;
        instance.Setup();
      }
      return instance;
    }

    private void UpdateHealthBars() {
      float playerHealthPer = player.Health / (float)player.MaxHealth;
      float enemyHealthPer = enemy.Health / (float)enemy.MaxHealth;

      const int MAX_HEALTHBAR_WIDTH = 226;
      lblPlayerHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * playerHealthPer);
      lblEnemyHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * enemyHealthPer);

      lblPlayerHealthFull.Text = player.Health.ToString();
      lblEnemyHealthFull.Text = enemy.Health.ToString();
    }

    private void btnAttack_Click(object sender, EventArgs e) {
            if (charbattle == 3 || charbattle == 5)
            {
                player.OnAttack(-4);
            }
            else if (charbattle == 4 || charbattle == 6)
            {
                player.OnAttack(-8);
            }
            else
            {
                player.OnAttack(-2);

            }
            
      if (enemy.Health > 0) {
                enemy.OnAttack(-4);

            }

      UpdateHealthBars();
      if (player.Health <= 0 || enemy.Health <= 0) {
        instance = null;
        Close();
      }
    }

    private void EnemyDamage(int amount) {
      enemy.AlterHealth(amount);
    }

    private void PlayerDamage(int amount) {
      player.AlterHealth(amount);
    }

    private void tmrFinalBattle_Tick(object sender, EventArgs e) {
      picBossBattle.Visible = false;
      tmrFinalBattle.Enabled = false;
    }
  }
}
