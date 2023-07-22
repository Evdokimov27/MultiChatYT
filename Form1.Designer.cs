namespace YTChat
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			videoIdText = new TextBox();
			label1 = new Label();
			button1 = new Button();
			button2 = new Button();
			Accounts = new ListBox();
			label2 = new Label();
			label3 = new Label();
			NameStreamText = new Label();
			button3 = new Button();
			textBox1 = new TextBox();
			button4 = new Button();
			SuspendLayout();
			// 
			// videoIdText
			// 
			videoIdText.Location = new Point(12, 39);
			videoIdText.Name = "videoIdText";
			videoIdText.Size = new Size(99, 23);
			videoIdText.TabIndex = 0;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(13, 15);
			label1.Name = "label1";
			label1.Size = new Size(98, 15);
			label1.TabIndex = 2;
			label1.Text = "ID видео/стрима";
			// 
			// button1
			// 
			button1.Location = new Point(117, 38);
			button1.Name = "button1";
			button1.Size = new Size(60, 23);
			button1.TabIndex = 4;
			button1.Text = "LIKE";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// button2
			// 
			button2.Location = new Point(58, 385);
			button2.Name = "button2";
			button2.Size = new Size(157, 37);
			button2.TabIndex = 5;
			button2.Text = "ОТПРАВИТЬ";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// Accounts
			// 
			Accounts.FormattingEnabled = true;
			Accounts.ItemHeight = 15;
			Accounts.Location = new Point(12, 85);
			Accounts.Name = "Accounts";
			Accounts.Size = new Size(230, 199);
			Accounts.TabIndex = 6;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(13, 67);
			label2.Name = "label2";
			label2.Size = new Size(123, 15);
			label2.TabIndex = 7;
			label2.Text = "Аккаунт отправителя";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(13, 290);
			label3.Name = "label3";
			label3.Size = new Size(73, 15);
			label3.TabIndex = 8;
			label3.Text = "Сообщение";
			// 
			// NameStreamText
			// 
			NameStreamText.AutoSize = true;
			NameStreamText.Location = new Point(179, 15);
			NameStreamText.Name = "NameStreamText";
			NameStreamText.Size = new Size(48, 15);
			NameStreamText.TabIndex = 9;
			NameStreamText.Text = "Ничего";
			// 
			// button3
			// 
			button3.Location = new Point(248, 87);
			button3.Name = "button3";
			button3.Size = new Size(19, 197);
			button3.TabIndex = 10;
			button3.Text = "ДОБАВИТЬ";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// textBox1
			// 
			textBox1.Location = new Point(13, 308);
			textBox1.Multiline = true;
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(251, 61);
			textBox1.TabIndex = 11;
			// 
			// button4
			// 
			button4.Location = new Point(183, 38);
			button4.Name = "button4";
			button4.Size = new Size(81, 23);
			button4.TabIndex = 12;
			button4.Text = "Просмотры";
			button4.UseVisualStyleBackColor = true;
			button4.Click += button4_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(276, 450);
			Controls.Add(button4);
			Controls.Add(textBox1);
			Controls.Add(button3);
			Controls.Add(NameStreamText);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(Accounts);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(label1);
			Controls.Add(videoIdText);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "Form1";
			ShowIcon = false;
			Text = "МультиЧАТ";
			Load += Form1_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox videoIdText;
		private Label label1;
		private Button button1;
		private Button button2;
		private ListBox Accounts;
		private Label label2;
		private Label label3;
		private Label NameStreamText;
		private Button button3;
		private TextBox textBox1;
		private Button button4;
	}
}