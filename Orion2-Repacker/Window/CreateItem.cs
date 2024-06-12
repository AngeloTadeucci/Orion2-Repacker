using System.IO.MemoryMappedFiles;
using System.Text;
using System.Xml;
using Orion.Crypto.Common;
using Orion.Crypto.Stream;
using Orion.Window.Common;
using static Orion.Crypto.CryptoMan;

namespace Orion.Window;
public partial class CreateItem : Form {
    private readonly string ImageFilePath;
    private readonly string ItemFilePath;
    private readonly string TexturesFilePath;
    private readonly string XmlFilePath;
    private string FemaleNifFilePath;
    private string[] FemaleTexturePaths = Array.Empty<string>();
    private string ItemXmlFilePath;
    private string MaleNifFilePath;
    private string[] MaleTexturePaths = Array.Empty<string>();
    private string SlotIconFilePath;

    public CreateItem(string xmlFilePath, string imageFilePath, string itemFilePath, string texturesFilePath) {
        XmlFilePath = xmlFilePath;
        ImageFilePath = imageFilePath;
        ItemFilePath = itemFilePath;
        TexturesFilePath = texturesFilePath;
        InitializeComponent();
    }

    private void OnCreate(object sender, EventArgs e) {
        if (!ValidateFields()) return;

        List<PackFileEntry> xmlEntries = GetEntriesForFile(XmlFilePath, out IPackStreamVerBase xmlStream, out MemoryMappedFile xmlMemFile);
        if (xmlEntries is null) return;
        PackFileEntry itemNameXml = xmlEntries.FirstOrDefault(x => x.Name.Contains("en/itemname.xml"));
        PopulateDataForEntry(xmlMemFile, itemNameXml);

        // Add new content to itemname.xml
        AddEntryItemName(itemNameXml);

        PackFileEntry korItemDescription = xmlEntries.FirstOrDefault(x => x.Name.Contains("en/koritemdescription.xml"));
        PopulateDataForEntry(xmlMemFile, korItemDescription);

        // Add new content to korItemDescription.xml
        AddEntryKorItemDescription(korItemDescription);

        // Add item xml entry
        AddItemXmlButton(xmlStream);

        SaveFile(xmlStream, xmlMemFile, "Xml.m2d");

        List<PackFileEntry> itemEntries = GetEntriesForFile(ItemFilePath, out IPackStreamVerBase itemStream, out MemoryMappedFile itemMemFile);
        if (itemEntries is null) return;

        if (!string.IsNullOrEmpty(MaleNifFilePath)) {
            string fileName = MaleNifFilePath[(MaleNifFilePath.LastIndexOf('\\') + 1)..];

            if (itemEntries.Any(x => x.Name.Contains(fileName))) {
                NotifyMessage($"There is already an entry with the same name '{fileName}' in Item.m2d, fix it");
                return;
            }
        }

        if (!string.IsNullOrEmpty(MaleNifFilePath)) {
            string fileName = FemaleNifFilePath[(FemaleNifFilePath.LastIndexOf('\\') + 1)..];

            if (itemEntries.Any(x => x.Name.Contains(fileName))) {
                NotifyMessage($"There is already an entry with the same name '{fileName}' in Item.m2d, fix it");
                return;
            }
        }

        AddNifs(itemStream);

        SaveFile(itemStream, itemMemFile, "Item.m2d");

        List<PackFileEntry> imageEntries = GetEntriesForFile(ImageFilePath, out IPackStreamVerBase imageStream, out MemoryMappedFile imageMemFile);
        if (imageEntries is null) return;

        if (imageEntries.Any(x => x.Name.Contains(slotIconFileStatus.Text))) {
            NotifyMessage($"There is already an entry with the same name '{slotIconFileStatus.Text}' in Image.m2d, fix it");
            return;
        }

        AddIconFile(imageStream);

        SaveFile(imageStream, imageMemFile, "Image.m2d");

        List<PackFileEntry> textureEntries = GetEntriesForFile(TexturesFilePath, out IPackStreamVerBase textureStream, out MemoryMappedFile textureMemFile);
        if (textureEntries is null) return;

        foreach (string path in MaleTexturePaths) {
            string maleTextureFileName = path[(path.LastIndexOf('\\') + 1)..];

            if (!textureEntries.Any(x => x.Name.Contains(maleTextureFileName))) continue;

            NotifyMessage($"There is already an entry with the same name '{maleTextureFileName}' in Textures.m2d, fix it");
            return;
        }

        foreach (string path in FemaleTexturePaths) {
            string femaleTextureFileName = path[(path.LastIndexOf('\\') + 1)..];

            if (!textureEntries.Any(x => x.Name.Contains(femaleTextureFileName))) continue;

            NotifyMessage($"There is already an entry with the same name '{femaleTextureFileName}' in Textures.m2d, fix it");
            return;
        }

        AddTextures(textureStream);

        SaveFile(textureStream, textureMemFile, "Textures.m2d");

        NotifyMessage("Done!");
    }

    private void AddIconFile(IPackStreamVerBase imageStream) {
        byte[] pData = File.ReadAllBytes(SlotIconFilePath);
        string fileName = SlotIconFilePath[(SlotIconFilePath.LastIndexOf('\\') + 1)..];

        PackFileEntry pEntry = new PackFileEntry {
            Name = "item/icon/" + fileName,
            Hash = Helpers.CreateHash(SlotIconFilePath),
            Index = 1,
            Changed = true,
            TreeName = "item/icon/" + fileName,
            Data = pData
        };
        imageStream.GetFileList().Add(pEntry);
    }

    private void AddItemXmlButton(IPackStreamVerBase xmlStream) {
        byte[] pData = File.ReadAllBytes(ItemXmlFilePath);
        string fileName = ItemXmlFilePath[(ItemXmlFilePath.LastIndexOf('\\') + 1)..];
        string itemId = fileName.Split('.').First();

        string firstDigit = itemId[..1];
        string next2Digits = itemId.Substring(1, 2);

        PackFileEntry pEntry = new PackFileEntry {
            Name = $"{firstDigit}/{next2Digits}/{fileName}",
            Hash = Helpers.CreateHash(ItemXmlFilePath),
            Index = 1,
            Changed = true,
            TreeName = $"{firstDigit}/{next2Digits}/{fileName}",
            Data = pData
        };
        xmlStream.GetFileList().Add(pEntry);
    }

    private void AddTextures(IPackStreamVerBase textureStream) {
        foreach (string path in MaleTexturePaths) {
            byte[] pData = File.ReadAllBytes(path);
            string fileName = path[(path.LastIndexOf('\\') + 1)..];

            string itemSlot = GetItemSlotDescription(slotNameTextBox.Text);
            if (string.IsNullOrEmpty(itemSlot)) return;
            PackFileEntry pEntry = new PackFileEntry {
                Name = itemSlot + "/" + fileName,
                Hash = Helpers.CreateHash(path),
                Index = 1,
                Changed = true,
                TreeName = itemSlot + "/" + fileName,
                Data = pData
            };
            textureStream.GetFileList().Add(pEntry);
        }

        foreach (string path in FemaleTexturePaths) {
            byte[] pData = File.ReadAllBytes(path);
            string fileName = path[(path.LastIndexOf('\\') + 1)..];

            string itemSlot = GetItemSlotDescription(slotNameTextBox.Text);
            if (string.IsNullOrEmpty(itemSlot)) return;
            PackFileEntry pEntry = new PackFileEntry {
                Name = itemSlot + "/" + fileName,
                Hash = Helpers.CreateHash(path),
                Index = 1,
                Changed = true,
                TreeName = itemSlot + "/" + fileName,
                Data = pData
            };
            textureStream.GetFileList().Add(pEntry);
        }
    }

    private static string GetItemSlotDescription(string itemSlot) {
        switch (itemSlot) {
            case "HR": // Hair
                return "item_hair";
            case "FA": // Face (the literal flesh and eyes)
                return "item_face";
            case "FD": // Face Decoration
                return "item_makeup";
            case "EA": // Earrings
                return "item_earaccessory";
            case "FH": // Face Accessory (Mask, Lollipop, Moustache etc.)
                return "item_forehead";
            case "EY": // Eye-wear (Glasses, eye-patch, some eye masks etc.)
                return "item_eyeaccessory";
            case "ER": // Ear???, literally only 10500001 exists for this slot
                return "";
            case "CP": // Hat/Cap
                return "item_cap";
            case "CL": // Tops, Coats and "Suits". Suits take both CL and PA slots usually.
                return "item_clothes";
            case "PA": // Bottoms/Pants
                return "item_pants";
            case "GL": // Gloves
                return "item_gloves";
            case "SH": // Shoes
                return "item_shoes";
            case "MT": // Cape/Mantle
                return "item_mantle";
            case "PD": // Pendant/Necklace
                return "";
            case "RI": // Ring
                return "";
            case "BE": // Belt
                return "";
            case "RH": // Right-hand. Longsword, Greatsword, Sceptre, Orb etc.
                return "item_righthand";
            case "LH": // Left-hand. Knight shields, Priest codexes.
                return "item_lefthand";
            case "OH": // Both-hand weapons, Thief daggers + Asaassin stars.
                return "item_bothhand";
        }

        return string.Empty;
    }

    private void AddNifs(IPackStreamVerBase itemStream) {
        if (!string.IsNullOrEmpty(MaleNifFilePath)) {
            byte[] pData = File.ReadAllBytes(MaleNifFilePath);
            string fileName = MaleNifFilePath[(MaleNifFilePath.LastIndexOf('\\') + 1)..];
            string itemId = fileName.Split('_').First();

            string firstDigit = itemId[..1];
            string next2Digits = itemId.Substring(1, 2);

            PackFileEntry pEntry = new PackFileEntry {
                Name = firstDigit + "/" + next2Digits + "/" + fileName,
                Hash = Helpers.CreateHash(MaleNifFilePath),
                Index = 1,
                Changed = true,
                TreeName = firstDigit + "/" + next2Digits + "/" + fileName,
                Data = pData
            };
            itemStream.GetFileList().Add(pEntry);
        }

        if (string.IsNullOrEmpty(FemaleNifFilePath)) return;

        {
            byte[] pData = File.ReadAllBytes(FemaleNifFilePath);
            string fileName = FemaleNifFilePath[(FemaleNifFilePath.LastIndexOf('\\') + 1)..];
            string itemId = fileName.Split('_').First();

            string firstDigit = itemId[..1];
            string next2Digits = itemId.Substring(1, 2);

            PackFileEntry pEntry = new PackFileEntry {
                Name = firstDigit + "/" + next2Digits + "/" + fileName,
                Hash = Helpers.CreateHash(FemaleNifFilePath),
                Index = 1,
                Changed = true,
                TreeName = firstDigit + "/" + next2Digits + "/" + fileName,
                Data = pData
            };
            itemStream.GetFileList().Add(pEntry);
        }
    }

    private void AddEntryKorItemDescription(PackFileEntry entry) {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(Encoding.UTF8.GetString(entry.Data));

        XmlNode root = xmlDoc.DocumentElement;
        XmlElement element = xmlDoc.CreateElement("key");

        XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
        idAttribute.InnerText = itemIdTextBox.Text;

        XmlAttribute tooltipDescription = xmlDoc.CreateAttribute("tooltipDescription");
        tooltipDescription.InnerText = tooltipDescriptionTextBox.Text;

        XmlAttribute guideDescription = xmlDoc.CreateAttribute("guideDescription");
        guideDescription.InnerText = guideDescriptionTextBox.Text;

        element.Attributes.Append(idAttribute);
        element.Attributes.Append(tooltipDescription);
        element.Attributes.Append(guideDescription);
        root.AppendChild(element);

        string formattedXml = FormatXml(xmlDoc);

        byte[] pData = Encoding.UTF8.GetBytes(formattedXml.ToCharArray());
        entry.Data = pData;
        entry.Changed = true;
    }

    private void SaveFile(IPackStreamVerBase pStream, MemoryMappedFile pDataMappedMemFile, string fileName) {
        SaveFileDialog pDialog = new SaveFileDialog {
            Title = $"Select the destination to save {fileName}",
            Filter = "MapleStory2 Files|*.m2d",
            FileName = fileName
        };

        if (pDialog.ShowDialog() != DialogResult.OK) return;

        string sPath = pDialog.FileName;

        pStream.GetFileList().Sort();
        SaveData(sPath, pStream.GetFileList(), pDataMappedMemFile);
        uint dwFileCount = (uint)pStream.GetFileList().Count;
        StringBuilder sFileString = new StringBuilder();
        foreach (PackFileEntry pEntry in pStream.GetFileList()) sFileString.Append(pEntry);
        byte[] pFileString = Encoding.UTF8.GetBytes(sFileString.ToString().ToCharArray());
        byte[] pHeader = Encrypt(pStream.GetVer(), pFileString, BufferManipulation.AES_ZLIB, out uint uHeaderLen, out uint uCompressedHeaderLen,
            out uint uEncodedHeaderLen);
        byte[] pFileTable;
        using (MemoryStream pOutStream = new MemoryStream()) {
            using (BinaryWriter pWriter = new BinaryWriter(pOutStream)) {
                foreach (PackFileEntry pEntry in pStream.GetFileList()) pEntry.FileHeader.Encode(pWriter);
            }

            pFileTable = pOutStream.ToArray();
        }

        pFileTable = Encrypt(pStream.GetVer(), pFileTable, BufferManipulation.AES_ZLIB, out uint uDataLen, out uint uCompressedDataLen,
            out uint uEncodedDataLen);
        pStream.SetFileListCount(dwFileCount);
        pStream.SetHeaderSize(uHeaderLen);
        pStream.SetCompressedHeaderSize(uCompressedHeaderLen);
        pStream.SetEncodedHeaderSize(uEncodedHeaderLen);
        pStream.SetDataSize(uDataLen);
        pStream.SetCompressedDataSize(uCompressedDataLen);
        pStream.SetEncodedDataSize(uEncodedDataLen);
        using (BinaryWriter pWriter = new BinaryWriter(File.Create(sPath.Replace(".m2d", ".m2h")))) {
            pWriter.Write(pStream.GetVer());
            pStream.Encode(pWriter);
            pWriter.Write(pHeader);
            pWriter.Write(pFileTable);
        }
    }

    private void AddEntryItemName(PackFileEntry entry) {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(Encoding.UTF8.GetString(entry.Data));

        XmlNode root = xmlDoc.DocumentElement;
        XmlElement element = xmlDoc.CreateElement("key");

        XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
        idAttribute.InnerText = itemIdTextBox.Text;

        XmlAttribute classAttribute = xmlDoc.CreateAttribute("class");
        classAttribute.InnerText = classTextBox.Text;

        XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
        nameAttribute.InnerText = nameTextBox.Text;

        element.Attributes.Append(idAttribute);
        element.Attributes.Append(classAttribute);
        element.Attributes.Append(nameAttribute);
        root.AppendChild(element);

        string formattedXml = FormatXml(xmlDoc);

        byte[] pData = Encoding.UTF8.GetBytes(formattedXml.ToCharArray());
        entry.Data = pData;
        entry.Changed = true;
    }

    private static void PopulateDataForEntry(MemoryMappedFile pDataMappedMemFile, PackFileEntry entry) {
        IPackFileHeaderVerBase pFileHeader2 = entry.FileHeader;
        if (pFileHeader2 == null) return;
        if (entry.Data != null) return;

        entry.Data = DecryptData(pFileHeader2, pDataMappedMemFile);
    }

    private List<PackFileEntry> GetEntriesForFile(string filePath, out IPackStreamVerBase pStream, out MemoryMappedFile pDataMappedMemFile) {
        pStream = null;
        pDataMappedMemFile = null;
        string m2hPath = filePath.Replace(".m2d", ".m2h");

        if (!File.Exists(m2hPath)) {
            string sHeaderName = m2hPath[(m2hPath.LastIndexOf('/') + 1)..];
            NotifyMessage($"Unable to load the {sHeaderName} file.\r\nPlease make sure it exists and is not being used.",
                MessageBoxIcon.Error);
            return null;
        }

        using (BinaryReader pHeader = new BinaryReader(File.OpenRead(m2hPath))) {
            // Construct a new packed stream from the header data
            pStream = PackVer.CreatePackVer(pHeader);

            // Insert a collection containing the file list information [index,hash,name]
            pStream.GetFileList().Clear();
            pStream.GetFileList().AddRange(PackFileEntry.CreateFileList(Encoding.UTF8.GetString(DecryptFileString(pStream, pHeader.BaseStream))));
            // Make the collection of files sorted by their FileIndex for easy fetching
            pStream.GetFileList().Sort();

            // Load the file allocation table and assign each file header to the entry within the list
            byte[] pFileTable = DecryptFileTable(pStream, pHeader.BaseStream);
            using (MemoryStream pTableStream = new MemoryStream(pFileTable)) {
                using (BinaryReader pReader = new BinaryReader(pTableStream)) {
                    IPackFileHeaderVerBase pFileHeader;

                    switch (pStream.GetVer()) {
                        case PackVer.MS2F:
                            for (ulong i = 0; i < pStream.GetFileListCount(); i++) {
                                pFileHeader = new PackFileHeaderVer1(pReader);
                                pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                            }

                            break;
                        case PackVer.NS2F:
                            for (ulong i = 0; i < pStream.GetFileListCount(); i++) {
                                pFileHeader = new PackFileHeaderVer2(pReader);
                                pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                            }

                            break;
                        case PackVer.OS2F:
                        case PackVer.PS2F:
                            for (ulong i = 0; i < pStream.GetFileListCount(); i++) {
                                pFileHeader = new PackFileHeaderVer3(pStream.GetVer(), pReader);
                                pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                            }

                            break;
                    }
                }
            }
        }

        pDataMappedMemFile = MemoryMappedFile.CreateFromFile(filePath);

        return pStream.GetFileList();
    }

    private static string FormatXml(XmlDocument xmlDoc) {
        string result = "";

        MemoryStream mStream = new MemoryStream();
        XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
        try {
            writer.Formatting = Formatting.Indented;

            // Write the XML into a formatting XmlTextWriter
            xmlDoc.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();

            // Have to rewind the MemoryStream in order to read
            // its contents.
            mStream.Position = 0;

            // Read MemoryStream contents into a StreamReader.
            StreamReader sReader = new StreamReader(mStream);

            // Extract the text from the StreamReader.
            result = sReader.ReadToEnd();
        } catch (XmlException) {
            // Handle the exception
        }

        mStream.Close();
        writer.Close();

        return result;
    }

    private static void SaveData(string sDataPath, List<PackFileEntry> aEntry, MemoryMappedFile pDataMappedMemFile) {
        // Declare MS2F as the initial version until specified.
        uint uVer = PackVer.MS2F;
        // Re-calculate all file offsets from start to finish
        ulong uOffset = 0;
        // Re-calculate all file indexes from start to finish
        int nCurIndex = 1;

        using (BinaryWriter pWriter = new BinaryWriter(File.Create(sDataPath))) {
            // Iterate all file entries that exist
            foreach (PackFileEntry pEntry in aEntry) {
                IPackFileHeaderVerBase pHeader = pEntry.FileHeader;

                // If the entry was modified, or is new, write the modified data block
                if (pEntry.Changed) {
                    // If the header is null (new entry), then create one
                    if (pHeader == null) {
                        // Hacky way of doing this, but this follows Nexon's current conventions.
                        uint dwBufferFlag;
                        if (pEntry.Name.EndsWith(".usm"))
                            dwBufferFlag = BufferManipulation.XOR;
                        else if (pEntry.Name.EndsWith(".png"))
                            dwBufferFlag = BufferManipulation.AES;
                        else
                            dwBufferFlag = BufferManipulation.AES_ZLIB;

                        switch (uVer) {
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
                    } else {
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
                    continue;
                }
                // If the entry is unchanged, parse the block from the original offsets

                // Make sure the entry has a parsed file header from load
                if (pHeader == null) continue;
                // Update the initial versioning before any future crypto calls
                if (pHeader.GetVer() != uVer) uVer = pHeader.GetVer();

                // Access the current encrypted block data from the memory map initially loaded
                using (MemoryMappedViewStream pBuffer =
                       pDataMappedMemFile.CreateViewStream((long)pHeader.GetOffset(), pHeader.GetEncodedFileSize())) {
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
            }
        }
    }

    private bool ValidateFields() {
        if (string.IsNullOrEmpty(itemIdTextBox.Text.Trim())) {
            NotifyMessage("Please add an item id.");
            return false;
        }

        if (!int.TryParse(itemIdTextBox.Text.Trim(), out int itemId)) {
            NotifyMessage("Cannot parse item id to integer, it has letters or number is too big or too small.");
            return false;
        }

        if (itemId <= 0) {
            NotifyMessage("Item id cannot be zero or negative.");
            return false;
        }

        if (string.IsNullOrEmpty(classTextBox.Text.Trim())) {
            NotifyMessage("Please add an class name.");
            return false;
        }

        if (string.IsNullOrEmpty(nameTextBox.Text.Trim())) {
            NotifyMessage("Please add an item name.");
            return false;
        }

        if (string.IsNullOrEmpty(tooltipDescriptionTextBox.Text.Trim())) {
            NotifyMessage("Please add an tool tip description.");
            return false;
        }

        if (string.IsNullOrEmpty(guideDescriptionTextBox.Text.Trim())) {
            NotifyMessage("Please add an guide description.");
            return false;
        }

        if (string.IsNullOrEmpty(ItemXmlFilePath)) {
            NotifyMessage("Please add an item xml file.");
            return false;
        }

        if (string.IsNullOrEmpty(slotNameTextBox.Text.Trim())) {
            NotifyMessage("Please add an item slot name.");
            return false;
        }

        if (string.IsNullOrEmpty(SlotIconFilePath)) {
            NotifyMessage("Please add an slot icon image.");
            return false;
        }

        if (string.IsNullOrEmpty(MaleNifFilePath) && string.IsNullOrEmpty(FemaleNifFilePath)) {
            NotifyMessage("Add at least one nif file for one of the genders");
            return false;
        }

        if (MaleTexturePaths.Length == 0 && FemaleTexturePaths.Length == 0) {
            NotifyMessage("Add at least one texture file for one of the genders");
            return false;
        }

        if (!string.IsNullOrEmpty(MaleNifFilePath) && MaleTexturePaths.Length == 0) {
            NotifyMessage("You have an male nif file but not textures for it.");
            return false;
        }

        if (!string.IsNullOrEmpty(FemaleNifFilePath) && FemaleTexturePaths.Length == 0) {
            NotifyMessage("You have an female nif file but not textures for it.");
            return false;
        }

        if (string.IsNullOrEmpty(MaleNifFilePath) && MaleTexturePaths.Length != 0) {
            NotifyMessage("You have male texture files but no nif for it.");
            return false;
        }

        if (string.IsNullOrEmpty(FemaleNifFilePath) && FemaleTexturePaths.Length != 0) {
            NotifyMessage("You have female texture files but no nif for it.");
            return false;
        }

        return true;
    }

    private void NotifyMessage(string sText, MessageBoxIcon eIcon = MessageBoxIcon.None) {
        MessageBox.Show(this, sText, Text, MessageBoxButtons.OK, eIcon);
    }

    private void OnFindMaleNifFile(object sender, EventArgs e) {
        OpenFileDialog pDialog = new OpenFileDialog {
            Title = "Select the male nif file",
            Filter = "Nif file|*.nif",
            Multiselect = false
        };

        if (pDialog.ShowDialog() != DialogResult.OK) return;

        MaleNifFilePath = pDialog.FileName;

        maleNifFileStatus.Text = MaleNifFilePath[(MaleNifFilePath.LastIndexOf('\\') + 1)..];
    }

    private void OnFindFemaleNifFile(object sender, EventArgs e) {
        OpenFileDialog pDialog = new OpenFileDialog {
            Title = "Select the female nif file",
            Filter = "Nif file|*.nif",
            Multiselect = false
        };

        if (pDialog.ShowDialog() != DialogResult.OK) return;
        FemaleNifFilePath = pDialog.FileName;

        femaleNifFileStatus.Text = FemaleNifFilePath[(FemaleNifFilePath.LastIndexOf('\\') + 1)..];
    }

    private void OnFindMaleTextureFiles(object sender, EventArgs e) {
        OpenFileDialog pDialog = new OpenFileDialog {
            Title = "Select the male texture files",
            Filter = "Texture files|*.dds",
            Multiselect = true
        };

        if (pDialog.ShowDialog() != DialogResult.OK) return;
        MaleTexturePaths = pDialog.FileNames;

        List<string> filenames = MaleTexturePaths.Select(path => path[(path.LastIndexOf('\\') + 1)..]).ToList();
        maleTextureStatus.Text = string.Join("\r\n", filenames);
    }

    private void OnFindFemaleTextureFiles(object sender, EventArgs e) {
        OpenFileDialog pDialog = new OpenFileDialog {
            Title = "Select the female texture files",
            Filter = "Texture files|*.dds",
            Multiselect = true
        };

        if (pDialog.ShowDialog() != DialogResult.OK) return;
        FemaleTexturePaths = pDialog.FileNames;

        List<string> filenames = FemaleTexturePaths.Select(path => path[(path.LastIndexOf('\\') + 1)..]).ToList();
        femaleTextureStatus.Text = string.Join("\r\n", filenames);
    }

    private void OnFindSlotItemImage(object sender, EventArgs e) {
        OpenFileDialog pDialog = new OpenFileDialog {
            Title = "Select the slot icon image",
            Filter = "Slot icon image|*.png",
            Multiselect = false
        };

        if (pDialog.ShowDialog() != DialogResult.OK) return;
        SlotIconFilePath = pDialog.FileName;

        slotIconFileStatus.Text = SlotIconFilePath[(SlotIconFilePath.LastIndexOf('\\') + 1)..];
        slotIconFileStatus.Visible = true;
    }

    private void OnRemoveMaleNifFile(object sender, EventArgs e) {
        MaleNifFilePath = "";
        maleNifFileStatus.Text = "";
    }

    private void OnRemoveFemaleNifFile(object sender, EventArgs e) {
        FemaleNifFilePath = "";
        femaleNifFileStatus.Text = "";
    }

    private void OnRemoveMaleTextureFiles(object sender, EventArgs e) {
        MaleTexturePaths = Array.Empty<string>();
        maleTextureStatus.Text = "";
    }

    private void OnRemoveFemaleTextureFiles(object sender, EventArgs e) {
        FemaleTexturePaths = Array.Empty<string>();
        femaleTextureStatus.Text = "";
    }

    private void OnRemoveSlotIconFile(object sender, EventArgs e) {
        SlotIconFilePath = "";
        slotIconFileStatus.Text = "";
        slotIconFileStatus.Visible = false;
    }

    private void OnAddItemXmlButton(object sender, EventArgs e) {
        OpenFileDialog pDialog = new OpenFileDialog {
            Title = "Select the item xml file",
            Filter = "Item xml file|*.xml",
            Multiselect = false
        };

        if (pDialog.ShowDialog() != DialogResult.OK) return;
        ItemXmlFilePath = pDialog.FileName;

        itemXmlStatus.Text = ItemXmlFilePath[(ItemXmlFilePath.LastIndexOf('\\') + 1)..];
    }

    private void OnRemoveItemXmlButton(object sender, EventArgs e) {
        ItemXmlFilePath = "";
        itemXmlStatus.Text = "";
    }
}