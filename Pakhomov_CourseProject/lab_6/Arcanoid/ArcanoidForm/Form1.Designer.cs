
namespace ArcanoidForm
{
  partial class Form1
  {
    /// <summary>
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1Score = new System.Windows.Forms.Label();
            this.GmStatusLabel = new System.Windows.Forms.Label();
            this.game2 = new Arcanoid.Game();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(567, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 36);
            this.button1.TabIndex = 2;
            this.button1.Text = "Старт";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(567, 98);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 36);
            this.button2.TabIndex = 3;
            this.button2.Text = "Закончить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1Score
            // 
            this.label1Score.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1Score.AutoSize = true;
            this.label1Score.Location = new System.Drawing.Point(611, 10);
            this.label1Score.Name = "label1Score";
            this.label1Score.Size = new System.Drawing.Size(35, 13);
            this.label1Score.TabIndex = 4;
            this.label1Score.Text = "Score";
            // 
            // GmStatusLabel
            // 
            this.GmStatusLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.GmStatusLabel.AutoSize = true;
            this.GmStatusLabel.Location = new System.Drawing.Point(555, 601);
            this.GmStatusLabel.Name = "GmStatusLabel";
            this.GmStatusLabel.Size = new System.Drawing.Size(16, 13);
            this.GmStatusLabel.TabIndex = 5;
            this.GmStatusLabel.Text = "...";
            // 
            // game2
            // 
            this.game2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.game2.BallColor = System.Drawing.Color.Black;
            this.game2.BlockColor = System.Drawing.Color.Crimson;
            this.game2.Location = new System.Drawing.Point(2, 2);
            this.game2.MapCol = 10;
            this.game2.MapColor = System.Drawing.Color.LightCyan;
            this.game2.MapRow = 30;
            this.game2.Name = "game2";
            this.game2.Padding = new System.Windows.Forms.Padding(5);
            this.game2.PlatformColor = System.Drawing.Color.OrangeRed;
            this.game2.Size = new System.Drawing.Size(540, 620);
            this.game2.TabIndex = 0;
            this.game2.RecordScore += new System.EventHandler(this.game_Score);
            this.game2.RecordStatus += new System.EventHandler(this.game2_RecordStatus);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 624);
            this.Controls.Add(this.GmStatusLabel);
            this.Controls.Add(this.label1Score);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.game2);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Arcanoid.Game game2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label1Score;
        private System.Windows.Forms.Label GmStatusLabel;
    }
}

