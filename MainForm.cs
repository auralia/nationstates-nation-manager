//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Auralia">
//     Copyright (C) 2013 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.NationManager
{
    using System;
    using System.Collections;
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

            this.Workers = new List<Tuple<ListViewItem, BackgroundWorker>>();

            this.listView.Sorting = SortOrder.Descending;
            this.listView.Tag = 1;
            this.listView.ListViewItemSorter = new ListViewItemComparer(1, SortOrder.Descending);
        }

        /// <summary>
        /// Represents the different status icons for a nation's list view entry.
        /// </summary>
        private enum ListViewItemIcon
        {
            /// <summary>
            /// Represents a successful status.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Represents a failure status.
            /// </summary>
            Failure = 1,

            /// <summary>
            /// Represents a warning status.
            /// </summary>
            Warning = 2,

            /// <summary>
            /// Represents a pending status.
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
        /// Gets or sets a list of <see cref="BackgroundWorker"/> objects associated with particular <see cref="ListViewItem"/> objects.
        /// </summary>
        /// <value>A list of <see cref="BackgroundWorker"/> objects associated with particular <see cref="ListViewItem"/> objects.</value>
        private List<Tuple<ListViewItem, BackgroundWorker>> Workers
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

            if (this.OpenFileStream != null)
            {
                this.OpenFileStream.Close();
            }

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
                    this.OpenFileStream.Flush();
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
                        fileStream.Flush();
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
            nationItem.Tag = nation;
            nationItem.ImageIndex = (int)ListViewItemIcon.Pending;
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, "?"));
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, "?"));
            nationItem.SubItems.Add(new ListViewItem.ListViewSubItem(nationItem, "Attempting to retrieve this nation's information..."));

            listView.Items.Add(nationItem);

            for (var i = this.Workers.Count - 1; i >= 0; i--)
            {
                if (this.Workers[i].Item1 == nationItem)
                {
                    this.Workers[i].Item2.CancelAsync();
                    this.Workers.RemoveAt(i);
                }
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += this.RetrieveStatus;
            worker.RunWorkerCompleted += this.CompleteListViewItemUpdate;

            this.Workers.Add(new Tuple<ListViewItem, BackgroundWorker>(nationItem, worker));

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
            for (var i = this.Workers.Count - 1; i >= 0; i--)
            {
                if (this.Workers[i].Item1 == nationItem)
                {
                    this.Workers[i].Item2.CancelAsync();
                    this.Workers.RemoveAt(i);
                }
            }

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

            nationItem.ImageIndex = (int)ListViewItemIcon.Pending;
            nationItem.SubItems[1].Text = "?";
            nationItem.SubItems[2].Text = "?";
            nationItem.SubItems[3].Text = "Attempting to retrieve this nation's information...";

            for (var i = this.Workers.Count - 1; i >= 0; i--)
            {
                if (this.Workers[i].Item1 == nationItem)
                {
                    this.Workers[i].Item2.CancelAsync();
                    this.Workers.RemoveAt(i);
                }
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += this.RetrieveStatus;
            worker.RunWorkerCompleted += this.CompleteListViewItemUpdate;

            this.Workers.Add(new Tuple<ListViewItem, BackgroundWorker>(nationItem, worker));

            worker.RunWorkerAsync(nationItem);
        }

        /// <summary>
        /// Retrieves the status of a nation. This method is invoked by a BackgroundWorker when it begins to do work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="DoWorkEventArgs"/> object that contains the event data.</param>
        private void RetrieveStatus(object sender, DoWorkEventArgs e)
        {
            if (((BackgroundWorker)sender).CancellationPending)
            {
                e.Result = null;
                return;
            }

            var nationItem = (ListViewItem)e.Argument;
            var nation = (Nation)nationItem.Tag;

            lock (Locker)
            {
                if (((BackgroundWorker)sender).CancellationPending)
                {
                    e.Result = null;
                    return;
                }

                var nationInformation = this.RetrieveNationInformation(nation.Name);

                var exists = nationInformation.Item1;
                var lastLogin = nationInformation.Item2;

                if (!exists.HasValue)
                {
                    var icon = ListViewItemIcon.Warning;
                    var statusText = "A network error occured while attempting to retrieve this nation's information. Are you connected to the Internet?";
                    
                    e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                }
                else if (exists.Value) 
                {
                    var icon = ListViewItemIcon.Success;
                    var statusText = string.Empty;

                    e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                }
                else 
                {
                    var icon = ListViewItemIcon.Failure;
                    var statusText = string.Empty;

                    e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                }
            }
        }

        /// <summary>
        /// Retrieves information about a nation, specifically whether or not the nation exists and the last login date and time for the nation.
        /// </summary>
        /// <param name="nation">The nation's name.</param>
        /// <returns>A <see cref="Tuple"/> object representing whether or not the nation exists and the last login date and time for the nation.</returns>
        private Tuple<bool?, DateTime?> RetrieveNationInformation(string nation)
        {
            bool? exists = null;
            DateTime? lastLogin = null;

            try
            {
                var shards = new Api.NationShards();
                shards.LastLogin = true;

                var nationData = this.NationStatesApi.CreateNationApiRequest(nation, shards);

                exists = true;
                lastLogin = nationData.LastLogin.Value;

                return new Tuple<bool?, DateTime?>(exists, lastLogin);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is WebException)
                {
                    var webEx = (WebException)ex.InnerException;
                    if (webEx.Response != null && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        exists = false;
                        lastLogin = null;

                        return new Tuple<bool?, DateTime?>(exists, lastLogin);
                    }
                }

                exists = null;
                lastLogin = null;

                return new Tuple<bool?, DateTime?>(exists, lastLogin);
            }
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

            nationItem.ImageIndex = (int)ListViewItemIcon.Pending;
            nationItem.SubItems[3].Text = "Attempting to log into this nation...";

            for (var i = this.Workers.Count - 1; i >= 0; i--)
            {
                if (this.Workers[i].Item1 == nationItem)
                {
                    this.Workers[i].Item2.CancelAsync();
                    this.Workers.RemoveAt(i);
                }
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += this.AttemptLogin;
            worker.RunWorkerCompleted += this.CompleteListViewItemUpdate;

            this.Workers.Add(new Tuple<ListViewItem, BackgroundWorker>(nationItem, worker));

            worker.RunWorkerAsync(new Tuple<string, string, ListViewItem>(nation.Name, nation.Password, nationItem));
        }

        /// <summary>
        /// Attempts to log into a nation on the NationStates website. This method is invoked by a BackgroundWorker when it begins to do work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="DoWorkEventArgs"/> object that contains the event data.</param>
        private void AttemptLogin(object sender, DoWorkEventArgs e)
        {
            if (((BackgroundWorker)sender).CancellationPending)
            {
                e.Result = null;
                return;
            }

            var loginInformation = (Tuple<string, string, ListViewItem>)e.Argument;
            var nation = loginInformation.Item1;
            var password = loginInformation.Item2;
            var nationItem = loginInformation.Item3;

            ListViewItemIcon icon = default(ListViewItemIcon);
            bool? exists = null;
            DateTime? lastLogin = null;
            string statusText = null;
            lock (Locker)
            {
                try
                {
                    if (((BackgroundWorker)sender).CancellationPending)
                    {
                        e.Result = null;
                        return;
                    }

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

                    var statusTuple = this.RetrieveNationInformation(nation);
                    exists = statusTuple.Item1;
                    lastLogin = statusTuple.Item2;

                    if (cookieContainer.GetCookies(new Uri("http://www.nationstates.net")).Count == 0)
                    {
                        icon = ListViewItemIcon.Failure;
                        statusText = "An error occured while attempting to log into this nation. Did you enter the correct username and password?";

                        e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                        return;
                    }
                    else
                    {
                        icon = ListViewItemIcon.Success;
                        statusText = "Successfully logged into this nation on " + DateTime.Now.ToLocalTime().ToShortDateString() + " at " + DateTime.Now.ToLocalTime().ToShortTimeString() + ".";

                        e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                        return;
                    }
                }
                catch (Exception)
                {
                    icon = ListViewItemIcon.Warning;
                    statusText = "A network error occured while attempting to log into this nation. Are you connected to the Internet?";

                    e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                    return;
                }
            }
        }

        /// <summary>
        /// Updates the list view item for a nation. This method is invoked by a BackgroundWorker when it finishes doing work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="RunWorkerCompletedEventArgs"/> object that contains the event data.</param>
        private void CompleteListViewItemUpdate(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var tuple = (Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>)e.Result;
                var icon = tuple.Item1;
                var exists = tuple.Item2;
                var lastLogin = tuple.Item3;
                var statusText = tuple.Item4;
                var nationItem = tuple.Item5;

                nationItem.ImageIndex = (int)icon;

                if (exists == null)
                {
                    nationItem.SubItems[1].Text = "?";
                }
                else if (exists.Value)
                {
                    nationItem.SubItems[1].Text = "Yes";
                }
                else
                {
                    nationItem.SubItems[1].Text = "No";
                }

                if (lastLogin == null)
                {
                    nationItem.SubItems[2].Text = "?";
                }
                else
                {
                    nationItem.SubItems[2].Text = lastLogin.Value.ToLocalTime().ToShortDateString() + " " + lastLogin.Value.ToLocalTime().ToShortTimeString();
                }

                if (statusText == null)
                {
                    nationItem.SubItems[3].Text = string.Empty;
                }
                else
                {
                    nationItem.SubItems[3].Text = statusText;
                }

                listView.Sort();
            }

            for (var i = this.Workers.Count - 1; i >= 0; i--)
            {
                if (this.Workers[i].Item2 == sender)
                {
                    this.Workers.RemoveAt(i);
                    break;
                }
            }
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

            nationItem.ImageIndex = (int)ListViewItemIcon.Pending;
            nationItem.SubItems[1].Text = "?";
            nationItem.SubItems[2].Text = "?";
            nationItem.SubItems[3].Text = "Attempting to restore this nation...";

            for (var i = this.Workers.Count - 1; i >= 0; i--)
            {
                if (this.Workers[i].Item1 == nationItem)
                {
                    this.Workers[i].Item2.CancelAsync();
                    this.Workers.RemoveAt(i);
                }
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += this.AttemptRestore;
            worker.RunWorkerCompleted += this.CompleteListViewItemUpdate;

            this.Workers.Add(new Tuple<ListViewItem, BackgroundWorker>(nationItem, worker));

            worker.RunWorkerAsync(new Tuple<string, string, ListViewItem>(nation.Name, nation.Password, nationItem));
        }

        /// <summary>
        /// Attempts to restore a nation on the NationStates website. This method is invoked by a BackgroundWorker when it begins to do work.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="DoWorkEventArgs"/> object that contains the event data.</param>
        private void AttemptRestore(object sender, DoWorkEventArgs e)
        {
            if (((BackgroundWorker)sender).CancellationPending)
            {
                e.Result = null;
                return;
            }

            var loginInformation = (Tuple<string, string, ListViewItem>)e.Argument;
            var nation = loginInformation.Item1;
            var password = loginInformation.Item2;
            var nationItem = loginInformation.Item3;

            ListViewItemIcon icon = default(ListViewItemIcon);
            bool? exists = null;
            DateTime? lastLogin = null;
            string statusText = null;
            lock (Locker)
            {
                try
                {
                    if (((BackgroundWorker)sender).CancellationPending)
                    {
                        e.Result = null;
                        return;
                    }

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

                    var statusTuple = this.RetrieveNationInformation(nation);
                    exists = statusTuple.Item1;
                    lastLogin = statusTuple.Item2;

                    if (cookieContainer.GetCookies(new Uri("http://www.nationstates.net")).Count == 0)
                    {
                        icon = ListViewItemIcon.Failure;
                        statusText = "An error occured while attempting to restore this nation. Did you enter the correct username and password?";

                        e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                        return;
                    }
                    else
                    {
                        icon = ListViewItemIcon.Success;
                        statusText = "Successfully restored this nation on " + DateTime.Now.ToLocalTime().ToShortDateString() + " at " + DateTime.Now.ToLocalTime().ToShortTimeString() + ".";

                        e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                        return;
                    }
                }
                catch (Exception)
                {
                    icon = ListViewItemIcon.Warning;
                    statusText = "A network error occured while attempting to restore this nation. Are you connected to the Internet?";

                    e.Result = new Tuple<ListViewItemIcon, bool?, DateTime?, string, ListViewItem>(icon, exists, lastLogin, statusText, nationItem);
                    return;
                }
            }
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
                if (!item.SubItems[1].Text.Equals("Yes"))
                {
                    allExist = false;
                    break;
                }
            }

            if (listView.SelectedItems.Count == 0)
            {
                allExist = false;
            }

            loginToolStripMenuItem.Enabled = allExist;
            loginToolStripMenuItem1.Enabled = allExist;

            restoreToolStripMenuItem.Enabled = listView.SelectedItems.Count == 1 && listView.SelectedItems[0].SubItems[1].Text.Equals("No");
            restoreToolStripMenuItem1.Enabled = listView.SelectedItems.Count == 1 && listView.SelectedItems[0].SubItems[1].Text.Equals("No");
        }
        
        /// <summary>
        /// This method is invoked when a list view column is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">A <see cref="ColumnClickEventArgs"/> object that contains the event data.</param>
        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var lastColumn = (int)listView.Tag;

            if (e.Column != lastColumn)
            {
                listView.Tag = e.Column;
                listView.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (listView.Sorting == SortOrder.Ascending)
                {
                    listView.Sorting = SortOrder.Descending;
                }
                else
                {
                    listView.Sorting = SortOrder.Ascending;
                }
            }

            this.listView.ListViewItemSorter = new ListViewItemComparer(e.Column, listView.Sorting);
            this.listView.Sort();
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

        /// <summary>
        /// Represents a comparer for list view items.
        /// </summary>
        private class ListViewItemComparer : IComparer
        {
            /// <summary>
            /// The column containing the list view items to be sorted.
            /// </summary>
            private int column;

            /// <summary>
            /// The order in which the list view items are to be sorted.
            /// </summary>
            private SortOrder order;

            /// <summary>
            /// Initializes a new instance of the <see cref="ListViewItemComparer"/> class.
            /// </summary>
            public ListViewItemComparer()
            {
                this.column = 0;
                this.order = SortOrder.Ascending;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ListViewItemComparer"/> class with the column containing the list view items to be sorted and the order in which the list view items are to be sorted.
            /// </summary>
            /// <param name="column">The column containing the list view items to be sorted.</param>
            /// <param name="order">The order in which the list view items are to be sorted.</param>
            public ListViewItemComparer(int column, SortOrder order)
            {
                this.column = column;
                this.order = order;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>An integer that indicates the relative values of x and y corresponding to whether one is less than, equal to, or greater than the other.</returns>
            public int Compare(object x, object y) 
            {
                int compareValue = string.Compare(((ListViewItem)x).SubItems[this.column].Text, ((ListViewItem)y).SubItems[this.column].Text);

                if (this.order == SortOrder.Descending)
                {
                    compareValue *= -1;
                }

                if (compareValue == 0 && this.column != 0)
                {
                    compareValue = string.Compare(((ListViewItem)x).SubItems[0].Text, ((ListViewItem)y).SubItems[0].Text);
                }

                return compareValue;
            }
        }
    }
}
