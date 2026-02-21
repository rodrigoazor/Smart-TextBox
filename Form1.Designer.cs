
namespace Smart_TextBox
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            smartTextBox1 = new TextBox.SmartTextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(562, 225);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // smartTextBox1
            // 
            smartTextBox1.AutoValidationMode = true;
            smartTextBox1.BackColor = Color.White;
            smartTextBox1.BorderColor = Color.Black;
            smartTextBox1.BorderErrorColor = Color.Red;
            smartTextBox1.BorderFocusColor = Color.FromArgb(0, 120, 215);
            smartTextBox1.BorderRadius = 10;
            smartTextBox1.BorderThickness = 1;
            smartTextBox1.CustomRegexPattern = "";
            smartTextBox1.ErrorMessage = "Valor inválido.";
            smartTextBox1.FocusMessage = "";
            smartTextBox1.InnerIcon = TextBox.InnerIconType.Email;
            smartTextBox1.InnerIconPosition = TextBox.InnerIconPosition.Left;
            smartTextBox1.Location = new Point(119, 141);
            smartTextBox1.Name = "smartTextBox1";
            smartTextBox1.ShowBorderFocus = true;
            smartTextBox1.ShowErrorIcon = true;
            smartTextBox1.Size = new Size(289, 40);
            smartTextBox1.TabIndex = 3;
            smartTextBox1.TimeToolTipText = 5;
            smartTextBox1.ValidationMessage = "Valor inválido.";
            smartTextBox1.ValidationMode = TextBox.ValidationMode.Email;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveBorder;
            ClientSize = new Size(800, 450);
            Controls.Add(smartTextBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private TextBox.SmartTextBox smartTextBox1;
    }
}