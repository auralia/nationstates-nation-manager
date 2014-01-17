//-----------------------------------------------------------------------
// <copyright file="NationDialog.cs" company="Auralia">
//     Copyright (C) 2013 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.NationManager
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Represents a dialog allowing a user to create or edit a nation.
    /// </summary>
    public partial class NationDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NationDialog"/> class.
        /// </summary>
        public NationDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the nation displayed in the nation text box.
        /// </summary>
        /// <value>The nation displayed in the nation text box.</value>
        public string Nation
        {
            get
            {
                return nationTextBox.Text;
            }

            set
            {
                nationTextBox.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the password displayed in the password text box.
        /// </summary>
        /// <value>The password displayed in the password text box.</value>
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
            if (nationTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("The nation field cannot be empty.", "NationStates Nation Manager");
            }
            else if (passwordTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("The password field cannot be empty.", "NationStates Nation Manager");
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
