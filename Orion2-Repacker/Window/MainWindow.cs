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

using Orion.Crypto;
using Orion.Crypto.Common;
using Orion.Crypto.Stream;
using Orion.Crypto.Stream.DDS;
using Orion.Window.Common;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;
using static Orion.Crypto.CryptoMan;

namespace Orion.Window
{
    public partial class MainWindow : Form
    {
        private string sHeaderUOL;
        private PackNodeList pNodeList;
        private MemoryMappedFile pDataMappedMemFile;
        private ProgressWindow pProgress;

        public MainWindow()
        {
            InitializeComponent();

            pImagePanel.AutoScroll = true;

            pImageData.BorderStyle = BorderStyle.None;
            pImageData.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);

            pMenuStrip.Renderer = new MenuRenderer();

            pPrevSize = Size;

            sHeaderUOL = "";

            pNodeList = null;
            pDataMappedMemFile = null;
            pProgress = null;

            UpdatePanel("Empty", null);
        }

        private void InitializeTree(IPackStreamVerBase pStream)
        {
            // Insert the root node (file)
            string[] aPath = sHeaderUOL.Replace(".m2h", "").Split('/');
            pTreeView.Nodes.Add(new PackNode(pStream, aPath[aPath.Length - 1]));

            if (pNodeList != null)
            {
                pNodeList.InternalRelease();
            }
            pNodeList = new PackNodeList("/");

            foreach (PackFileEntry pEntry in pStream.GetFileList())
            {
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
                            if (pCurList == pNodeList)
                            {
                                pTreeView.Nodes[0].Nodes.Add(new PackNode(pCurList.Children[sDir], sDir));
                            }
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
            }

            // Sort all nodes
            pTreeView.Sort();
        }

        #region File
        private void OnLoadFile(object sender, EventArgs e)
        {
            if (pNodeList != null)
            {
                NotifyMessage("Please unload the current file first.", MessageBoxIcon.Information);
                return;
            }

            OpenFileDialog pDialog = new OpenFileDialog()
            {
                Title = "Select the MS2 file to load",
                Filter = "MapleStory2 Files|*.m2d",
                Multiselect = false
            };

            if (pDialog.ShowDialog() == DialogResult.OK)
            {
                string sDataUOL = Dir_BackSlashToSlash(pDialog.FileName);
                sHeaderUOL = sDataUOL.Replace(".m2d", ".m2h");

                if (!File.Exists(sHeaderUOL))
                {
                    string sHeaderName = sHeaderUOL.Substring(sHeaderUOL.LastIndexOf('/') + 1);
                    NotifyMessage(string.Format("Unable to load the {0} file.\r\nPlease make sure it exists and is not being used.", sHeaderName), MessageBoxIcon.Error);
                    return;
                }

                IPackStreamVerBase pStream;
                using (BinaryReader pHeader = new BinaryReader(File.OpenRead(sHeaderUOL)))
                {
                    // Construct a new packed stream from the header data
                    pStream = PackVer.CreatePackVer(pHeader);

                    // Insert a collection containing the file list information [index,hash,name]
                    pStream.GetFileList().Clear();
                    pStream.GetFileList().AddRange(PackFileEntry.CreateFileList(Encoding.UTF8.GetString(CryptoMan.DecryptFileString(pStream, pHeader.BaseStream))));
                    // Make the collection of files sorted by their FileIndex for easy fetching
                    pStream.GetFileList().Sort();

                    // Load the file allocation table and assign each file header to the entry within the list
                    byte[] pFileTable = CryptoMan.DecryptFileTable(pStream, pHeader.BaseStream);
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
            }
        } // Open

        private void OnSaveFile(object sender, EventArgs e)
        {
            if (pTreeView.SelectedNode is PackNode pNode && pNode.Tag is IPackStreamVerBase)
            {
                SaveFileDialog pDialog = new SaveFileDialog
                {
                    Title = "Select the destination to save the file",
                    Filter = "MapleStory2 Files|*.m2d"
                };

                if (pDialog.ShowDialog() == DialogResult.OK)
                {
                    string sPath = Dir_BackSlashToSlash(pDialog.FileName);

                    if (!pSaveWorkerThread.IsBusy)
                    {
                        pProgress = new ProgressWindow
                        {
                            Path = sPath,
                            Stream = (pNode.Tag as IPackStreamVerBase)
                        };
                        pProgress.Show(this);
                        // Why do you make this so complicated C#? 
                        int x = DesktopBounds.Left + (Width - pProgress.Width) / 2;
                        int y = DesktopBounds.Top + (Height - pProgress.Height) / 2;
                        pProgress.SetDesktopLocation(x, y);

                        pSaveWorkerThread.RunWorkerAsync();
                    }
                }
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
                IPackStreamVerBase pStream;

                if (pTreeView.Nodes.Count > 0)
                {
                    pStream = pTreeView.Nodes[0].Tag as IPackStreamVerBase;
                    if (pStream == null)
                    {
                        return;
                    }

                    pTreeView.Nodes.Clear();
                    pTreeView.Refresh();

                    InitializeTree(pStream);
                    UpdatePanel("Empty", null);
                }
            }
            else
            {
                NotifyMessage("There is no package to be reloaded.", MessageBoxIcon.Warning);
            }
        } // Reload

        private void OnUnloadFile(object sender, EventArgs e)
        {
            if (pNodeList != null)
            {
                pTreeView.Nodes.Clear();

                pNodeList.InternalRelease();
                pNodeList = null;

                sHeaderUOL = "";

                if (pDataMappedMemFile != null)
                {
                    pDataMappedMemFile.Dispose();
                    pDataMappedMemFile = null;
                }

                UpdatePanel("Empty", null);

                System.GC.Collect();
            }
            else
            {
                NotifyMessage("There is no package to be unloaded.", MessageBoxIcon.Warning);
            }
        } // Unload

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        } // Exit
        #endregion

        #region Edit
        private void OnAddFile(object sender, EventArgs e)
        {
            if (pNodeList == null)
            {
                NotifyMessage("Please load an file first.", MessageBoxIcon.Information);
                return;
            }

            PackNode pNode = pTreeView.SelectedNode as PackNode;
            if (pNode != null)
            {
                if (pNode.Tag is PackFileEntry) //wtf are they thinking?
                {
                    NotifyMessage("Please select a directory to add into!", MessageBoxIcon.Exclamation);
                    return;
                }
            }

            OpenFileDialog pDialog = new OpenFileDialog()
            {
                Title = "Select the file to add",
                Filter = "MapleStory2 Files|*",
                Multiselect = false
            };

            if (pDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            sHeaderUOL = Dir_BackSlashToSlash(pDialog.FileName);

            string sHeaderName = sHeaderUOL.Substring(sHeaderUOL.LastIndexOf('/') + 1);

            if (!File.Exists(sHeaderUOL))
            {
                NotifyMessage(string.Format("Unable to load the {0} file.\r\nPlease make sure it exists and is not being used.", sHeaderName), MessageBoxIcon.Error);
                return;
            }

            PackNodeList pList;
            if (pNode.Level == 0)
            {
                // If they're trying to add to the root of the file,
                // then just use the root node list of this tree.
                pList = pNodeList;
            }
            else
            {
                pList = pNode.Tag as PackNodeList;
            }

            byte[] pData = File.ReadAllBytes(pDialog.FileName);

            PackFileEntry pEntry = new PackFileEntry()
            {
                Name = sHeaderName,
                Hash = CreateHash(sHeaderUOL),
                Index = 1,
                Changed = true,
                TreeName = sHeaderName,
                Data = pData
            };

            if (pList.Entries.ContainsKey(pEntry.TreeName))
            {
                NotifyMessage("File name already exists in directory.", MessageBoxIcon.Exclamation);
                return;
            }
            AddFileEntry(pEntry);
            pList.Entries.Add(pEntry.TreeName, pEntry);

            PackNode pChild = new PackNode(pEntry, pEntry.TreeName);
            pNode.Nodes.Add(pChild);

            pEntry.Name = pChild.Path;
        } // Add

        private void OnAddFolder(object sender, EventArgs e)
        {
            if (pNodeList == null)
            {
                NotifyMessage("Please load an file first.", MessageBoxIcon.Information);
                return;
            }

            PackNode pNode = pTreeView.SelectedNode as PackNode;
            if (pNode == null)
            {
                return;
            }

            if (pNode.Tag is PackFileEntry)
            {
                NotifyMessage("Please select a directory to add into!", MessageBoxIcon.Exclamation);
                return;
            }

            PackNodeList pList = pNode.Tag as PackNodeList;

            string nodeName = ShowDialog("Type the folder name");
            if (string.IsNullOrEmpty(nodeName))
            {
                return;
            }
            nodeName += "/";
            PackNodeList newNodeList = new PackNodeList(nodeName);
            PackNode pChild = new PackNode(newNodeList, nodeName);
            pList.Children.Add(nodeName, newNodeList);
            pNode.Nodes.Add(pChild);
        }

        private void OnRemoveFile(object sender, EventArgs e)
        {
            if (pTreeView.SelectedNode is PackNode pNode)
            {
                if (pTreeView.Nodes[0] is PackNode pRoot && pNode != pRoot)
                {
                    if (pRoot.Tag is IPackStreamVerBase pStream)
                    {
                        if (pNode.Tag is PackFileEntry pEntry)
                        {
                            pStream.GetFileList().Remove(pEntry);
                            if (pNode.Parent == pRoot as TreeNode)
                            {
                                pNodeList.Entries.Remove(pEntry.TreeName);
                            }
                            else
                            {
                                (pNode.Parent.Tag as PackNodeList).Entries.Remove(pEntry.TreeName);
                            }
                            pNode.Parent.Nodes.Remove(pNode);
                        }
                        else if (pNode.Tag is PackNodeList)
                        {
                            string sWarning = "WARNING: You are about to delete an entire directory!" + "\r\nBy deleting this directory, all inner directories and entries will also be removed." + "\r\n\r\nAre you sure you want to continue?";
                            if (MessageBox.Show(this, sWarning, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                RemoveDirectory(pNode, pStream);
                                if (pNode.Parent == pRoot as TreeNode)
                                {
                                    pNodeList.Children.Remove(pNode.Name);
                                }
                                else
                                {
                                    (pNode.Parent.Tag as PackNodeList).Children.Remove(pNode.Name);
                                }
                                pNode.Remove();
                            }
                        }
                    }
                }
            }
            else
            {
                NotifyMessage("Please select a file or directory to remove.", MessageBoxIcon.Exclamation);
            }
        } // Remove

        private void OnCopyNode(object sender, EventArgs e)
        {
            if (pTreeView.SelectedNode is PackNode pNode)
            {
                if (pNode.Tag is PackFileEntry pEntry)
                {
                    // Clear any current data from clipboard.
                    Clipboard.Clear();
                    // Copy the new copied entry object to clipboard.
                    Clipboard.SetData(PackFileEntry.DATA_FORMAT, pEntry.CreateCopy());
                }
                else
                {
                    if (pNode.Tag is PackNodeList pList)
                    {
                        PackNodeList pListCopy = new PackNodeList(pList.Directory);
                        foreach (PackFileEntry pChild in pList.Entries.Values)
                        {
                            byte[] pBlock = CryptoMan.DecryptData(pChild.FileHeader, pDataMappedMemFile);
                            pListCopy.Entries.Add(pChild.TreeName, pChild.CreateCopy(pBlock));
                        }
                        Clipboard.Clear();
                        Clipboard.SetData(PackNodeList.DATA_FORMAT, pListCopy);
                    }
                }
            }
            else
            {
                NotifyMessage("Please select the node you wish to copy.", MessageBoxIcon.Exclamation);
            }
        } // Copy

        private void OnPasteNode(object sender, EventArgs e)
        {
            IDataObject pData = Clipboard.GetDataObject();
            if (pData == null)
            {
                return;
            }

            if (pTreeView.SelectedNode is PackNode pNode)
            {
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
                {
                    // If they're trying to add to the root of the file,
                    // then just use the root node list of this tree.
                    pList = pNodeList;
                }
                else
                {
                    pList = pNode.Tag as PackNodeList;
                }

                if (pList != null && pObj != null)
                {
                    if (pObj is PackFileEntry)
                    {
                        PackFileEntry pEntry = pObj as PackFileEntry;

                        if (pList.Entries.ContainsKey(pEntry.TreeName))
                        {
                            NotifyMessage("File name already exists in directory.", MessageBoxIcon.Exclamation);
                            return;
                        }
                        AddFileEntry(pEntry);
                        pList.Entries.Add(pEntry.TreeName, pEntry);

                        PackNode pChild = new PackNode(pEntry, pEntry.TreeName);
                        pNode.Nodes.Add(pChild);

                        pEntry.Name = pChild.Path;
                    }
                    else if (pObj is PackNodeList)
                    {
                        PackNodeList pChildList = pObj as PackNodeList;

                        PackNode pChild = new PackNode(pChildList, pChildList.Directory);
                        pList.Children.Add(pChildList.Directory, pChildList);
                        pNode.Nodes.Add(pChild);

                        foreach (PackFileEntry pEntry in pChildList.Entries.Values)
                        {
                            AddFileEntry(pEntry);
                            PackNode pListNode = new PackNode(pEntry, pEntry.TreeName);
                            pChild.Nodes.Add(pListNode);

                            pEntry.Name = pListNode.Path;
                        }
                    }
                }
            }
        } // Paste

        private void OnExpandNodes(object sender, EventArgs e)
        {
            pTreeView.ExpandAll();
        } // Edit / Expand

        private void OnCollapseNodes(object sender, EventArgs e)
        {
            pTreeView.CollapseAll();
        } // Edit / Collapse
        #endregion

        #region Tools
        private void OnExport(object sender, EventArgs e)
        {
            if (pTreeView.SelectedNode is PackNode pNode)
            {
                if (pNode.Tag is PackNodeList)
                {
                    FolderBrowserDialog pDialog = new FolderBrowserDialog
                    {
                        Description = "Select the destination folder to export to"
                    };

                    if (pDialog.ShowDialog() == DialogResult.OK)
                    {
                        StringBuilder sPath = new StringBuilder(Dir_BackSlashToSlash(pDialog.SelectedPath)).Append("/");
                        PackNode pParent = pNode.Parent as PackNode;
                        while (pParent != null && pParent.Tag is PackNodeList)
                        {
                            sPath.Append(pParent.Name);

                            pParent = pParent.Parent as PackNode;
                        }
                        sPath.Append(pNode.Name);

                        OnExportNodeList(sPath.ToString(), pNode.Tag as PackNodeList);

                        NotifyMessage(string.Format("Successfully exported to {0}", sPath), MessageBoxIcon.Information);
                    }
                }
                else if (pNode.Tag is IPackStreamVerBase)
                {
                    FolderBrowserDialog pDialog = new FolderBrowserDialog
                    {
                        Description = "Select the destination folder to export to"
                    };

                    if (pDialog.ShowDialog() == DialogResult.OK)
                    {
                        StringBuilder sPath = new StringBuilder(Dir_BackSlashToSlash(pDialog.SelectedPath));
                        sPath.Append("/");
                        sPath.Append(pNode.Name);
                        sPath.Append("/");
                        // Create root directory
                        if (!Directory.Exists(sPath.ToString()))
                        {
                            Directory.CreateDirectory(sPath.ToString());
                        }

                        foreach (PackNode pRootChild in pNode.Nodes)
                        {
                            if (pRootChild.Tag != null && pRootChild.Tag is PackNodeList)
                            {
                                OnExportNodeList(sPath.ToString() + pRootChild.Name, pRootChild.Tag as PackNodeList);
                            }
                            else if (pRootChild.Tag != null && pRootChild.Tag is PackFileEntry)
                            {
                                PackFileEntry pEntry = pRootChild.Tag as PackFileEntry;
                                IPackFileHeaderVerBase pFileHeader = pEntry.FileHeader;
                                if (pFileHeader != null)
                                {
                                    PackNode pChild = new PackNode(pEntry, pEntry.TreeName);
                                    if (pChild.Data == null)
                                    {
                                        pChild.Data = CryptoMan.DecryptData(pFileHeader, pDataMappedMemFile);
                                        File.WriteAllBytes(sPath + pChild.Name, pChild.Data);

                                        // Nullify the data as it was previously.
                                        pChild.Data = null;
                                    }
                                    else
                                    {
                                        File.WriteAllBytes(sPath + pChild.Name, pChild.Data);
                                    }
                                }
                            }
                        }

                        NotifyMessage(string.Format("Successfully exported to {0}", sPath), MessageBoxIcon.Information);
                    }
                }
                else if (pNode.Tag is PackFileEntry)
                {
                    PackFileEntry pEntry = pNode.Tag as PackFileEntry;
                    string sName = pEntry.TreeName.Split('.')[0];
                    string sExtension = pEntry.TreeName.Split('.')[1];

                    SaveFileDialog pDialog = new SaveFileDialog
                    {
                        Title = "Select the destination to export the file",
                        FileName = sName,
                        Filter = string.Format("{0} File|*.{1}", sExtension.ToUpper(), sExtension)
                    };

                    if (pDialog.ShowDialog() == DialogResult.OK)
                    {
                        IPackFileHeaderVerBase pFileHeader = pEntry.FileHeader;
                        if (pFileHeader != null)
                        {
                            if (pNode.Data == null)
                            {
                                pNode.Data = CryptoMan.DecryptData(pFileHeader, pDataMappedMemFile);
                                File.WriteAllBytes(pDialog.FileName, pNode.Data);

                                // Nullify the data as it was previously.
                                pNode.Data = null;
                            }
                            else
                            {
                                File.WriteAllBytes(pDialog.FileName, pNode.Data);
                            }
                        }

                        NotifyMessage(string.Format("Successfully exported to {0}", pDialog.FileName), MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                NotifyMessage("Please select a file to export.", MessageBoxIcon.Asterisk);
            }
        } // Export

        private void OnSearch(object sender, EventArgs e)
        {
            LoadSearchForm(sender, e);
        } // Search
        #endregion

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

        #region Helpers
        private void NotifyMessage(string sText, MessageBoxIcon eIcon = MessageBoxIcon.None)
        {
            MessageBox.Show(this, sText, Text, MessageBoxButtons.OK, eIcon);
        }

        private static string CreateHash(string sHeaderUOL)
        {
            if (!File.Exists(sHeaderUOL))
            {
                return "";
            }

            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(sHeaderUOL))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void OnChangeImage(object sender, EventArgs e)
        {
            if (!pChangeImageBtn.Visible)
            {
                return;
            }

            if (pTreeView.SelectedNode is PackNode pNode && pNode.Data != null)
            {
                if (pNode.Tag is PackFileEntry pEntry)
                {
                    string sExtension = pEntry.TreeName.Split('.')[1];
                    OpenFileDialog pDialog = new OpenFileDialog
                    {
                        Title = "Select the new image",
                        Filter = string.Format("{0} Image|*.{0}",
                        sExtension.ToUpper()),
                        Multiselect = false
                    };
                    if (pDialog.ShowDialog() == DialogResult.OK)
                    {
                        byte[] pData = File.ReadAllBytes(pDialog.FileName);
                        if (pNode.Data != pData)
                        {
                            pEntry.Data = pData;
                            pEntry.Changed = true;
                            UpdatePanel(sExtension, pData);
                        }
                    }
                }
            }
        }

        private void OnChangeWindowSize(object sender, EventArgs e)
        {
            int nHeight = (Size.Height - pPrevSize.Height);
            int nWidth = (Size.Width - pPrevSize.Width);

            pTextData.Size = new Size
            {
                Height = pTextData.Height + nHeight,
                Width = pTextData.Width + nWidth
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
            if (!(pTreeView.SelectedNode is PackNode pNode) || pNode.Nodes.Count != 0)
            {
                return;
            }

            object pObj = pNode.Tag;

            if (pObj is PackNodeList)
            {
                PackNodeList pList = pObj as PackNodeList;

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
            }
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
            if (pTreeView.Nodes.Count > 0)
            {
                if (MessageBox.Show(this, "Are you sure you want to exit?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void RemoveDirectory(PackNode pNode, IPackStreamVerBase pStream)
        {
            if (pNode.Nodes.Count == 0)
            {
                if (pNode.Tag is PackNodeList)
                {
                    PackNodeList pList = pNode.Tag as PackNodeList;

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
            }

            foreach (PackNode pChild in pNode.Nodes)
            {
                RemoveDirectory(pChild, pStream);
            }

            if (pNode.Tag is PackFileEntry)
            {
                pStream.GetFileList().Remove(pNode.Tag as PackFileEntry);
            }
        }

        private void RenderImageData(bool bChange)
        {
            pImageData.Visible = pImagePanel.Visible;

            if (pImageData.Visible)
            {
                // If the size of the bitmap image is bigger than the actual panel,
                // then we adjust the image sizing mode to zoom the image in order
                // to fit the full image within the current size of the panel.
                if (pImageData.Image.Size.Height > pImagePanel.Size.Height || pImageData.Image.Size.Width > pImagePanel.Size.Width)
                {
                    // If we went from selecting a small image to selecting a big image,
                    // then adjust the panel and data to fit the size of the new bitmap.
                    if (!bChange)
                    {
                        OnChangeWindowSize(null, null);
                    }

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
        }

        private void UpdatePanel(string sExtension, byte[] pBuffer)
        {
            if (pBuffer == null)
            {
                pEntryValue.Text = sExtension;
                pEntryName.Visible = false;
                pTextData.Visible = false;
                pUpdateDataBtn.Visible = false;
                pImagePanel.Visible = false;
                pChangeImageBtn.Visible = false;
            }
            else
            {
                pEntryValue.Text = string.Format("{0} File", sExtension.ToUpper());
                pEntryName.Visible = true;

                pTextData.Visible = (sExtension.Equals("ini") || sExtension.Equals("nt") || sExtension.Equals("lua")
                    || sExtension.Equals("xml") || sExtension.Equals("flat") || sExtension.Equals("xblock")
                    || sExtension.Equals("diagram") || sExtension.Equals("preset") || sExtension.Equals("emtproj"));
                pUpdateDataBtn.Visible = pTextData.Visible;

                pImagePanel.Visible = (sExtension.Equals("png") || sExtension.Equals("dds"));
                pChangeImageBtn.Visible = pImagePanel.Visible;

                if (pTextData.Visible)
                {
                    pTextData.Text = Encoding.UTF8.GetString(pBuffer);
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

            UpdateStyle(sExtension);
            RenderImageData(false);
        }

        private void UpdateStyle(string sExtension)
        {
            if (sExtension.Equals("ini") || sExtension.Equals("nt"))
            {
                // Set the Styles to replicate Sublime
                pTextData.StyleResetDefault();
                pTextData.Styles[Style.Default].Font = "Consolas";
                pTextData.Styles[Style.Default].Size = 10;
                pTextData.Styles[Style.Default].BackColor = Color.FromArgb(0x282923);
                pTextData.Styles[Style.Default].ForeColor = Color.White;
                pTextData.StyleClearAll();
            }
            else if (sExtension.Equals("lua"))
            {
                // Extracted from the Lua Scintilla lexer and SciTE .properties file

                var sAlpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var sNumeric = "0123456789";
                var sAccented = "ŠšŒœŸÿÀàÁáÂâÃãÄäÅåÆæÇçÈèÉéÊêËëÌìÍíÎîÏïÐðÑñÒòÓóÔôÕõÖØøÙùÚúÛûÜüÝýÞþßö";

                // Configuring the default style with properties
                // we have common to every lexer style saves time.
                pTextData.StyleResetDefault();
                pTextData.Styles[Style.Default].Font = "Consolas";
                pTextData.Styles[Style.Default].Size = 10;
                pTextData.Styles[Style.Default].BackColor = Color.FromArgb(0x282923);
                pTextData.Styles[Style.Default].ForeColor = Color.White;
                pTextData.StyleClearAll();

                // Configure the Lua lexer styles
                pTextData.Styles[Style.Lua.Default].ForeColor = Color.Silver;
                pTextData.Styles[Style.Lua.Comment].ForeColor = Color.FromArgb(0xD8D8D8);
                pTextData.Styles[Style.Lua.CommentLine].ForeColor = Color.FromArgb(0xD8D8D8);
                pTextData.Styles[Style.Lua.Number].ForeColor = Color.FromArgb(0xC48CFF);
                pTextData.Styles[Style.Lua.Word].ForeColor = Color.FromArgb(0xFF007F);
                pTextData.Styles[Style.Lua.Word2].ForeColor = Color.BlueViolet;
                pTextData.Styles[Style.Lua.Word3].ForeColor = Color.FromArgb(0x52E3F6);
                pTextData.Styles[Style.Lua.Word4].ForeColor = Color.FromArgb(0x52E3F6);
                pTextData.Styles[Style.Lua.String].ForeColor = Color.FromArgb(0xECE47E);
                pTextData.Styles[Style.Lua.Character].ForeColor = Color.FromArgb(0xECE47E);
                pTextData.Styles[Style.Lua.LiteralString].ForeColor = Color.FromArgb(0xECE47E);
                pTextData.Styles[Style.Lua.StringEol].BackColor = Color.Pink;
                pTextData.Styles[Style.Lua.Operator].ForeColor = Color.White;
                pTextData.Styles[Style.Lua.Preprocessor].ForeColor = Color.Maroon;
                pTextData.Lexer = Lexer.Lua;
                pTextData.WordChars = sAlpha + sNumeric + sAccented;

                // Keywords
                pTextData.SetKeywords(0, "and break do else elseif end for function if in local nil not or repeat return then until while" + " false true" + " goto");
                // Basic Functions
                pTextData.SetKeywords(1, "assert collectgarbage dofile error _G getmetatable ipairs loadfile next pairs pcall print rawequal rawget rawset setmetatable tonumber tostring type _VERSION xpcall string table math coroutine io os debug" + " getfenv gcinfo load loadlib loadstring require select setfenv unpack _LOADED LUA_PATH _REQUIREDNAME package rawlen package bit32 utf8 _ENV");
                // String Manipulation & Mathematical
                pTextData.SetKeywords(2, "string.byte string.char string.dump string.find string.format string.gsub string.len string.lower string.rep string.sub string.upper table.concat table.insert table.remove table.sort math.abs math.acos math.asin math.atan math.atan2 math.ceil math.cos math.deg math.exp math.floor math.frexp math.ldexp math.log math.max math.min math.pi math.pow math.rad math.random math.randomseed math.sin math.sqrt math.tan" + " string.gfind string.gmatch string.match string.reverse string.pack string.packsize string.unpack table.foreach table.foreachi table.getn table.setn table.maxn table.pack table.unpack table.move math.cosh math.fmod math.huge math.log10 math.modf math.mod math.sinh math.tanh math.maxinteger math.mininteger math.tointeger math.type math.ult" + " bit32.arshift bit32.band bit32.bnot bit32.bor bit32.btest bit32.bxor bit32.extract bit32.replace bit32.lrotate bit32.lshift bit32.rrotate bit32.rshift" + " utf8.char utf8.charpattern utf8.codes utf8.codepoint utf8.len utf8.offset");
                // Input and Output Facilities and System Facilities
                pTextData.SetKeywords(3, "coroutine.create coroutine.resume coroutine.status coroutine.wrap coroutine.yield io.close io.flush io.input io.lines io.open io.output io.read io.tmpfile io.type io.write io.stdin io.stdout io.stderr os.clock os.date os.difftime os.execute os.exit os.getenv os.remove os.rename os.setlocale os.time os.tmpname" + " coroutine.isyieldable coroutine.running io.popen module package.loaders package.seeall package.config package.searchers package.searchpath" + " require package.cpath package.loaded package.loadlib package.path package.preload");

                // Instruct the lexer to calculate folding
                pTextData.SetProperty("fold", "1");
                pTextData.SetProperty("fold.compact", "1");

                // Configure a margin to display folding symbols
                pTextData.Margins[2].Type = MarginType.Symbol;
                pTextData.Margins[2].Mask = Marker.MaskFolders;
                pTextData.Margins[2].Sensitive = true;
                pTextData.Margins[2].Width = 20;

                // Set colors for all folding markers
                for (int i = 25; i <= 31; i++)
                {
                    pTextData.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    pTextData.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Configure folding markers with respective symbols
                pTextData.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                pTextData.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                pTextData.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                pTextData.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                pTextData.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                pTextData.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                pTextData.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                pTextData.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
            }
            else
            {
                // Reset the styles
                pTextData.StyleResetDefault();
                pTextData.Styles[Style.Default].Font = "Consolas";
                pTextData.Styles[Style.Default].Size = 10;
                pTextData.Styles[Style.Default].BackColor = Color.FromArgb(0x282923);
                pTextData.Styles[Style.Default].ForeColor = Color.LightGray;
                pTextData.StyleClearAll();

                // Set the Styles to replicate Sublime
                pTextData.Styles[Style.Xml.XmlStart].ForeColor = Color.White;
                pTextData.Styles[Style.Xml.XmlEnd].ForeColor = Color.White;
                pTextData.Styles[Style.Xml.Other].ForeColor = Color.White;
                pTextData.Styles[Style.Xml.Attribute].ForeColor = Color.FromArgb(0xA7EC21);
                pTextData.Styles[Style.Xml.Entity].ForeColor = Color.FromArgb(0xA7EC21);
                pTextData.Styles[Style.Xml.Comment].ForeColor = Color.FromArgb(0xD8D8D8);
                pTextData.Styles[Style.Xml.Tag].ForeColor = Color.FromArgb(0xFF007F);
                pTextData.Styles[Style.Xml.TagEnd].ForeColor = Color.FromArgb(0xFF007F);
                pTextData.Styles[Style.Xml.DoubleString].ForeColor = Color.FromArgb(0xECE47E);
                pTextData.Styles[Style.Xml.SingleString].ForeColor = Color.FromArgb(0xECE47E);

                // Set the XML Lexer
                pTextData.Lexer = Lexer.Xml;

                // Show line numbers
                pTextData.Margins[0].Width = pTextData.TextWidth(Style.LineNumber, new string('9', 8)) + 2;

                // Enable folding
                pTextData.SetProperty("fold", "1");
                pTextData.SetProperty("fold.compact", "1");
                pTextData.SetProperty("fold.html", "1");

                // Use Margin 2 for fold markers
                pTextData.Margins[2].Type = MarginType.Symbol;
                pTextData.Margins[2].Mask = Marker.MaskFolders;
                pTextData.Margins[2].Sensitive = true;
                pTextData.Margins[2].Width = 20;

                // Reset folder markers
                for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
                {
                    pTextData.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    pTextData.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Style the folder markers
                pTextData.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                pTextData.Markers[Marker.Folder].SetBackColor(SystemColors.ControlText);
                pTextData.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                pTextData.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                pTextData.Markers[Marker.FolderEnd].SetBackColor(SystemColors.ControlText);
                pTextData.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                pTextData.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                pTextData.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                pTextData.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                pTextData.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
            }
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

        #region Helpers - File
        private void AddFileEntry(PackFileEntry pEntry)
        {
            if (pTreeView.Nodes[0] is PackNode pRoot)
            {
                if (pRoot.Tag is IPackStreamVerBase pStream)
                {
                    pStream.GetFileList().Add(pEntry);
                }
            }
        }
        #endregion

        #region Helpers - Save
        private void OnSaveBegin(object sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker)
            {
                IPackStreamVerBase pStream = pProgress.Stream;
                if (pStream == null)
                {
                    return;
                }
                pProgress.Start();
                pStream.GetFileList().Sort();
                SaveData(pProgress.Path, pStream.GetFileList());
                uint dwFileCount = (uint)pStream.GetFileList().Count;
                StringBuilder sFileString = new StringBuilder();
                foreach (PackFileEntry pEntry in pStream.GetFileList())
                {
                    sFileString.Append(pEntry.ToString());
                }
                pSaveWorkerThread.ReportProgress(96);
                byte[] pFileString = Encoding.UTF8.GetBytes(sFileString.ToString().ToCharArray());
                byte[] pHeader = CryptoMan.Encrypt(pStream.GetVer(), pFileString, BufferManipulation.AES_ZLIB, out uint uHeaderLen, out uint uCompressedHeaderLen, out uint uEncodedHeaderLen);
                pSaveWorkerThread.ReportProgress(97);
                byte[] pFileTable;
                using (MemoryStream pOutStream = new MemoryStream())
                {
                    using (BinaryWriter pWriter = new BinaryWriter(pOutStream))
                    {
                        foreach (PackFileEntry pEntry in pStream.GetFileList())
                        {
                            pEntry.FileHeader.Encode(pWriter);
                        }
                    }
                    pFileTable = pOutStream.ToArray();
                }
                pSaveWorkerThread.ReportProgress(98);
                pFileTable = CryptoMan.Encrypt(pStream.GetVer(), pFileTable, BufferManipulation.AES_ZLIB, out uint uDataLen, out uint uCompressedDataLen, out uint uEncodedDataLen);
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
        }

        private void OnSaveChanges(object sender, EventArgs e)
        {
            if (!pUpdateDataBtn.Visible)
            {
                return;
            }

            if (pTreeView.SelectedNode is PackNode pNode && pNode.Data != null)
            {
                if (pNode.Tag is PackFileEntry pEntry)
                {
                    string sData = pTextData.Text;
                    byte[] pData = Encoding.UTF8.GetBytes(sData.ToCharArray());
                    if (pNode.Data != pData)
                    {
                        pEntry.Data = pData;
                        pEntry.Changed = true;
                    }
                }
            }
        }

        private void OnSaveComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(pProgress, e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pProgress.Finish();
            pProgress.Close();

            TimeSpan pInterval = TimeSpan.FromMilliseconds(pProgress.ElapsedTime);
            NotifyMessage(string.Format("Successfully saved in {0} minutes and {1} seconds!", pInterval.Minutes, pInterval.Seconds), MessageBoxIcon.Information);

            // Perform heavy cleanup
            System.GC.Collect();
        }

        private void OnSaveProgress(object sender, ProgressChangedEventArgs e)
        {
            pProgress.UpdateProgressBar(e.ProgressPercentage);
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
                            {
                                dwBufferFlag = BufferManipulation.XOR;
                            }
                            else if (pEntry.Name.EndsWith(".png"))
                            {
                                dwBufferFlag = BufferManipulation.AES;
                            }
                            else
                            {
                                dwBufferFlag = BufferManipulation.AES_ZLIB;
                            }

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
                        pWriter.Write(CryptoMan.Encrypt(uVer, pEntry.Data, pEntry.FileHeader.GetBufferFlag(), out uint uLen, out uint uCompressed, out uint uEncoded));

                        // Apply the file size changes from the new buffer
                        pHeader.SetFileSize(uLen);
                        pHeader.SetCompressedFileSize(uCompressed);
                        pHeader.SetEncodedFileSize(uEncoded);

                        // Update the Entry's index to the new current index
                        pEntry.Index = nCurIndex;

                        nCurIndex++;
                        uOffset += pHeader.GetEncodedFileSize();
                    }
                    // If the entry is unchanged, parse the block from the original offsets
                    else
                    {
                        // Make sure the entry has a parsed file header from load
                        if (pHeader != null)
                        {
                            // Update the initial versioning before any future crypto calls
                            if (pHeader.GetVer() != uVer)
                            {
                                uVer = pHeader.GetVer();
                            }

                            // Access the current encrypted block data from the memory map initially loaded
                            using (MemoryMappedViewStream pBuffer = pDataMappedMemFile.CreateViewStream((long)pHeader.GetOffset(), (long)pHeader.GetEncodedFileSize()))
                            {
                                byte[] pSrc = new byte[pHeader.GetEncodedFileSize()];

                                if (pBuffer.Read(pSrc, 0, (int)pHeader.GetEncodedFileSize()) == pHeader.GetEncodedFileSize())
                                {
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
                            }
                        }
                    }
                    // Allow the remaining 5% for header file write progression
                    pSaveWorkerThread.ReportProgress((int)(((double)(nCurIndex - 1) / (double)aEntry.Count) * 95.0d));
                }
            }
        }
        #endregion

        #region Helpers - Search
        private void LoadSearchForm(object sender, EventArgs e)
        {
            Form form = new Form { Text = "Search" };
            Label label = new Label() { Text = "Input Text", Location = new Point(10, 10), AutoSize = true };
            TextBox txtBox = new TextBox() { Name = "searchTxtBox", Size = new Size(220, 0), Location = new Point(10, 25) };
            Button button = new Button() { AutoSize = true, Location = new Point(155, 50), Text = "OK" };
            form.Controls.Add(label);
            form.Controls.Add(txtBox);
            form.Controls.Add(button);
            form.Controls[2].Click += new EventHandler(OnSearchButtonClicked);
            form.Controls[1].KeyDown += new KeyEventHandler(ActionSearchForm);
            form.Size = new Size(260, 120);
            form.AutoSize = false;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.ShowDialog();
        } // Search form

        private void ActionSearchForm(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ActiveForm.Close();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                OnSearchButtonClicked(sender, e);
            }
        }

        private void OnSearchButtonClicked(object sender, EventArgs e)
        {
            Form form = ActiveForm;
            string input = form.Controls[1].Text;

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pTextData.Text))
            {
                NotifyMessage("You have to open a file first in order to search a text.", MessageBoxIcon.Information);
                ActiveForm.Close();
                return;
            }

            // Indicators 0-7 could be in use by a lexer
            // so we'll use indicator 8 to highlight words.
            const int NUM = 8;

            // Remove all uses of our indicator
            pTextData.IndicatorCurrent = NUM;
            pTextData.IndicatorClearRange(0, pTextData.TextLength);

            // Update indicator appearance
            pTextData.Indicators[NUM].Style = IndicatorStyle.StraightBox;
            pTextData.Indicators[NUM].Under = true;
            pTextData.Indicators[NUM].ForeColor = Color.Yellow;
            pTextData.Indicators[NUM].OutlineAlpha = 50;
            pTextData.Indicators[NUM].Alpha = 50;

            // Search the document
            pTextData.TargetStart = 0;
            pTextData.TargetEnd = pTextData.TextLength;
            pTextData.SearchFlags = SearchFlags.None;

            while (pTextData.SearchInTarget(input) != -1)
            {
                // Mark the search results with the current indicator
                pTextData.IndicatorFillRange(pTextData.TargetStart, pTextData.TargetEnd - pTextData.TargetStart);

                // Search the remainder of the document
                pTextData.TargetStart = pTextData.TargetEnd;
                pTextData.TargetEnd = pTextData.TextLength;
            }

            ActiveForm.Close();
        }
        #endregion

        #region Helpers - Export
        private void OnExportNodeList(string sDir, PackNodeList pList)
        {
            if (!Directory.Exists(sDir))
            {
                Directory.CreateDirectory(sDir);
            }

            foreach (KeyValuePair<string, PackNodeList> pChild in pList.Children)
            {
                //PackNode pGrandChild = new PackNode(pChild.Value, pChild.Key);
                if (!Directory.Exists(sDir + pChild.Key))
                {
                    Directory.CreateDirectory(sDir + pChild.Key);
                }
                OnExportNodeList(sDir + pChild.Key, pChild.Value);
            }

            foreach (PackFileEntry pEntry in pList.Entries.Values)
            {
                IPackFileHeaderVerBase pFileHeader = pEntry.FileHeader;
                if (pFileHeader != null)
                {
                    PackNode pChild = new PackNode(pEntry, pEntry.TreeName);
                    if (pChild.Data == null)
                    {
                        pChild.Data = CryptoMan.DecryptData(pFileHeader, pDataMappedMemFile);
                        File.WriteAllBytes(sDir + pChild.Name, pChild.Data);

                        // Nullify the data as it was previously.
                        pChild.Data = null;
                    }
                    else
                    {
                        File.WriteAllBytes(sDir + pChild.Name, pChild.Data);
                    }
                }
            }
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
                    {
                        if (pNode.Data == null)
                        {
                            pNode.Data = CryptoMan.DecryptData(pFileHeader, pDataMappedMemFile);
                        }
                    }
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
            Form prompt = new Form()
            {
                Width = 290,
                Height = 80,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox textBox = new TextBox() { Left = 10, Top = 10, Width = 200 };
            Button confirmation = new Button() { Text = "Ok", Left = 220, Width = 50, Top = 9, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
