namespace VirusChecker
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
            label1 = new Label();
            openFileDialog1 = new OpenFileDialog();
            chooseFileButton = new Button();
            resultLabel = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(248, 26);
            label1.Name = "label1";
            label1.Size = new Size(322, 65);
            label1.TabIndex = 0;
            label1.Text = "VirusChecker";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // chooseFileButton
            // 
            chooseFileButton.Location = new Point(348, 219);
            chooseFileButton.Name = "chooseFileButton";
            chooseFileButton.Size = new Size(118, 23);
            chooseFileButton.TabIndex = 1;
            chooseFileButton.Text = "Выбрать файл...";
            chooseFileButton.UseVisualStyleBackColor = true;
            chooseFileButton.Click += chooseFileButton_Click;
            // 
            // resultLabel
            // 
            resultLabel.AutoSize = true;
            resultLabel.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resultLabel.Location = new Point(260, 135);
            resultLabel.Name = "resultLabel";
            resultLabel.Size = new Size(297, 30);
            resultLabel.TabIndex = 2;
            resultLabel.Text = "Загрузите файл для проверки";
            resultLabel.TextAlign = ContentAlignment.BottomCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources._7674afaab0239fcc95db982e75bdc684;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(resultLabel);
            Controls.Add(chooseFileButton);
            Controls.Add(label1);
            Name = "Form1";
            Text = "VirusChecker";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private OpenFileDialog openFileDialog1;
        private Button chooseFileButton;
        private Label resultLabel;
    }
}
