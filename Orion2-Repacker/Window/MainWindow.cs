/*
 *      This file is part of Orion2, a MapleStory2 Packaging Library Project.
 *      Copyright (C) 2018 Eric Smith <notericsoft@gmail.com>
 *
 *      This program is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 *
 *      This program is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU General Public License for more details.
 *
 *      You should have received a copy of the GNU General Public License
 */

using System.ComponentModel;
using System.IO.MemoryMappedFiles;
using System.Text;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using Orion.Crypto.Common;
using Orion.Crypto.Stream;
using Orion.Crypto.Stream.DDS;
using Orion.Window.Common;
using static Orion.Crypto.CryptoMan;

namespace Orion.Window
{
    public partial class MainWindow : Form
    {
        private ExtractWindow extractWindow;
        private MemoryMappedFile pDataMappedMemFile;
        private PackNodeList pNodeList;
        private ProgressWindow pProgress;
        private string sHeaderUOL;

        public MainWindow()
        {
            InitializeComponent();

            pImagePanel.AutoScroll = true;

            pImageData.BorderStyle = BorderStyle.None;
            pImageData.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            pMenuStrip.Renderer = new MenuRenderer();

            pPrevSize = Size;

            sHeaderUOL = "";

            pNodeList = null;
            pDataMappedMemFile = null;
            pProgress = null;

            UpdatePanel("Empty", null);

            Properties.Settings.Default.Reload();
            wordWrapToolStripMenuItem.Checked = Properties.Settings.Default.EditorWordWrap;
            if (Properties.Settings.Default.EditorTheme == "vs")
            {
                lightToolStripMenuItem.Checked = true;
            }
            else
            {
                darkToolStripMenuItem.Checked = true;
            }

            InitializeWebViewAsync();
        }

        async void InitializeWebViewAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += SaveFile;


            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"window.chrome.webview.addEventListener('message', (event) => {
                try {
                    if (event.data.type == 'updateContent') {
                        editor.setScrollTop(0, 1);
                        editor.setScrollLeft(0, 1);
                        editor.setValue(event.data.content);
                    } else if (event.data.type == 'theme') {
                        editor.updateOptions({theme: event.data.theme});
                    } else if (event.data.type == 'wordWrap') {
                        editor.updateOptions({wordWrap: event.data.wordWrap});
                    } else if (event.data.type == 'updateSettings') {
                        delete event.data.type;
                        editor.updateOptions(event.data);
                    } else if (event.data.type == 'saveFile') {
                        window.chrome.webview.postMessage(editor.getValue());
                    }
                } catch (error) {
                    alert(error);
                }
            });");
            webView.Source = new Uri(Path.Combine(Application.StartupPath, @"MonacoEditor\index.html"));
        }

        private void SaveFile(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (!(pTreeView.SelectedNode is PackNode pNode) || pNode.Data == null) return;

            if (!(pNode.Tag is PackFileEntry pEntry)) return;

            string sData = e.TryGetWebMessageAsString();
            byte[] pData = Encoding.UTF8.GetBytes(sData.ToCharArray());
            if (pNode.Data == pData) return;

            pEntry.Data = pData;
            pEntry.Changed = true;
        }

        private void InitializeTree(IPackStreamVerBase pStream)
        {
            // Insert the root node (file)
            string[] aPath = sHeaderUOL.Replace(".m2h", "").Split('/');
            pTreeView.Nodes.Add(new PackNode(pStream, aPath[aPath.Length - 1]));

            pNodeList?.InternalRelease();
            pNodeList = new PackNodeList("/");

            foreach (PackFileEntry pEntry in pStream.GetFileList())
                if (pEntry.Name.Contains("/"))
                {
                    string sPath = pEntry.Name;
                    PackNodeList pCurList = pNodeList;

                    while (sPath.Contains("/"))
                    {
                        string sDir = sPath.Substring(0, sPath.IndexOf('/') + 1);
                        if (!pCurList.Children.ContainsKey(sDir))
                        {
                            pCurList.Children.Add(sDir, new PackNodeList(sDir));
                            if (pCurList == pNodeList) pTreeView.Nodes[0].Nodes.Add(new PackNode(pCurList.Children[sDir], sDir));
                        }

                        pCurList = pCurList.Children[sDir];

                        sPath = sPath.Substring(sPath.IndexOf('/') + 1);
                    }

                    pEntry.TreeName = sPath;
                    pCurList.Entries.Add(sPath, pEntry);
                }
                else
                {
                    pEntry.TreeName = pEntry.Name;

                    pNodeList.Entries.Add(pEntry.Name, pEntry);
                    pTreeView.Nodes[0].Nodes.Add(new PackNode(pEntry, pEntry.Name));
                }

            // Sort all nodes
            pTreeView.Sort();
        }

        #region About

        private void OnAbout(object sender, EventArgs e)
        {
            About pAbout = new About
            {
                Owner = this
            };

            pAbout.ShowDialog();
        } // About

        #endregion

        #region Helpers - File

        private void AddFileEntry(PackFileEntry pEntry)
        {
            if (pTreeView.Nodes[0] is PackNode pRoot)
                if (pRoot.Tag is IPackStreamVerBase pStream)
                    pStream.GetFileList().Add(pEntry);
        }

        #endregion

        #region Helpers - Select

        private void OnSelectNode(object sender, TreeViewEventArgs e)
        {
            if (pTreeView.SelectedNode is PackNode pNode)
            {
                object pObj = pNode.Tag;
                pEntryName.Visible = true;
                pEntryName.Text = pNode.Name;
                if (pObj is PackNodeList)
                {
                    UpdatePanel("Packed Directory", null);
                }
                else if (pObj is PackFileEntry)
                {
                    PackFileEntry pEntry = pObj as PackFileEntry;
                    IPackFileHeaderVerBase pFileHeader = pEntry.FileHeader;
                    if (pFileHeader != null)
                        if (pNode.Data == null)
                            pNode.Data = DecryptData(pFileHeader, pDataMappedMemFile);
                    UpdatePanel(pEntry.TreeName.Split('.')[1].ToLower(), pNode.Data);
                }
                else if (pObj is IPackStreamVerBase)
                {
                    UpdatePanel("Packed Data File", null);
                }
                else
                {
                    UpdatePanel("Empty", null);
                }
            }
        }

        #endregion

        public static string ShowDialog(string caption)
        {
            Form prompt = new Form
            {
                Width = 290,
                Height = 80,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox textBox = new TextBox
            {
                Left = 10,
                Top = 10,
                Width = 200
            };
            Button confirmation = new Button
            {
                Text = "Ok",
                Left = 220,
                Width = 50,
                Top = 9,
                DialogResult = DialogResult.OK
            };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        #region File

        private void OnLoadFile(object sender, EventArgs e)
        {
            if (pNodeList != null)
            {
                NotifyMessage("Please unload the current file first.", MessageBoxIcon.Information);
                return;
            }

            OpenFileDialog pDialog = new OpenFileDialog
            {
                Title = "Select the MS2 file to load",
                Filter = "MapleStory2 Files|*.m2d",
                Multiselect = false,
                InitialDirectory = Properties.Settings.Default.LastInputFolder
            };

            if (pDialog.ShowDialog() != DialogResult.OK) return;

            Properties.Settings.Default.LastInputFolder = pDialog.FileName[..pDialog.FileName.LastIndexOf('\\')];
            Properties.Settings.Default.Save();

            string sDataUOL = Dir_BackSlashToSlash(pDialog.FileName);
            sHeaderUOL = sDataUOL.Replace(".m2d", ".m2h");

            string fileName = sHeaderUOL.Substring(sHeaderUOL.LastIndexOf('/') + 1);
            if (!File.Exists(sHeaderUOL))
            {
                NotifyMessage($"Unable to load the {fileName} file.\r\nPlease make sure it exists and is not being used.", MessageBoxIcon.Error);
                return;
            }

            // Window title
            Text = "Orion2 Repacker | " + pDialog.FileName;

            IPackStreamVerBase pStream;
            using (BinaryReader pHeader = new BinaryReader(File.OpenRead(sHeaderUOL)))
            {
                // Construct a new packed stream from the header data
                pStream = PackVer.CreatePackVer(pHeader);

                // Insert a collection containing the file list information [index,hash,name]
                pStream.GetFileList().Clear();
                pStream.GetFileList().AddRange(PackFileEntry.CreateFileList(Encoding.UTF8.GetString(DecryptFileString(pStream, pHeader.BaseStream))));
                // Make the collection of files sorted by their FileIndex for easy fetching
                pStream.GetFileList().Sort();

                // Load the file allocation table and assign each file header to the entry within the list
                byte[] pFileTable = DecryptFileTable(pStream, pHeader.BaseStream);
                using (MemoryStream pTableStream = new MemoryStream(pFileTable))
                {
                    using (BinaryReader pReader = new BinaryReader(pTableStream))
                    {
                        IPackFileHeaderVerBase pFileHeader;

                        switch (pStream.GetVer())
                        {
                            case PackVer.MS2F:
                                for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                {
                                    pFileHeader = new PackFileHeaderVer1(pReader);
                                    pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                }

                                break;
                            case PackVer.NS2F:
                                for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                {
                                    pFileHeader = new PackFileHeaderVer2(pReader);
                                    pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                }

                                break;
                            case PackVer.OS2F:
                            case PackVer.PS2F:
                                for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                {
                                    pFileHeader = new PackFileHeaderVer3(pStream.GetVer(), pReader);
                                    pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                }

                                break;
                        }
                    }
                }
            }

            pDataMappedMemFile = MemoryMappedFile.CreateFromFile(sDataUOL);

            InitializeTree(pStream);
        } // Open

        private void OnSaveFile(object sender, EventArgs e)
        {
            if (pTreeView.SelectedNode is PackNode pNode && pNode.Tag is IPackStreamVerBase)
            {
                SaveFileDialog pDialog = new SaveFileDialog
                {
                    Title = "Select the destination to save the file",
                    Filter = "MapleStory2 Files|*.m2d",
                    InitialDirectory = Properties.Settings.Default.LastOutputFolder
                };

                if (pDialog.ShowDialog() != DialogResult.OK) return;
                string sPath = Dir_BackSlashToSlash(pDialog.FileName);

                Properties.Settings.Default.LastOutputFolder = pDialog.FileName[..pDialog.FileName.LastIndexOf('\\')];
                Properties.Settings.Default.Save();

                if (pSaveWorkerThread.IsBusy) return;
                pProgress = new ProgressWindow
                {
                    Path = sPath,
                    Stream = pNode.Tag as IPackStreamVerBase
                };
                pProgress.Show(this);
                // Why do you make this so complicated C#?
                int x = DesktopBounds.Left + (Width - pProgress.Width) / 2;
                int y = DesktopBounds.Top + (Height - pProgress.Height) / 2;
                pProgress.SetDesktopLocation(x, y);

                pSaveWorkerThread.RunWorkerAsync();
            }
            else
            {
                NotifyMessage("Please select a Packed Data File file to save.", MessageBoxIcon.Information);
            }
        } // Save

        private void OnReloadFile(object sender, EventArgs e)
        {
            if (pNodeList != null)
            {
                if (pTreeView.Nodes.Count <= 0) return;
                IPackStreamVerBase pStream = pTreeView.Nodes[0].Tag as IPackStreamVerBase;
                if (pStream == null) return;

                pTreeView.Nodes.Clear();
                pTreeView.Refresh();

                InitializeTree(pStream);
                UpdatePanel("Empty", null);
                return;
            }

            NotifyMessage("There is no package to be reloaded.", MessageBoxIcon.Warning);
        } // Reload

        private void OnUnloadFile(object sender, EventArgs e)
        {
            if (pNodeList != null)
            {
                pTreeView.Nodes.Clear();

                pNodeList.InternalRelease();
                pNodeList = null;

                sHeaderUOL = "";
                Text = "Orion2 Repacker";

                if (pDataMappedMemFile != null)
                {
                    pDataMappedMemFile.Dispose();
                    pDataMappedMemFile = null;
                }

                UpdatePanel("Empty", null);

                GC.Collect();
                return;
            }

            NotifyMessage("There is no package to be unloaded.", MessageBoxIcon.Warning);
        } // Unload

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        } // Exit

        #endregion

        #region Edit

        private void OnAddFile(object sender, EventArgs e)
        {
            if (pNodeList is null)
            {
                NotifyMessage("Please load a file first.", MessageBoxIcon.Information);
                return;
            }

            PackNode pNode = pTreeView.SelectedNode as PackNode;
            if (pNode?.Tag is PackFileEntry)
            {
                NotifyMessage("Please select a directory to add into!", MessageBoxIcon.Exclamation);
                return;
            }

            OpenFileDialog pDialog = new OpenFileDialog
            {
                Title = "Select files to add",
                Filter = "MapleStory2 Files|*",
                Multiselect = true
            };

            if (pDialog.ShowDialog() != DialogResult.OK) return;

            foreach (string fileName in pDialog.FileNames)
            {
                string sHeaderUOL = Dir_BackSlashToSlash(fileName);
                string sHeaderName = sHeaderUOL.Substring(sHeaderUOL.LastIndexOf('/') + 1);

                if (!File.Exists(sHeaderUOL))
                {
                    NotifyMessage($"Unable to load the {sHeaderName} file.\r\nPlease make sure it exists and is not being used.",
                        MessageBoxIcon.Error);
                    return;
                }

                PackNodeList pList;
                if (pNode.Level == 0)
                    // If they're trying to add to the root of the file,
                    // then just use the root node list of this tree.
                    pList = pNodeList;
                else
                    pList = pNode.Tag as PackNodeList;

                byte[] pData = File.ReadAllBytes(fileName);

                PackFileEntry pEntry = new PackFileEntry
                {
                    Name = sHeaderName,
                    Hash = Helpers.CreateHash(sHeaderUOL),
                    Index = 1,
                    Changed = true,
                    TreeName = sHeaderName,
                    Data = pData
                };

                if (pList.Entries.ContainsKey(pEntry.TreeName))
                {
                    NotifyMessage($"The file '{pEntry.TreeName}' already exists in the directory.", MessageBoxIcon.Exclamation);
                    continue;
                }

                AddFileEntry(pEntry);
                pList.Entries.Add(pEntry.TreeName, pEntry);

                PackNode pChild = new PackNode(pEntry, pEntry.TreeName);
                pNode.Nodes.Add(pChild);

                pEntry.Name = pChild.Path;
            }
        } // Add


        private void OnAddFolder(object sender, EventArgs e)
        {
            if (pNodeList == null)
            {
                NotifyMessage("Please load an file first.", MessageBoxIcon.Information);
                return;
            }

            if (!(pTreeView.SelectedNode is PackNode pNode)) return;

            if (pNode.Tag is PackFileEntry)
            {
                NotifyMessage("Please select a directory to add into!", MessageBoxIcon.Exclamation);
                return;
            }

            PackNodeList pList = pNode.Tag as PackNodeList;

            string nodeName = ShowDialog("Type the folder name");
            if (string.IsNullOrEmpty(nodeName)) return;
            nodeName += "/";

            PackNodeList newNodeList = new PackNodeList(nodeName);
            PackNode pChild = new PackNode(newNodeList, nodeName);

            pList.Children.Add(nodeName, newNodeList);
            pNode.Nodes.Add(pChild);
        }

        private void OnRemoveFile(object sender, EventArgs e)
        {
            if (!(pTreeView.SelectedNode is PackNode pNode))
            {
                NotifyMessage("Please select a file or directory to remove.", MessageBoxIcon.Exclamation);
                return;
            }

            if (!(pTreeView.Nodes[0] is PackNode pRoot) || pNode == pRoot) return;

            if (!(pRoot.Tag is IPackStreamVerBase pStream)) return;

            switch (pNode.Tag)
            {
                case PackFileEntry pEntry:
                    {
                        pStream.GetFileList().Remove(pEntry);
                        if (pNode.Parent == pRoot)
                            pNodeList.Entries.Remove(pEntry.TreeName);
                        else
                            (pNode.Parent.Tag as PackNodeList).Entries.Remove(pEntry.TreeName);
                        pNode.Parent.Nodes.Remove(pNode);
                        break;
                    }
                case PackNodeList _:
                    {
                        const string sWarning = "WARNING: You are about to delete an entire directory!" +
                                                "\r\nBy deleting this directory, all inner directories and entries will also be removed." +
                                                "\r\n\r\nAre you sure you want to continue?";
                        if (MessageBox.Show(this, sWarning, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            RemoveDirectory(pNode, pStream);
                            if (pNode.Parent == pRoot)
                                pNodeList.Children.Remove(pNode.Name);
                            else
                                (pNode.Parent.Tag as PackNodeList).Children.Remove(pNode.Name);
                            pNode.Remove();
                        }

                        break;
                    }
            }
        } // Remove

        private void OnCopyNode(object sender, EventArgs e)
        {
            if (!(pTreeView.SelectedNode is PackNode pNode))
            {
                NotifyMessage("Please select the node you wish to copy.", MessageBoxIcon.Exclamation);
                return;
            }

            switch (pNode.Tag)
            {
                case PackFileEntry pEntry:
                    // Clear any current data from clipboard.
                    Clipboard.Clear();
                    // Copy the new copied entry object to clipboard.
                    Clipboard.SetData(PackFileEntry.DATA_FORMAT, pEntry.CreateCopy());
                    break;
                case PackNodeList pList:
                    {
                        PackNodeList pListCopy = new PackNodeList(pList.Directory);
                        foreach (PackFileEntry pChild in pList.Entries.Values)
                        {
                            byte[] pBlock = DecryptData(pChild.FileHeader, pDataMappedMemFile);
                            pListCopy.Entries.Add(pChild.TreeName, pChild.CreateCopy(pBlock));
                        }

                        Clipboard.Clear();
                        Clipboard.SetData(PackNodeList.DATA_FORMAT, pListCopy);
                        break;
                    }
            }
        } // Copy

        private void OnPasteNode(object sender, EventArgs e)
        {
            IDataObject pData = Clipboard.GetDataObject();
            if (pData == null) return;

            if (!(pTreeView.SelectedNode is PackNode pNode)) return;

            if (pNode.Tag is PackFileEntry) //wtf are they thinking?
            {
                NotifyMessage("Please select a directory to paste into!", MessageBoxIcon.Exclamation);
                return;
            }

            object pObj;
            if (pData.GetDataPresent(PackFileEntry.DATA_FORMAT))
            {
                pObj = (PackFileEntry)pData.GetData(PackFileEntry.DATA_FORMAT);
            }
            else if (pData.GetDataPresent(PackNodeList.DATA_FORMAT))
            {
                pObj = (PackNodeList)pData.GetData(PackNodeList.DATA_FORMAT);
            }
            else
            {
                NotifyMessage("No files or directories are currently copied to clipboard.", MessageBoxIcon.Exclamation);
                return;
            }

            PackNodeList pList;
            if (pNode.Level == 0)
                // If they're trying to add to the root of the file,
                // then just use the root node list of this tree.
                pList = pNodeList;
            else
                pList = pNode.Tag as PackNodeList;

            if (pList == null || pObj == null) return;
            switch (pObj)
            {
                case PackFileEntry obj:
                    {
                        if (pList.Entries.ContainsKey(obj.TreeName))
                        {
                            NotifyMessage("File name already exists in directory.", MessageBoxIcon.Exclamation);
                            return;
                        }

                        AddFileEntry(obj);
                        pList.Entries.Add(obj.TreeName, obj);

                        PackNode pChild = new PackNode(obj, obj.TreeName);
                        pNode.Nodes.Add(pChild);

                        obj.Name = pChild.Path;
                        break;
                    }
                case PackNodeList obj:
                    {
                        PackNode pChild = new PackNode(obj, obj.Directory);
                        pList.Children.Add(obj.Directory, obj);
                        pNode.Nodes.Add(pChild);

                        foreach (PackFileEntry pEntry in obj.Entries.Values)
                        {
                            AddFileEntry(pEntry);
                            PackNode pListNode = new PackNode(pEntry, pEntry.TreeName);
                            pChild.Nodes.Add(pListNode);

                            pEntry.Name = pListNode.Path;
                        }

                        break;
                    }
            }
        } // Paste

        private void OnExpandNodes(object sender, EventArgs e) => pTreeView.ExpandAll();

        private void OnCollapseNodes(object sender, EventArgs e) => pTreeView.CollapseAll();

        #endregion

        #region Tools

        private void OnExport(object sender, EventArgs e)
        {
            if (!(pTreeView.SelectedNode is PackNode pNode))
            {
                NotifyMessage("Please select a file to export.", MessageBoxIcon.Asterisk);
                return;
            }

            switch (pNode.Tag)
            {
                case PackNodeList tag:
                    {
                        FolderBrowserDialog pDialog = new FolderBrowserDialog
                        {
                            Description = "Select the destination folder to export to",
                            InitialDirectory = Properties.Settings.Default.LastExportFolder
                        };

                        if (pDialog.ShowDialog() != DialogResult.OK)
                        {
                            break;
                        }

                        Properties.Settings.Default.LastExportFolder = pDialog.SelectedPath + "\\";
                        Properties.Settings.Default.Save();

                        StringBuilder sPath = new StringBuilder(Dir_BackSlashToSlash(pDialog.SelectedPath)).Append("/");
                        PackNode pParent = pNode.Parent as PackNode;
                        while (pParent?.Tag is PackNodeList)
                        {
                            sPath.Append(pParent.Name);

                            pParent = pParent.Parent as PackNode;
                        }

                        sPath.Append(pNode.Name);

                        OnExportNodeList(sPath.ToString(), tag);

                        NotifyMessage($"Successfully exported to {sPath}", MessageBoxIcon.Information);

                        break;
                    }
                case IPackStreamVerBase _:
                    {
                        FolderBrowserDialog pDialog = new FolderBrowserDialog
                        {
                            Description = "Select the destination folder to export to",
                            InitialDirectory = Properties.Settings.Default.LastExportFolder
                        };

                        if (pDialog.ShowDialog() != DialogResult.OK)
                        {
                            break;
                        }

                        Properties.Settings.Default.LastExportFolder = pDialog.SelectedPath + "\\";
                        Properties.Settings.Default.Save();

                        StringBuilder sPath = new StringBuilder(Dir_BackSlashToSlash(pDialog.SelectedPath));
                        sPath.Append("/");
                        sPath.Append(pNode.Name);
                        sPath.Append("/");
                        // Create root directory
                        if (!Directory.Exists(sPath.ToString())) Directory.CreateDirectory(sPath.ToString());

                        extractWindow = new ExtractWindow
                        {
                            Path = sPath.ToString(),
                            PackNode = pNode
                        };

                        extractWindow.Show(this);
                        // Why do you make this so complicated C#?
                        int x = DesktopBounds.Left + (Width - extractWindow.Width) / 2;
                        int y = DesktopBounds.Top + (Height - extractWindow.Height) / 2;
                        extractWindow.SetDesktopLocation(x, y);
                        extractWindow.SetProgressBarSize(pNode.Nodes.Count);

                        extractWorkerThread.WorkerReportsProgress = true;
                        extractWorkerThread.RunWorkerAsync();

                        break;
                    }
                case PackFileEntry tag:
                    {
                        string sName = tag.TreeName.Split('.')[0];
                        string sExtension = tag.TreeName.Split('.')[1];

                        SaveFileDialog pDialog = new SaveFileDialog
                        {
                            Title = "Select the destination to export the file",
                            FileName = sName,
                            Filter = $"{sExtension.ToUpper()} File|*.{sExtension}",
                            InitialDirectory = Properties.Settings.Default.LastExportFolder
                        };

                        if (pDialog.ShowDialog() != DialogResult.OK)
                        {
                            break;
                        }

                        Properties.Settings.Default.LastExportFolder = pDialog.FileName[..pDialog.FileName.LastIndexOf('\\')] + "\\";
                        Properties.Settings.Default.Save();

                        IPackFileHeaderVerBase pFileHeader = tag.FileHeader;
                        if (pFileHeader != null)
                        {
                            if (pNode.Data == null)
                            {
                                pNode.Data = DecryptData(pFileHeader, pDataMappedMemFile);
                                File.WriteAllBytes(pDialog.FileName, pNode.Data);

                                // Nullify the data as it was previously.
                                pNode.Data = null;
                                return;
                            }

                            File.WriteAllBytes(pDialog.FileName, pNode.Data);
                        }

                        NotifyMessage($"Successfully exported to {pDialog.FileName}", MessageBoxIcon.Information);

                        break;
                    }
            }
        } // Export

        private void OnSearch(object sender, EventArgs e)
        {
            NotifyMessage("TODO: Search all files for string?");
        }

        private void OnCreateItem(object sender, EventArgs e)
        {
            NotifyMessage("Its recommended to make a backup of your files before continuing!");
            OpenFileDialog folderBrowserDialog = new OpenFileDialog
            {
                Title = "Select the Xml.m2d file",
                Filter = "MapleStory2 Files|*.m2d",
                Multiselect = false
            };

            string xmlFilePath = folderBrowserDialog.ShowDialog() == DialogResult.OK ? folderBrowserDialog.FileName : string.Empty;

            if (xmlFilePath == string.Empty)
            {
                NotifyMessage("Please select the folder before continuing.");
                return;
            }

            if (!xmlFilePath.Contains("Xml.m2d"))
            {
                NotifyMessage("Couldn't find Xml.m2d");
                return;
            }

            string basePath = xmlFilePath.Replace("Xml.m2d", "");
            string baseResourcePath = basePath + "Resource\\";
            string baseModelPath = baseResourcePath + "Model\\";

            string imageFilePath = baseResourcePath + "Image.m2d";
            string itemFilePath = baseModelPath + "Item.m2d";
            string textureFilePath = baseModelPath + "Textures.m2d";

            if (!File.Exists(imageFilePath))
            {
                NotifyMessage("Couldn't find Image.m2d");
                return;
            }

            if (!File.Exists(itemFilePath))
            {
                NotifyMessage("Couldn't find Item.m2d");
                return;
            }

            if (!File.Exists(textureFilePath))
            {
                NotifyMessage("Couldn't find Textures.m2d");
                return;
            }

            CreateItem dialog = new CreateItem(xmlFilePath, imageFilePath, itemFilePath, textureFilePath);

            dialog.ShowDialog();
        }

        #endregion

        #region Helpers

        private void NotifyMessage(string sText, MessageBoxIcon eIcon = MessageBoxIcon.None)
        {
            MessageBox.Show(this, sText, Text, MessageBoxButtons.OK, eIcon);
        }

        private void OnChangeImage(object sender, EventArgs e)
        {
            if (!pChangeImageBtn.Visible) return;

            if (!(pTreeView.SelectedNode is PackNode pNode) || pNode.Data == null) return;

            if (!(pNode.Tag is PackFileEntry pEntry)) return;

            string sExtension = pEntry.TreeName.Split('.')[1];
            OpenFileDialog pDialog = new OpenFileDialog
            {
                Title = "Select the new image",
                Filter = string.Format("{0} Image|*.{0}",
                    sExtension.ToUpper()),
                Multiselect = false
            };
            if (pDialog.ShowDialog() != DialogResult.OK) return;

            byte[] pData = File.ReadAllBytes(pDialog.FileName);
            if (pNode.Data == pData) return;

            pEntry.Data = pData;
            pEntry.Changed = true;
            UpdatePanel(sExtension, pData);
        }

        private void OnChangeWindowSize(object sender, EventArgs e)
        {
            int nHeight = Size.Height - pPrevSize.Height;
            int nWidth = Size.Width - pPrevSize.Width;

            webView.Size = new Size
            {
                Height = webView.Height + nHeight,
                Width = webView.Width + nWidth
            };

            pImagePanel.Size = new Size
            {
                Height = pImagePanel.Height + nHeight,
                Width = pImagePanel.Width + nWidth
            };

            pTreeView.Size = new Size
            {
                Height = pTreeView.Height + nHeight,
                Width = pTreeView.Width
            };

            pEntryValue.Location = new Point
            {
                X = pEntryValue.Location.X + nWidth,
                Y = pEntryValue.Location.Y
            };

            pPrevSize = Size;
            pImageData.Size = pImagePanel.Size;

            RenderImageData(true);
        }

        private void OnDoubleClickNode(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!(pTreeView.SelectedNode is PackNode pNode) || pNode.Nodes.Count != 0) return;

            object pObj = pNode.Tag;

            if (!(pObj is PackNodeList pList)) return;

            // Iterate all further directories within the list
            foreach (KeyValuePair<string, PackNodeList> pChild in pList.Children)
            {
                pNode.Nodes.Add(new PackNode(pChild.Value, pChild.Key));
            }

            // Iterate entries
            foreach (PackFileEntry pEntry in pList.Entries.Values)
            {
                pNode.Nodes.Add(new PackNode(pEntry, pEntry.TreeName));
            }

            pNode.Expand();
            /*else if (pObj is PackFileEntry)
            {
                PackFileEntry pEntry = pObj as PackFileEntry;
                PackFileHeaderVerBase pFileHeader = pEntry.FileHeader;

                if (pFileHeader != null)
                {
                    byte[] pBuffer = CryptoMan.DecryptData(pFileHeader, pDataMappedMemFile);

                    UpdatePanel(pEntry.TreeName.Split('.')[1].ToLower(), pBuffer);
                }
            }*/
        }

        private void OnWindowClosing(object sender, FormClosingEventArgs e)
        {
            // Only ask for confirmation when the user has files open.
            if (pTreeView.Nodes.Count <= 0) return;

            if (MessageBox.Show(this, "Are you sure you want to exit?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No) return;

            e.Cancel = true;
        }

        private static void RemoveDirectory(PackNode pNode, IPackStreamVerBase pStream)
        {
            if (pNode.Nodes.Count == 0)
                if (pNode.Tag is PackNodeList pList)
                {
                    foreach (KeyValuePair<string, PackNodeList> pChild in pList.Children)
                    {
                        pNode.Nodes.Add(new PackNode(pChild.Value, pChild.Key));
                    }

                    foreach (PackFileEntry pEntry in pList.Entries.Values)
                    {
                        pNode.Nodes.Add(new PackNode(pEntry, pEntry.TreeName));
                    }

                    pList.Children.Clear();
                    pList.Entries.Clear();
                }

            foreach (PackNode pChild in pNode.Nodes)
            {
                RemoveDirectory(pChild, pStream);
            }

            if (pNode.Tag is PackFileEntry tag)
            {
                pStream.GetFileList().Remove(tag);
            }
        }

        private void RenderImageData(bool bChange)
        {
            pImageData.Visible = pImagePanel.Visible;

            if (!pImageData.Visible) return;

            // If the size of the bitmap image is bigger than the actual panel,
            // then we adjust the image sizing mode to zoom the image in order
            // to fit the full image within the current size of the panel.
            if (pImageData.Image.Size.Height > pImagePanel.Size.Height || pImageData.Image.Size.Width > pImagePanel.Size.Width)
            {
                // If we went from selecting a small image to selecting a big image,
                // then adjust the panel and data to fit the size of the new bitmap.
                if (!bChange) OnChangeWindowSize(null, null);

                // Since the image is too big, scale it in zoom mode to fit it.
                pImageData.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                // Since the image is less than or equal to the size of the panel,
                // we are able to render the image as-is with no additional scaling.
                pImageData.SizeMode = PictureBoxSizeMode.Normal;
            }

            // Render the new size changes.
            pImageData.Update();
        }

        private void UpdatePanel(string sExtension, byte[] pBuffer)
        {
            if (pBuffer == null)
            {
                pEntryValue.Text = sExtension;
                pEntryName.Visible = false;
                pUpdateDataBtn.Visible = false;
                pImagePanel.Visible = false;
                pChangeImageBtn.Visible = false;
                webView.Visible = false;
            }
            else
            {
                pEntryValue.Text = $"{sExtension.ToUpper()} File";
                pEntryName.Visible = true;

                bool isText = sExtension.Equals("ini") || sExtension.Equals("nt") || sExtension.Equals("lua")
                                                    || sExtension.Equals("xml") || sExtension.Equals("flat") || sExtension.Equals("xblock")
                                                    || sExtension.Equals("diagram") || sExtension.Equals("preset") || sExtension.Equals("emtproj");
                webView.Visible = isText;
                pUpdateDataBtn.Visible = isText;

                pImagePanel.Visible = sExtension.Equals("png") || sExtension.Equals("dds");
                pChangeImageBtn.Visible = pImagePanel.Visible;

                if (isText)
                {
                    Properties.Settings.Default.Reload();
                    string editorTheme = Properties.Settings.Default.EditorTheme;
                    string wordWrap = Properties.Settings.Default.EditorWordWrap ? "on" : "off";
                    string language = "xml";
                    switch (sExtension)
                    {
                        case "ini":
                            language = "ini";
                            break;
                        case "nt":
                            language = "txt";
                            break;
                        case "lua":
                            language = "lua";
                            break;
                        default:
                            break;
                    }
                    JObject json = new()
                    {
                        { "type", "updateSettings" },
                        { "theme", editorTheme },
                        { "wordWrap", wordWrap },
                        { "language", language }
                    };
                    webView.CoreWebView2.PostWebMessageAsJson(json.ToString());

                    string content = Encoding.UTF8.GetString(pBuffer);
                    json = new JObject
                    {
                        { "type", "updateContent" },
                        { "content", content }
                    };

                    if (content.Contains("encoding=\"euc-kr\""))
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        json = new JObject
                        {
                            { "type", "updateContent" },
                            { "content", Encoding.GetEncoding("euc-kr").GetString(pBuffer) }
                        };
                    }

                    webView.CoreWebView2.PostWebMessageAsJson(json.ToString());
                }
                else if (pImagePanel.Visible)
                {
                    Bitmap pImage;
                    if (sExtension.Equals("png"))
                    {
                        using (MemoryStream pStream = new MemoryStream(pBuffer))
                        {
                            pImage = new Bitmap(pStream);
                        }
                    }
                    else //if (sExtension.Equals("dds"))
                    {
                        pImage = DDS.LoadImage(pBuffer);
                    }

                    pImageData.Image = pImage;
                }
            }

            /*
             * TODO:
             * *.nif, *.kf, and *.kfm files
             * Shaders/*.fxo - directx shader files?
             * PrecomputedTerrain/*.tok - mesh3d files? token files?
             * Gfx/*.gfx - graphics gen files?
             * Precompiled/luapack.o - object files?
            */

            RenderImageData(false);
        }

        private static string Dir_BackSlashToSlash(string sDir)
        {
            while (sDir.Contains("\\"))
            {
                sDir = sDir.Replace("\\", "/");
            }

            return sDir;
        }

        #endregion

        #region Helpers - Save

        private void OnSaveBegin(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker)) return;

            IPackStreamVerBase pStream = pProgress.Stream;

            if (pStream == null) return;

            pProgress.Start();
            pStream.GetFileList().Sort();
            SaveData(pProgress.Path, pStream.GetFileList());
            uint dwFileCount = (uint)pStream.GetFileList().Count;
            StringBuilder sFileString = new StringBuilder();
            foreach (PackFileEntry pEntry in pStream.GetFileList()) sFileString.Append(pEntry);
            pSaveWorkerThread.ReportProgress(96);
            byte[] pFileString = Encoding.UTF8.GetBytes(sFileString.ToString().ToCharArray());
            byte[] pHeader = Encrypt(pStream.GetVer(), pFileString, BufferManipulation.AES_ZLIB, out uint uHeaderLen, out uint uCompressedHeaderLen,
                out uint uEncodedHeaderLen);
            pSaveWorkerThread.ReportProgress(97);
            byte[] pFileTable;
            using (MemoryStream pOutStream = new MemoryStream())
            {
                using (BinaryWriter pWriter = new BinaryWriter(pOutStream))
                {
                    foreach (PackFileEntry pEntry in pStream.GetFileList()) pEntry.FileHeader.Encode(pWriter);
                }

                pFileTable = pOutStream.ToArray();
            }

            pSaveWorkerThread.ReportProgress(98);
            pFileTable = Encrypt(pStream.GetVer(), pFileTable, BufferManipulation.AES_ZLIB, out uint uDataLen, out uint uCompressedDataLen,
                out uint uEncodedDataLen);
            pSaveWorkerThread.ReportProgress(99);
            pStream.SetFileListCount(dwFileCount);
            pStream.SetHeaderSize(uHeaderLen);
            pStream.SetCompressedHeaderSize(uCompressedHeaderLen);
            pStream.SetEncodedHeaderSize(uEncodedHeaderLen);
            pStream.SetDataSize(uDataLen);
            pStream.SetCompressedDataSize(uCompressedDataLen);
            pStream.SetEncodedDataSize(uEncodedDataLen);
            using (BinaryWriter pWriter = new BinaryWriter(File.Create(pProgress.Path.Replace(".m2d", ".m2h"))))
            {
                pWriter.Write(pStream.GetVer());
                pStream.Encode(pWriter);
                pWriter.Write(pHeader);
                pWriter.Write(pFileTable);
            }

            pSaveWorkerThread.ReportProgress(100);
        }

        private void OnSaveChanges(object sender, EventArgs e)
        {
            if (!pUpdateDataBtn.Visible) return;

            if (!(pTreeView.SelectedNode is PackNode pNode) || pNode.Data == null) return;

            if (!(pNode.Tag is PackFileEntry)) return;

            JObject json = new JObject
            {
                { "type", "saveFile" }
            };
            webView.CoreWebView2.PostWebMessageAsJson(json.ToString());

        }

        private void OnSaveProgress(object sender, ProgressChangedEventArgs e)
        {
            pProgress.UpdateProgressBar(e.ProgressPercentage);
        }

        private void OnSaveComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(pProgress, e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

            pProgress.Finish();
            pProgress.Close();

            TimeSpan pInterval = TimeSpan.FromMilliseconds(pProgress.ElapsedTime);
            NotifyMessage($"Successfully saved in {pInterval.Minutes} minutes and {pInterval.Seconds} seconds!",
                MessageBoxIcon.Information);

            // Perform heavy cleanup
            GC.Collect();
        }

        private void OnExtractComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(extractWindow, e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

            extractWindow.Finish();
            extractWindow.Close();

            TimeSpan pInterval = TimeSpan.FromMilliseconds(extractWindow.ElapsedTime);
            NotifyMessage($"Successfully exported in {pInterval.Minutes} minutes and {pInterval.Seconds} seconds!",
                MessageBoxIcon.Information);

            // Perform heavy cleanup
            GC.Collect();
        }

        private void OnExtractProgress(object sender, ProgressChangedEventArgs e)
        {
            extractWindow.UpdateProgressBar(e.ProgressPercentage);
        }

        private void SaveData(string sDataPath, List<PackFileEntry> aEntry)
        {
            // Declare MS2F as the initial version until specified.
            uint uVer = PackVer.MS2F;
            // Re-calculate all file offsets from start to finish
            ulong uOffset = 0;
            // Re-calculate all file indexes from start to finish
            int nCurIndex = 1;

            using (BinaryWriter pWriter = new BinaryWriter(File.Create(sDataPath)))
            {
                // Iterate all file entries that exist
                foreach (PackFileEntry pEntry in aEntry)
                {
                    IPackFileHeaderVerBase pHeader = pEntry.FileHeader;

                    // If the entry was modified, or is new, write the modified data block
                    if (pEntry.Changed)
                    {
                        // If the header is null (new entry), then create one
                        if (pHeader == null)
                        {
                            // Hacky way of doing this, but this follows Nexon's current conventions.
                            uint dwBufferFlag;
                            if (pEntry.Name.EndsWith(".usm"))
                                dwBufferFlag = BufferManipulation.XOR;
                            else if (pEntry.Name.EndsWith(".png"))
                                dwBufferFlag = BufferManipulation.AES;
                            else
                                dwBufferFlag = BufferManipulation.AES_ZLIB;

                            switch (uVer)
                            {
                                case PackVer.MS2F:
                                    pHeader = PackFileHeaderVer1.CreateHeader(nCurIndex, dwBufferFlag, uOffset, pEntry.Data);
                                    break;
                                case PackVer.NS2F:
                                    pHeader = PackFileHeaderVer2.CreateHeader(nCurIndex, dwBufferFlag, uOffset, pEntry.Data);
                                    break;
                                case PackVer.OS2F:
                                case PackVer.PS2F:
                                    pHeader = PackFileHeaderVer3.CreateHeader(uVer, nCurIndex, dwBufferFlag, uOffset, pEntry.Data);
                                    break;
                            }

                            // Update the entry's file header to the newly created one
                            pEntry.FileHeader = pHeader;
                        }
                        else
                        {
                            // If the header existed already, re-calculate the file index and offset.
                            pHeader.SetFileIndex(nCurIndex);
                            pHeader.SetOffset(uOffset);
                        }

                        // Encrypt the new data block and output the header size data
                        pWriter.Write(Encrypt(uVer, pEntry.Data, pEntry.FileHeader.GetBufferFlag(), out uint uLen, out uint uCompressed, out uint uEncoded));

                        // Apply the file size changes from the new buffer
                        pHeader.SetFileSize(uLen);
                        pHeader.SetCompressedFileSize(uCompressed);
                        pHeader.SetEncodedFileSize(uEncoded);

                        // Update the Entry's index to the new current index
                        pEntry.Index = nCurIndex;

                        nCurIndex++;
                        uOffset += pHeader.GetEncodedFileSize();

                        // Allow the remaining 5% for header file write progression
                        pSaveWorkerThread.ReportProgress((int)((nCurIndex - 1) / (double)aEntry.Count * 95.0d));
                        continue;
                    }
                    // If the entry is unchanged, parse the block from the original offsets

                    // Make sure the entry has a parsed file header from load
                    if (pHeader == null) continue;

                    // Update the initial versioning before any future crypto calls
                    if (pHeader.GetVer() != uVer) uVer = pHeader.GetVer();

                    // Access the current encrypted block data from the memory map initially loaded
                    using (MemoryMappedViewStream pBuffer = pDataMappedMemFile.CreateViewStream((long)pHeader.GetOffset(), pHeader.GetEncodedFileSize()))
                    {
                        byte[] pSrc = new byte[pHeader.GetEncodedFileSize()];

                        if (pBuffer.Read(pSrc, 0, (int)pHeader.GetEncodedFileSize()) != pHeader.GetEncodedFileSize()) continue;
                        // Modify the header's file index to the updated offset after entry changes
                        pHeader.SetFileIndex(nCurIndex);
                        // Modify the header's offset to the updated offset after entry changes
                        pHeader.SetOffset(uOffset);
                        // Write the original (completely encrypted) block of data to file
                        pWriter.Write(pSrc);

                        // Update the Entry's index to the new current index
                        pEntry.Index = nCurIndex;

                        nCurIndex++;
                        uOffset += pHeader.GetEncodedFileSize();
                    }

                    // Allow the remaining 5% for header file write progression
                    pSaveWorkerThread.ReportProgress((int)((nCurIndex - 1) / (double)aEntry.Count * 95.0d));
                }
            }
        }

        #endregion

        #region Helpers - Export

        private void OnExportNodeList(string extractPath, PackNodeList nodeList)
        {
            Directory.CreateDirectory(extractPath);

            foreach (KeyValuePair<string, PackNodeList> pChild in nodeList.Children)
            {
                //PackNode pGrandChild = new PackNode(pChild.Value, pChild.Key);

                Directory.CreateDirectory(extractPath + pChild.Key);
                OnExportNodeList(extractPath + pChild.Key, pChild.Value);
            }

            foreach (PackFileEntry pEntry in nodeList.Entries.Values)
            {
                IPackFileHeaderVerBase pFileHeader = pEntry.FileHeader;
                if (pFileHeader == null) continue;

                PackNode pChild = new PackNode(pEntry, pEntry.TreeName);
                if (pChild.Data == null)
                {
                    pChild.Data = DecryptData(pFileHeader, pDataMappedMemFile);
                    File.WriteAllBytes(extractPath + pChild.Name, pChild.Data);

                    // Nullify the data as it was previously.
                    pChild.Data = null;
                    continue;
                }

                File.WriteAllBytes(extractPath + pChild.Name, pChild.Data);
            }
        }

        private void extractWorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker)) return;

            string sPath = extractWindow.Path;
            PackNode pNode = extractWindow.PackNode;
            if (pNode is null) return;

            extractWindow.Start();
            int i = 0;
            foreach (PackNode pRootChild in pNode.Nodes)
            {
                switch (pRootChild.Tag)
                {
                    case PackNodeList nodeList:
                        OnExportNodeList(sPath + pRootChild.Name, nodeList);
                        break;
                    case PackFileEntry fileEntry:
                        IPackFileHeaderVerBase pFileHeader = fileEntry.FileHeader;
                        if (pFileHeader == null) continue;

                        PackNode pChild = new PackNode(fileEntry, fileEntry.TreeName);
                        if (pChild.Data is null)
                        {
                            pChild.Data = DecryptData(pFileHeader, pDataMappedMemFile);
                            File.WriteAllBytes(sPath + pChild.Name, pChild.Data);

                            // Nullify the data as it was previously.
                            pChild.Data = null;
                            continue;
                        }

                        File.WriteAllBytes(sPath + pChild.Name, pChild.Data);
                        break;
                }

                extractWorkerThread.ReportProgress(i++);
            }
        }

        #endregion

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wordWrapToolStripMenuItem.Checked = !wordWrapToolStripMenuItem.Checked;

            string wordWrapValue = wordWrapToolStripMenuItem.Checked ? "on" : "off";

            JObject json = new JObject
            {
                { "type", "wordWrap" },
                { "wordWrap", wordWrapValue }
            };
            webView.CoreWebView2.PostWebMessageAsJson(json.ToString());

            Properties.Settings.Default.EditorWordWrap = wordWrapToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            darkToolStripMenuItem.Checked = true;
            lightToolStripMenuItem.Checked = false;
            JObject json = new JObject
            {
                { "type", "theme" },
                { "theme", "vs-dark" }
            };
            webView.CoreWebView2.PostWebMessageAsJson(json.ToString());
            Properties.Settings.Default.EditorTheme = "vs-dark";
            Properties.Settings.Default.Save();
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lightToolStripMenuItem.Checked = true;
            darkToolStripMenuItem.Checked = false;
            JObject json = new JObject
            {
                { "type", "theme" },
                { "theme", "vs" }
            };
            webView.CoreWebView2.PostWebMessageAsJson(json.ToString());
            Properties.Settings.Default.EditorTheme = "vs";
            Properties.Settings.Default.Save();
        }
    }
}