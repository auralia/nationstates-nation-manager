//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Auralia">
//     Copyright (C) 2013 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.NationManager
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using System.Windows.Forms;

    /// <summary>
    /// Represents the main form of the application.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Represents an object used for thread synchronization.
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            if (Properties.Settings.Default.UserAgent.Equals(string.Empty))
            {
                do
                {
                    MessageBox.Show("You must set a user agent for the NationStates API before using this application. If you are not sure what a user agent is, type in the name of your main nation.", "NationStates Nation Manager");
                }
                while (this.ShowOptionsDialog() == DialogResult.Cancel);
            }

            this.Nations = new List<Nation>();
            this.NationStatesApi = new Api.Api(this.UserAgent);
        }

        /// <summary>
        /// Represents the status of a nation.
        /// </summary>
        private enum NationStatus
        {
            /// <summary>
            /// Represents that the puppet exists.
            /// </summary>
            Exists = 0,

            /// <summary>
            /// Represents that the puppet does not exist.
            /// </summary>
            DoesNotExist = 1,

            /// <summary>
            /// Represents that the puppet may or may not exist.
            /// </summary>
            Unknown = 2,

            /// <summary>
            /// Represents that a determination of the puppet's existence is pending. 
            /// </summary>
            Pending = 3
        }

        /// <summary>
        /// Gets or sets an <see cref="Api.Api"/> object representing the NationStates API.
        /// </summary>
        /// <value>An <see cref="Api.Api"/> object representing the NationStates API.</value>
        private Api.Api NationStatesApi
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the delay between login attempts in milliseconds.
        /// </summary>
        /// <value>The delay between login attempts in milliseconds</value>
        private int LoginAttemptDelay
        {
            get
            {
                return Properties.Settings.Default.LoginAttemptDelay;
            }

            set
            {
                Properties.Settings.Default.LoginAttemptDelay = value;
            }
        }

        /// <summary>
        /// Gets or sets the delay between API requests in milliseconds.
        /// </summary>
        /// <value>The delay between API requests in milliseconds.</value>
        private int ApiRequestDelay
        {
            get
            {
                return Properties.Settings.Default.ApiRequestDelay;
            }

            set
            {
                Properties.Settings.Default.ApiRequestDelay = value;

                if (this.NationStatesApi != null)
                {
                    this.NationStatesApi.StandardDelay = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> object representing the date and time of the last login attempt.
        /// </summary>
        /// <value>A <see cref="DateTime"/> object representing the date and time of the last login attempt.</value>
        private DateTime LastLoginAttemptTime
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the user agent specified in API requests and login attempts.
        /// </summary>
        /// <value>The user agent specified in API requests and login attempts.</value>
        private string UserAgent
        {
            get
            {
                return Properties.Settings.Default.UserAgent;
            }

            set
            {
                Properties.Settings.Default.UserAgent = value;

                if (this.NationStatesApi != null)
                {
                    this.NationStatesApi.UserAgent = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a list of <see cref="Nation"/> objects currently loaded in the list view.
        /// </summary>
        /// <value>A list of <see cref="Nation"/> objects currently loaded in the list view.</value>
        private List<Nation> Nations
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the name of the open file.
        /// </summary>
        /// <value>The name of the open file.</value>
        private string OpenFileName
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the password used to encrypt the open file.
        /// </summary>
        /// <value>The password used to encrypt the open file.</value>
        private string OpenFilePassword
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="FileStream"/> object representing the open file.
        /// </summary>
        /// <value>The <see cref="FileStream"/> object representing the open file.</value>
        private FileStream OpenFileStream
        {
            get;

            set;
        }

        /// <summary>
        /// This method is invoked when the New tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.NewFile();
        }

        /// <summary>
        /// Creates a new nation file.
        /// </summary>
        private void NewFile()
        {
            if (this.ShowSavePrompt() != DialogResult.Cancel)
            {
                this.ResetNations();
            }
        }

        /// <summary>
        /// Resets the list of nations.
        /// </summary>
        private void ResetNations()
        {
            this.Text = "NationStates Nation Manager - Untitled";

            this.OpenFileStream = null;
            this.OpenFileName = null;
            this.OpenFilePassword = null;

            this.Nations = new List<Nation>();
            this.listView.Items.Clear();
        }

        /// <summary>
        /// This method is invoked when the Open tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenFile();
        }

        /// <summary>
        /// Opens a puppet file.
        /// </summary>
        private void OpenFile()
        {
            if (normalOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                var passwordDialog = new PasswordDialog();

                var dialogResult = passwordDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if (this.ShowSavePrompt() != DialogResult.Cancel)
                    {
                        this.ResetNations();

                        FileStream fileStream = null;
                        byte[] iv = null;
                        byte[] encryptedBytes = null;

                        try
                        {
                            fileStream = new FileStream(normalOpenFileDialog.FileName, FileMode.Open);

                            iv = new byte[16];
                            encryptedBytes = new byte[fileStream.Length - 16];

                            fileStream.Read(iv, 0, 16);
                            fileStream.Read(encryptedBytes, 0, encryptedBytes.Length);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("An error occurred while opening or parsing data from the file. Do you have sufficient permissions to access the file?", "NationStates Nation Manager");

                            if (fileStream != null)
                            {
                                fileStream.Close();
                            }

                            return;
                        }

                        MemoryStream decryptedStream = null;
                        CryptoStream cryptoStream = null;

                        try
                        {
                            var key = this.GenerateKeyFromPassword(passwordDialog.Password);
                            var decryptor = new AesCryptoServiceProvider().CreateDecryptor(key, iv);

                            decryptedStream = new MemoryStream();
                            cryptoStream = new CryptoStream(decryptedStream, decryptor, CryptoStreamMode.Write);
                            cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            decryptedStream.Seek(0, SeekOrigin.Begin);

                            var formatter = new BinaryFormatter();
                            var nations = (List<Nation>)formatter.Deserialize(decryptedStream);

                            foreach (Nation nation in nations)
                            {
                                this.AddNation(nation);
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("An error occurred while decrypting the file. Did you enter the correct password?", "NationStates Nation Manager");

                            if (decryptedStream != null)
                            {
                                decryptedStream.Close();
                            }

                            if (cryptoStream != null)
                            {
                                cryptoStream.Close();
                            }

                            return;
                        }

                        if (this.OpenFileStream != null)
                        {
                            this.OpenFileStream.Close();
                        }

                        this.OpenFileStream = fileStream;
                        this.OpenFileName = new FileInfo(normalOpenFileDialog.FileName).Name;
                        this.OpenFilePassword = passwordDialog.Password;

                        this.Text = "NationStates Nation Manager - " + this.OpenFileName;
                    }
                }
            }
        }

        /// <summary>
        /// This method is invoked when the Save tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveFileExisting();
        }

        /// <summary>
        /// Saves the puppet list to an existing file.
        /// </summary>
        private void SaveFileExisting()
        {
            if (this.OpenFileStream != null)
            {
                byte[] iv = null;
                byte[] encryptedBytes = null;

                MemoryStream encryptedStream = null;
                CryptoStream cryptoStream = null;

                try
                {
                    var serializationStream = new MemoryStream();

                    var formatter = new BinaryFormatter();
                    formatter.Serialize(serializationStream, this.Nations);

                    var serializedBytes = serializationStream.ToArray();

                    iv = this.GenerateInitializationVector();
                    var key = this.GenerateKeyFromPassword(this.OpenFilePassword);
                    var encryptor = new AesCryptoServiceProvider().CreateEncryptor(key, iv);

                    encryptedStream = new MemoryStream();
                    cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write);
                    cryptoStream.Write(serializedBytes, 0, serializedBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    encryptedStream.Seek(0, SeekOrigin.Begin);
                    encryptedBytes = new byte[encryptedStream.Length];
                    encryptedStream.Read(encryptedBytes, 0, encryptedBytes.Length);
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occurred while encrypting the data to write to the file.", "NationStates Nation Manager");

                    if (encryptedStream != null)
                    {
                        encryptedStream.Close();
                    }

                    if (cryptoStream != null)
                    {
                        cryptoStream.Close();
                    }

                    return;
                }

                try
                {
                    this.OpenFileStream.SetLength(0);
                    this.OpenFileStream.Write(iv, 0, iv.Length);
                    this.OpenFileStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occurred while creating or saving data to the file. Do you have sufficient permissions to access the file?", "NationStates Nation Manager");

                    return;
                }
            }
            else
            {
                this.SaveFileNew();
            }
        }

        /// <summary>
        /// This method is invoked when the Save As tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveFileNew();
        }

        /// <summary>
        /// Saves the puppet list to an new file.
        /// </summary>
        private void SaveFileNew()
        {
            if (normalSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var passwordDialog = new PasswordDialog();
                var dialogResult = passwordDialog.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    byte[] iv = null;
                    byte[] encryptedBytes = null;

                    MemoryStream encryptedStream = null;
                    CryptoStream cryptoStream = null;

                    try
                    {
                        var serializationStream = new MemoryStream();

                        var formatter = new BinaryFormatter();
                        formatter.Serialize(serializationStream, this.Nations);

                        var serializedBytes = serializationStream.ToArray();

                        iv = this.GenerateInitializationVector();
                        var key = this.GenerateKeyFromPassword(passwordDialog.Password);
                        var encryptor = new AesCryptoServiceProvider().CreateEncryptor(key, iv);

                        encryptedStream = new MemoryStream();
                        cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write);
                        cryptoStream.Write(serializedBytes, 0, serializedBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        encryptedStream.Seek(0, SeekOrigin.Begin);
                        encryptedBytes = new byte[encryptedStream.Length];
                        encryptedStream.Read(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error occurred while encrypting the data to write to the file.", "NationStates Nation Manager");

                        if (encryptedStream != null)
                        {
                            encryptedStream.Close();
                        }

                        if (cryptoStream != null)
                        {
                            cryptoStream.Close();
                        }

                        return;
                    }

                    FileStream fileStream = null;

                    try
                    {
                        fileStream = new FileStream(normalSaveFileDialog.FileName, FileMode.Create);
                        fileStream.Write(iv, 0, iv.Length);
                        fileStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error occurred while creating or saving data to the file.  Do you have sufficient permissions to access the file?", "NationStates Nation Manager");

                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }

                        return;
                    }

                    if (this.OpenFileStream != null)
                    {
                        this.OpenFileStream.Close();
                    }

                    this.OpenFileStream = fileStream;
                    this.OpenFileName = new FileInfo(normalSaveFileDialog.FileName).Name;
                    this.OpenFilePassword = passwordDialog.Password;

                    this.Text = "NationStates Nation Manager - " + this.OpenFileName;
                }
            }
        }

        /// <summary>
        /// Prompts the user whether they wish to save the current puppet list.
        /// </summary>
        /// <returns>The <see cref="DialogResult"/> returned by the save prompt.</returns>
        private DialogResult ShowSavePrompt()
        {
            DialogResult dialogResult = default(DialogResult);

            if (this.OpenFileStream != null)
            {
                dialogResult = MessageBox.Show("Do you wish to save any changes to " + this.OpenFileName + "?", "NationStates Nation Manager", MessageBoxButtons.YesNoCancel);

                if (dialogResult == DialogResult.Yes)
                {
                    this.SaveFileExisting();
                }
            }
            else if (this.Nations.Count > 0)
            {
                dialogResult = MessageBox.Show("Do you wish to save any changes to Untitled?", "NationStates Nation Manager", MessageBoxButtons.YesNoCancel);

                if (dialogResult == DialogResult.Yes)
                {
                    this.SaveFileNew();
                }
            }

            return dialogResult;
        }

        /// <summary>
        /// This method is invoked when the Import tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ImportFile();
        }

        /// <summary>
        /// Imports a list of puppets from a text file.
        /// </summary>
        private void ImportFile()
        {
            if (importOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = null;
                StreamReader streamReader = null;

                try
                {
                    fileStream = new FileStream(importOpenFileDialog.FileName, FileMode.Open);
                    streamReader = new StreamReader(fileStream);

                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split(',');

                        var nation = new Nation(values[0], values[1]);
                        this.AddNation(nation);
                    }

                    streamReader.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occurred while opening or parsing data from the file.  Do you have sufficient permissions to access the file?", "NationStates Nation Manager");
                }
                finally
                {
                    if (streamReader != null)
                    {
                        streamReader.Close();
                    }
                }
            }
        }

        /// <summary>
        /// This method is invoked when the Export tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExportFile();
        }

        /// <summary>
        /// Exports the list of puppets to a text file.
        /// </summary>
        private void ExportFile()
        {
            if (exportSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = null;
                StreamWriter streamWriter = null;

                try
                {
                    fileStream = new FileStream(exportSaveFileDialog.FileName, FileMode.Create);
                    streamWriter = new StreamWriter(fileStream);

                    foreach (Nation nation in this.Nations)
                    {
                        streamWriter.WriteLine(nation.Name + "," + nation.Password);
                    }

                    streamWriter.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occurred while creating or saving data to the file.  Do you have sufficient permissions to access the file?", "NationStates Nation Manager");
                }
                finally
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                    }
                }
            }
        }

        /// <summary>
        /// This method is invoked when the Exit tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void Exit()
        {
            this.Close();
        }

        /// <summary>
        /// This method is invoked when the main form is closing.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="FormClosingEventArgs"/> object that contains the event data.</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = this.ShowSavePrompt() == DialogResult.Cancel;
        }

        /// <summary>
        /// This method is invoked when the Add tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateNation();
        }

        /// <summary>
        /// Creates a nation and adds it to the list of nations.
        /// </summary>
        private void CreateNation()
        {
            var nationDialog = new NationDialog();

            if (nationDialog.ShowDialog() == DialogResult.OK)
            {
                var nation = new Nation(nationDialog.Nation, nationDialog.Password);

                this.AddNation(nation);
            }
        }

        /// <summary>
        /// Adds a nation to the list of nations.
        /// </summary>
        /// <param name="nation">The <see cref="Nation"/> object representing the nation to be added.</param>
        private void AddNation(Nation nation)
        {
            this.Nations.Add(nation);

            var nationItem = new ListViewItem(nation.Name);
            nationItem.ImageIndex = (int)NationStatus.Pending;
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, "Attempting to retrieve the nation's status..."));

            listView.Items.Add(nationItem);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += this.RetrieveStatus;
            worker.RunWorkerCompleted += this.UpdateStatus;

            worker.RunWorkerAsync(nationItem);
        }

        /// <summary>
        /// This method is invoked when the Edit tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                this.EditNation(item);
            }
        }

        /// <summary>
        /// Edits a nation.
        /// </summary>
        /// <param name="nationItem">The <see cref="ListViewItem"/> object representing the nation to be edited.</param>
        private void EditNation(ListViewItem nationItem)
        {
            var nation = (Nation)nationItem.Tag;

            var nationDialog = new NationDialog();
            nationDialog.Nation = nation.Name;
            nationDialog.Password = nation.Password;

            if (nationDialog.ShowDialog() == DialogResult.OK)
            {
                nation.Name = nationDialog.Nation;
                nation.Password = nationDialog.Password;

                this.RefreshNation(nationItem);
            }
        }

        /// <summary>
        /// This method is invoked when the Remove tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                this.RemoveNation(item);
            }
        }

        /// <summary>
        /// Removes a puppet from the list of puppets.
        /// </summary>
        /// <param name="nationItem">The <see cref="ListViewItem"/> object representing the puppet to be removed.</param>
        private void RemoveNation(ListViewItem nationItem)
        {
            var nation = (Nation)nationItem.Tag;
            this.Nations.Remove(nation);

            listView.Items.Remove(nationItem);
        }

        /// <summary>
        /// This method is invoked when the Refresh tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                this.RefreshNation(item);
            }
        }

        /// <summary>
        /// Refreshes the status of a nation.
        /// </summary>
        /// <param name="nationItem">The <see cref="ListViewItem"/> object representing the nation whose status is to be refreshed.</param>
        private void RefreshNation(ListViewItem nationItem)
        {
            var nation = (Nation)nationItem.Tag;

            nationItem.ImageIndex = (int)NationStatus.Pending;
            nationItem.SubItems.RemoveAt(1);
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, "Attempting to retrieve the nation's status..."));

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += this.RetrieveStatus;
            worker.RunWorkerCompleted += this.UpdateStatus;

            worker.RunWorkerAsync(nationItem);
        }

        /// <summary>
        /// Retrieves the status of a nation. This method is invoked by a BackgroundWorker when it begins to do work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="DoWorkEventArgs"/> object that contains the event data.</param>
        private void RetrieveStatus(object sender, DoWorkEventArgs e)
        {
            var nationItem = (ListViewItem)e.Argument;
            var nation = (Nation)nationItem.Tag;

            NationStatus statusType = default(NationStatus);
            string statusText = null;
            lock (Locker)
            {
                try
                {
                    var shards = new Api.NationShards();
                    shards.LastLogin = true;

                    var nationData = this.NationStatesApi.CreateNationApiRequest(nation.Name, shards);

                    statusType = NationStatus.Exists;
                    statusText = "This nation exists and was last logged into on " + nationData.LastLogin.Value.ToLocalTime().ToShortDateString() + " " + nationData.LastLogin.Value.ToLocalTime().ToShortTimeString() + ".";

                    e.Result = new Tuple<NationStatus, string, ListViewItem>(statusType, statusText, nationItem);
                    return;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is WebException)
                    {
                        var webEx = (WebException)ex.InnerException;
                        if (webEx.Response != null && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.NotFound)
                        {
                            statusType = NationStatus.DoesNotExist;
                            statusText = "This nation does not exist. The nation may have ceased to exist or it may never have been created.";

                            e.Result = new Tuple<NationStatus, string, ListViewItem>(statusType, statusText, nationItem);
                            return;
                        }
                    }

                    statusType = NationStatus.Unknown;
                    statusText = "This nation's status could not be determined. Are you connected to the Internet?";

                    e.Result = new Tuple<NationStatus, string, ListViewItem>(statusType, statusText, nationItem);
                    return;
                }
            }
        }
        
        /// <summary>
        /// Updates the status of a nation once it has been retrieved. This method is invoked by a BackgroundWorker when it finishes doing work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="RunWorkerCompletedEventArgs"/> object that contains the event data.</param>
        private void UpdateStatus(object sender, RunWorkerCompletedEventArgs e)
        {
            var statusTuple = (Tuple<NationStatus, string, ListViewItem>)e.Result;
            var statusType = statusTuple.Item1;
            var statusText = statusTuple.Item2;
            var nationItem = statusTuple.Item3;

            nationItem.ImageIndex = (int)statusType;
            nationItem.SubItems.RemoveAt(1);
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, statusText));
        }

        /// <summary>
        /// This method is invoked when the Login tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void LoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                this.LoginNation(item);
            }
        }

        /// <summary>
        /// Logs into a nation.
        /// </summary>
        /// <param name="nationItem">The <see cref="ListViewItem"/> object representing the nation to log into.</param>
        private void LoginNation(ListViewItem nationItem)
        {
            var nation = (Nation)nationItem.Tag;

            nationItem.ImageIndex = (int)NationStatus.Pending;
            nationItem.SubItems.RemoveAt(1);
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, "Attempting to log into nation..."));

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += this.AttemptLogin;
            worker.RunWorkerCompleted += this.CompleteLogin;

            worker.RunWorkerAsync(new Tuple<string, string, ListViewItem>(nation.Name, nation.Password, nationItem));
        }

        /// <summary>
        /// Attempts to log into a nation on the NationStates website. This method is invoked by a BackgroundWorker when it begins to do work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="DoWorkEventArgs"/> object that contains the event data.</param>
        private void AttemptLogin(object sender, DoWorkEventArgs e)
        {
            var loginInformation = (Tuple<string, string, ListViewItem>)e.Argument;
            var nation = loginInformation.Item1;
            var password = loginInformation.Item2;
            var nationItem = loginInformation.Item3;

            lock (Locker)
            {
                try
                {
                    CookieContainer cookieContainer = new CookieContainer();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.nationstates.net/");
                    request.Method = "POST";
                    request.UserAgent = this.UserAgent;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.CookieContainer = cookieContainer;

                    while (DateTime.UtcNow.Ticks - this.LastLoginAttemptTime.Ticks < this.LoginAttemptDelay * TimeSpan.TicksPerMillisecond)
                    {
                    }

                    Stream requestStream = request.GetRequestStream();
                    StreamWriter requestStreamWriter = new StreamWriter(requestStream);
                    requestStreamWriter.Write("logging_in=1&nation=" + WebUtility.UrlEncode(nation.Trim()) + "&password=" + WebUtility.UrlEncode(password));
                    requestStreamWriter.Close();
                    requestStream.Close();

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    response.Close();

                    this.LastLoginAttemptTime = DateTime.UtcNow;

                    if (cookieContainer.GetCookies(new Uri("http://www.nationstates.net")).Count == 0)
                    {
                        MessageBox.Show("An error occurred while logging into the nation " + nation + ". Did you enter the correct username and password?", "NationStates Nation Manager");
                    }
                }
                catch (Exception)
                {
                }
            }

            e.Result = nationItem;
        }

        /// <summary>
        /// Updates the login status of a nation once it has been retrieved. This method is invoked by a BackgroundWorker when it finishes doing work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="RunWorkerCompletedEventArgs"/> object that contains the event data.</param>
        private void CompleteLogin(object sender, RunWorkerCompletedEventArgs e)
        {
            var nationItem = (ListViewItem)e.Result;

            this.RefreshNation(nationItem);
        }

        /// <summary>
        /// This method is invoked when the Restore tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Limited to one nation per click, per NationStates scripting rules
            if (listView.SelectedItems.Count > 0)
            {
                this.RestoreNation(listView.SelectedItems[0]);
            }
        }

        /// <summary>
        /// Restores a nation that has ceased to exist.
        /// </summary>
        /// <param name="nationItem">The <see cref="ListViewItem"/> object representing the nation to restore.</param>
        private void RestoreNation(ListViewItem nationItem)
        {
            var nation = (Nation)nationItem.Tag;

            nationItem.ImageIndex = (int)NationStatus.Pending;
            nationItem.SubItems.RemoveAt(1);
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, "Attempting to restore nation..."));

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += this.AttemptRestore;
            worker.RunWorkerCompleted += this.CompleteRestore;

            worker.RunWorkerAsync(new Tuple<string, string, ListViewItem>(nation.Name, nation.Password, nationItem));
        }

        /// <summary>
        /// Attempts to restore a nation on the NationStates website. This method is invoked by a BackgroundWorker when it begins to do work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="DoWorkEventArgs"/> object that contains the event data.</param>
        private void AttemptRestore(object sender, DoWorkEventArgs e)
        {
            var loginInformation = (Tuple<string, string, ListViewItem>)e.Argument;
            var nation = loginInformation.Item1;
            var password = loginInformation.Item2;
            var nationItem = loginInformation.Item3;

            lock (Locker)
            {
                try
                {
                    CookieContainer cookieContainer = new CookieContainer();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.nationstates.net/");
                    request.Method = "POST";
                    request.UserAgent = this.UserAgent;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.CookieContainer = cookieContainer;

                    while (DateTime.UtcNow.Ticks - this.LastLoginAttemptTime.Ticks < this.LoginAttemptDelay * TimeSpan.TicksPerMillisecond)
                    {
                    }

                    Stream requestStream = request.GetRequestStream();
                    StreamWriter requestStreamWriter = new StreamWriter(requestStream);
                    requestStreamWriter.Write("logging_in=1&nation=" + WebUtility.UrlEncode(nation.Trim()) + "&restore_nation=" + WebUtility.UrlEncode(" Restore " + nation.Trim() + " ") + "&restore_password=" + WebUtility.UrlEncode(password));
                    requestStreamWriter.Close();
                    requestStream.Close();

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    response.Close();

                    this.LastLoginAttemptTime = DateTime.UtcNow;

                    if (cookieContainer.GetCookies(new Uri("http://www.nationstates.net")).Count == 0)
                    {
                        MessageBox.Show("An error occurred while restoring the nation " + nation + ". Did you enter the correct username and password?", "NationStates Nation Manager");
                    }
                }
                catch (Exception)
                {
                }
            }

            e.Result = nationItem;
        }

        /// <summary>
        /// Updates the restore status of a nation once it has been retrieved. This method is invoked by a BackgroundWorker when it finishes doing work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="RunWorkerCompletedEventArgs"/> object that contains the event data.</param>
        private void CompleteRestore(object sender, RunWorkerCompletedEventArgs e)
        {
            var nationItem = (ListViewItem)e.Result;

            this.RefreshNation(nationItem);
        }

        /// <summary>
        /// This method is invoked when the Select All tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                this.SelectNation(item);
            }
        }

        /// <summary>
        /// Selects a nation.
        /// </summary>
        /// <param name="nationItem">The <see cref="ListViewItem"/> object representing the nation to be selected.</param>
        private void SelectNation(ListViewItem nationItem)
        {
            nationItem.Selected = true;
        }

        /// <summary>
        /// This method is invoked when the Options tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowOptionsDialog();
        }

        /// <summary>
        /// Shows an instance of the <see cref="OptionsDialog"/>.
        /// </summary>
        /// <returns>The <see cref="DialogResult"/> returned by the <see cref="OptionsDialog"/> instance.</returns>
        private DialogResult ShowOptionsDialog()
        {
            var optionsDialog = new OptionsDialog();

            optionsDialog.UserAgent = this.UserAgent;
            optionsDialog.ApiRequestDelay = this.ApiRequestDelay;
            optionsDialog.LoginAttemptDelay = this.LoginAttemptDelay;

            var dialogResult = optionsDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                this.UserAgent = optionsDialog.UserAgent;
                this.ApiRequestDelay = optionsDialog.ApiRequestDelay;
                this.LoginAttemptDelay = optionsDialog.LoginAttemptDelay;

                Properties.Settings.Default.Save();
            }

            return dialogResult;
        }

        /// <summary>
        /// This method is invoked when the About tool strip menu item is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog();
        }

        /// <summary>
        /// This method is invoked when the context menu strip is opening.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="CancelEventArgs"/> object that contains the event data.</param>
        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = listView.SelectedItems.Count < 1;
        }

        /// <summary>
        /// This method is invoked when the selected index of the list view changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            editToolStripMenuItem.Enabled = listView.SelectedItems.Count == 1;
            editToolStripMenuItem1.Enabled = listView.SelectedItems.Count == 1;

            removeToolStripMenuItem.Enabled = listView.SelectedItems.Count > 0;
            removeToolStripMenuItem1.Enabled = listView.SelectedItems.Count > 0;

            refreshToolStripMenuItem.Enabled = listView.SelectedItems.Count > 0;
            refreshToolStripMenuItem1.Enabled = listView.SelectedItems.Count > 0;

            bool allExist = true;
            foreach (ListViewItem item in listView.SelectedItems)
            {
                if (item.ImageIndex != (int)NationStatus.Exists)
                {
                    allExist = false;
                    break;
                }
            }

            loginToolStripMenuItem.Enabled = allExist;
            loginToolStripMenuItem1.Enabled = allExist;

            restoreToolStripMenuItem.Enabled = listView.SelectedItems.Count == 1 && listView.SelectedItems[0].ImageIndex == (int)NationStatus.DoesNotExist;
            restoreToolStripMenuItem1.Enabled = listView.SelectedItems.Count == 1 && listView.SelectedItems[0].ImageIndex == (int)NationStatus.DoesNotExist;
        }

        /// <summary>
        /// Generates an encryption key based on a password.
        /// </summary>
        /// <param name="password">The password upon which to base the encryption key.</param>
        /// <returns>The encryption key.</returns>
        private byte[] GenerateKeyFromPassword(string password)
        {
            byte[] salt = { 29, 123, 254, 12, 39, 48, 92, 189 };

            var deriveBytes = new Rfc2898DeriveBytes(password, salt);

            return deriveBytes.GetBytes(16);
        }

        /// <summary>
        /// Generates a random encryption initialization vector.
        /// </summary>
        /// <returns>An initialization vector.</returns>
        private byte[] GenerateInitializationVector()
        {
            var aesCryptoServiceProvider = new AesCryptoServiceProvider();
            aesCryptoServiceProvider.GenerateIV();

            return aesCryptoServiceProvider.IV;
        }
    }
}
