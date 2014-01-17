namespace Auralia.NationStates.NationManager
{
    partial class OptionsDialog
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
            this.networkGroupBox = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.loginAttemptDelayUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.apiRequestDelayUpDown = new System.Windows.Forms.NumericUpDown();
            this.userAgentLabel = new System.Windows.Forms.Label();
            this.userAgentTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.networkGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loginAttemptDelayUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.apiRequestDelayUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // networkGroupBox
            // 
            this.networkGroupBox.Controls.Add(this.label4);
            this.networkGroupBox.Controls.Add(this.loginAttemptDelayUpDown);
            this.networkGroupBox.Controls.Add(this.label3);
            this.networkGroupBox.Controls.Add(this.label2);
            this.networkGroupBox.Controls.Add(this.label1);
            this.networkGroupBox.Controls.Add(this.apiRequestDelayUpDown);
            this.networkGroupBox.Controls.Add(this.userAgentLabel);
            this.networkGroupBox.Controls.Add(this.userAgentTextBox);
            this.networkGroupBox.Location = new System.Drawing.Point(12, 12);
            this.networkGroupBox.Name = "networkGroupBox";
            this.networkGroupBox.Size = new System.Drawing.Size(360, 108);
            this.networkGroupBox.TabIndex = 0;
            this.networkGroupBox.TabStop = false;
            this.networkGroupBox.Text = "Network";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(291, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "milliseconds";
            // 
            // loginAttemptDelayUpDown
            // 
            this.loginAttemptDelayUpDown.Location = new System.Drawing.Point(161, 71);
            this.loginAttemptDelayUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.loginAttemptDelayUpDown.Minimum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.loginAttemptDelayUpDown.Name = "loginAttemptDelayUpDown";
            this.loginAttemptDelayUpDown.Size = new System.Drawing.Size(124, 20);
            this.loginAttemptDelayUpDown.TabIndex = 7;
            this.loginAttemptDelayUpDown.Value = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Delay between login attempts:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(291, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "milliseconds";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Delay between API requests:";
            // 
            // apiRequestDelayUpDown
            // 
            this.apiRequestDelayUpDown.Location = new System.Drawing.Point(161, 45);
            this.apiRequestDelayUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.apiRequestDelayUpDown.Minimum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.apiRequestDelayUpDown.Name = "apiRequestDelayUpDown";
            this.apiRequestDelayUpDown.Size = new System.Drawing.Size(124, 20);
            this.apiRequestDelayUpDown.TabIndex = 4;
            this.apiRequestDelayUpDown.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            // 
            // userAgentLabel
            // 
            this.userAgentLabel.AutoSize = true;
            this.userAgentLabel.Location = new System.Drawing.Point(6, 22);
            this.userAgentLabel.Name = "userAgentLabel";
            this.userAgentLabel.Size = new System.Drawing.Size(62, 13);
            this.userAgentLabel.TabIndex = 1;
            this.userAgentLabel.Text = "User agent:";
            // 
            // userAgentTextBox
            // 
            this.userAgentTextBox.Location = new System.Drawing.Point(161, 19);
            this.userAgentTextBox.Name = "userAgentTextBox";
            this.userAgentTextBox.Size = new System.Drawing.Size(193, 20);
            this.userAgentTextBox.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(297, 126);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(216, 126);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.networkGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.networkGroupBox.ResumeLayout(false);
            this.networkGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loginAttemptDelayUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.apiRequestDelayUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox networkGroupBox;
        private System.Windows.Forms.Label userAgentLabel;
        private System.Windows.Forms.TextBox userAgentTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown apiRequestDelayUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown loginAttemptDelayUpDown;
        private System.Windows.Forms.Label label3;
    }
}