//-----------------------------------------------------------------------
// <copyright file="OptionsDialog.cs" company="Auralia">
//     Copyright (C) 2013 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.NationManager
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Represents a dialog allowing a user to change application options.
    /// </summary>
    public partial class OptionsDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialog"/> class.
        /// </summary>
        public OptionsDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the user agent specified in API requests and login attempts.
        /// </summary>
        /// <value>The user agent specified in API requests and login attempts.</value>
        public string UserAgent
        {
            get
            {
                return userAgentTextBox.Text;
            }

            set
            {
                userAgentTextBox.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the delay between API requests in milliseconds.
        /// </summary>
        /// <value>The delay between API requests in milliseconds.</value>
        public int ApiRequestDelay
        {
            get
            {
                return (int)apiRequestDelayUpDown.Value;
            }

            set
            {
                apiRequestDelayUpDown.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the delay between login attempts in milliseconds.
        /// </summary>
        /// <value>The delay between login attempts in milliseconds</value>
        public int LoginAttemptDelay
        {
            get
            {
                return (int)loginAttemptDelayUpDown.Value;
            }

            set
            {
                loginAttemptDelayUpDown.Value = value;
            }
        }

        /// <summary>
        /// This method is invoked when the OK button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (userAgentTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("The user agent field cannot be empty.", "NationStates Nation Manager");
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
