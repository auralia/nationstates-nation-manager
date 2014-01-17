//-----------------------------------------------------------------------
// <copyright file="PasswordDialog.cs" company="Auralia">
//     Copyright (C) 2013 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.NationManager
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Represents a dialog allowing a user to specify a password.
    /// </summary>
    public partial class PasswordDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordDialog"/> class.
        /// </summary>
        public PasswordDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get
            {
                return passwordTextBox.Text;
            }

            set
            {
                passwordTextBox.Text = value;
            }
        }

        /// <summary>
        /// This method is invoked when the OK button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (!passwordTextBox.Text.Equals(reenterPasswordTextBox.Text))
            {
                MessageBox.Show("The passwords must match.");
            }
            else if (passwordTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("The password field cannot be empty.");
            }
            else
            {
                this.DialogResult = DialogResult.OK;

                this.Close();
            }
        }

        /// <summary>
        /// This method is invoked when the cancel button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }
    }
}
