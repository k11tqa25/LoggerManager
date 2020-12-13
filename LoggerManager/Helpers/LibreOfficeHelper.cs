using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A class that defines the style of a cell
    /// </summary>
    public class CellStyle
    {
        /// <summary>
        ///  The foreground color (text color) of the cell
        /// </summary>
        public Color ForegroundColor { get; set; } = Color.Empty;

        /// <summary>
        /// The background color of the cell
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.Empty;
    }

    /// <summary>
    /// A helper class for Libre Office
    /// </summary>
    public static class LibreOfficeHelper
    {
        // Namespaces. We need this to initialize XmlNamespaceManager so that we can search XmlDocument.
        private static string[,] namespaces = new string[,]
        {
            {"table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"},
            {"office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"},
            {"style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"},
            {"text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"},
            {"draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"},
            {"fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"},
            {"dc", "http://purl.org/dc/elements/1.1/"},
            {"meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"},
            {"number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"},
            {"presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"},
            {"svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"},
            {"chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"},
            {"dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"},
            {"math", "http://www.w3.org/1998/Math/MathML"},
            {"form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"},
            {"script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"},
            {"ooo", "http://openoffice.org/2004/office"},
            {"ooow", "http://openoffice.org/2004/writer"},
            {"oooc", "http://openoffice.org/2004/calc"},
            {"dom", "http://www.w3.org/2001/xml-events"},
            {"xforms", "http://www.w3.org/2002/xforms"},
            {"xsd", "http://www.w3.org/2001/XMLSchema"},
            {"xsi", "http://www.w3.org/2001/XMLSchema-instance"},
            {"rpt", "http://openoffice.org/2005/report"},
            {"of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"},
            {"rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"},
            {"config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0"}
        };

        /// <summary>
        /// Open a ods file
        /// </summary>
        /// <param name="inputFilename">The file to open.</param>
        /// <returns></returns>
        private static (ZipFile, XmlDocument, XmlNamespaceManager) OpenLibreOffice(string inputFilename)
        {
            ZipFile odsZipFile = GetZipFile(inputFilename);

            // Get content.xml file
            XmlDocument contentXml = GetContentXmlFile(odsZipFile);

            // Initialize XmlNamespaceManager
            XmlNamespaceManager nmsManager = InitializeXmlNamespaceManager(contentXml);

            return (odsZipFile, contentXml, nmsManager);
        }

        // Read zip stream (.ods file is zip file).
        private static ZipFile GetZipFile(Stream stream)
        {
            return ZipFile.Read(stream);
        }

        // Read zip file (.ods file is zip file).
        private static ZipFile GetZipFile(string inputFilePath)
        {
            return ZipFile.Read(inputFilePath);
        }

        private static XmlDocument GetContentXmlFile(ZipFile zipFile)
        {
            // Get file(in zip archive) that contains data ("content.xml").
            ZipEntry contentZipEntry = zipFile["content.xml"];

            // Extract that file to MemoryStream.
            Stream contentStream = new MemoryStream();
            contentZipEntry.Extract(contentStream);
            contentStream.Seek(0, SeekOrigin.Begin);

            // Create XmlDocument from MemoryStream (MemoryStream contains content.xml).
            XmlDocument contentXml = new XmlDocument();
            contentXml.Load(contentStream);

            return contentXml;
        }

        private static XmlNamespaceManager InitializeXmlNamespaceManager(XmlDocument xmlDocument)
        {
            XmlNamespaceManager nmsManager = new XmlNamespaceManager(xmlDocument.NameTable);

            for (int i = 0; i < namespaces.GetLength(0); i++)
                nmsManager.AddNamespace(namespaces[i, 0], namespaces[i, 1]);

            return nmsManager;
        }

        /// <summary>
        /// Reads a .ods file and store it in a <see cref="DataSet"/>.
        /// </summary>
        /// <param name="inputFilename">Path to the .ods file.</param>
        /// <returns><see cref="DataSet"/> that represents .ods file.</returns>
        public static DataSet ReadOdsFile(string inputFilename)
        {
            (var zip, var contentXml, var nmsManager) = OpenLibreOffice(inputFilename);

            DataSet odsFile = new DataSet(Path.GetFileName(inputFilename));

            foreach (XmlNode tableNode in GetTableNodes(contentXml, nmsManager))
                odsFile.Tables.Add(GetSheet(tableNode, nmsManager));

            return odsFile;
        }

        // In ODF sheet is stored in table:table node
        private static XmlNodeList GetTableNodes(XmlDocument contentXmlDocument, XmlNamespaceManager nmsManager)
        {
            return contentXmlDocument.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
        }

        private static XmlNodeList GetStyleNodes(XmlDocument contentXmlDocument, XmlNamespaceManager nmsManager)
        {
            return contentXmlDocument.SelectNodes("/office:document-content/office:automatic-styles/style:style", nmsManager);
        }

        private static DataTable GetSheet(XmlNode tableNode, XmlNamespaceManager nmsManager)
        {
            DataTable sheet = new DataTable(tableNode.Attributes["table:name"].Value);

            XmlNodeList rowNodes = tableNode.SelectNodes("table:table-row", nmsManager);

            int rowIndex = 0;
            foreach (XmlNode rowNode in rowNodes)
                GetRow(rowNode, sheet, nmsManager, ref rowIndex);

            return sheet;
        }

        private static void GetRow(XmlNode rowNode, DataTable sheet, XmlNamespaceManager nmsManager, ref int rowIndex)
        {
            XmlAttribute rowsRepeated = rowNode.Attributes["table:number-rows-repeated"];
            if (rowsRepeated == null || Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture) == 1)
            {
                while (sheet.Rows.Count < rowIndex)
                    sheet.Rows.Add(sheet.NewRow());

                DataRow row = sheet.NewRow();

                XmlNodeList cellNodes = rowNode.SelectNodes("table:table-cell", nmsManager);

                int cellIndex = 0;
                foreach (XmlNode cellNode in cellNodes)
                    GetCell(cellNode, row, nmsManager, ref cellIndex);

                sheet.Rows.Add(row);

                rowIndex++;
            }
            else
            {
                rowIndex += Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture);
            }

            // sheet must have at least one cell
            if (sheet.Rows.Count == 0)
            {
                sheet.Rows.Add(sheet.NewRow());
                sheet.Columns.Add();
            }
        }

        private static void GetCell(XmlNode cellNode, DataRow row, XmlNamespaceManager nmsManager, ref int cellIndex)
        {
            XmlAttribute cellRepeated = cellNode.Attributes["table:number-columns-repeated"];

            if (cellRepeated == null)
            {
                DataTable sheet = row.Table;

                while (sheet.Columns.Count <= cellIndex)
                    sheet.Columns.Add();

                row[cellIndex] = ReadCellValue(cellNode);

                cellIndex++;
            }
            else
            {
                cellIndex += Convert.ToInt32(cellRepeated.Value, CultureInfo.InvariantCulture);
            }
        }

        private static string ReadCellValue(XmlNode cell)
        {
            XmlAttribute cellVal = cell.Attributes["office:value"];

            if (cellVal == null)
                return String.IsNullOrEmpty(cell.InnerText) ? null : cell.InnerText;
            else
                return cellVal.Value;
        }

        /// <summary>
        /// Writes a <see cref="DataSet"/> as a .ods file.
        /// </summary>
        /// <param name="odsFile"><see cref="DataSet"/> that represent .ods file.</param>
        /// <param name="outputFilename">The name of the file to save to.</param>
        /// <param name="templateFilename">The template ods file to reference to.</param>
        /// <param name="styleDataset">The dataset that contains the style info of each cell.</param>
        public static void WriteToOdsFile(DataSet odsFile, string outputFilename, string templateFilename, DataSet styleDataset = null)
        {
            (var templateFile, var contentXml, var nmsManager) = OpenLibreOffice(templateFilename);

            XmlNode sheetsRootNode = GetSheetsRootNodeAndRemoveChildrens(contentXml, nmsManager);

            foreach (DataTable sheet in odsFile.Tables)
            {
                if(styleDataset != null) SaveSheet(sheet, sheetsRootNode, styleDataset.Tables.GetTable(sheet.TableName));
                else SaveSheet(sheet, sheetsRootNode);
            }

            SaveContentXml(templateFile, contentXml);

            templateFile.Save(outputFilename);
        }

        /// <summary>
        /// Add styles to the a new template file. You should then save content to this new template file. <br></br>
        /// <strong>Do not save the new template to the same template file.</strong>
        /// </summary>
        /// <param name="styleDictionary">The dictionary that contains the name of the style and the <see cref="CellStyle"/></param>
        /// <param name="templateFilename">The filepath of the template.</param>
        /// <param name="newTemplateFilename">The filepath of the new template.</param>
        public static bool CreateStyleTemplate(Dictionary<string, CellStyle> styleDictionary, string templateFilename, string newTemplateFilename)
        {
            // Never override the template file
            if (templateFilename == newTemplateFilename) return false;

            (var templateFile, var contentXml, var nmsManager) = OpenLibreOffice(templateFilename);

            XmlNodeList styleNodes = GetStyleNodes(contentXml, nmsManager);

            XmlNode styleRootNode = styleNodes.Item(0).ParentNode;

            XmlDocument ownerDocument = styleNodes.Item(0).OwnerDocument;

            foreach (var s in styleDictionary.Keys)
            {
                CellStyle style = styleDictionary[s];

                XmlNode newStyleNode = ownerDocument.CreateElement("style:style", GetNamespaceUri("style"));

                XmlAttribute newStyleName = ownerDocument.CreateAttribute("style:name", GetNamespaceUri("style"));
                newStyleName.Value = s;
                XmlAttribute newStyleFamily = ownerDocument.CreateAttribute("style:family", GetNamespaceUri("style"));
                newStyleFamily.Value = "table-cell";
                XmlAttribute newStyleParent = ownerDocument.CreateAttribute("style:parent-style-name", GetNamespaceUri("style"));
                newStyleParent.Value = "Default";

                newStyleNode.Attributes.Append(newStyleName);
                newStyleNode.Attributes.Append(newStyleFamily);
                newStyleNode.Attributes.Append(newStyleParent);

                if (style.ForegroundColor != Color.Empty)
                {
                    XmlNode textColor = ownerDocument.CreateElement("style:text-properties", GetNamespaceUri("style"));
                    XmlAttribute attr = ownerDocument.CreateAttribute("fo:color", GetNamespaceUri("fo"));
                    attr.Value = style.ForegroundColor.ToHexString();
                    textColor.Attributes.Append(attr);
                    newStyleNode.AppendChild(textColor);
                }
                if (style.BackgroundColor != Color.Empty)
                {
                    XmlNode textColor = ownerDocument.CreateElement("style:table-cell-properties", GetNamespaceUri("style"));
                    XmlAttribute attr = ownerDocument.CreateAttribute("fo:background-color", GetNamespaceUri("fo"));
                    attr.Value = style.BackgroundColor.ToHexString();
                    textColor.Attributes.Append(attr);
                    newStyleNode.AppendChild(textColor);
                }

                styleRootNode.AppendChild(newStyleNode);
            }

            SaveContentXml(templateFile, contentXml);

            try
            {
                templateFile.Save(newTemplateFilename);
            }
            catch
            {
                return false;
            }

            return true;

        }

        private static XmlNode GetSheetsRootNodeAndRemoveChildrens(XmlDocument contentXml, XmlNamespaceManager nmsManager)
        {
            XmlNodeList tableNodes = GetTableNodes(contentXml, nmsManager);

            XmlNode sheetsRootNode = tableNodes.Item(0).ParentNode;
            // remove sheets from template file
            foreach (XmlNode tableNode in tableNodes)
                sheetsRootNode.RemoveChild(tableNode);

            return sheetsRootNode;
        }

        private static void SaveSheet(DataTable sheet, XmlNode sheetsRootNode, DataTable styleSheet = null)
        {
            XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;

            XmlNode sheetNode = ownerDocument.CreateElement("table:table", GetNamespaceUri("table"));

            XmlAttribute sheetName = ownerDocument.CreateAttribute("table:name", GetNamespaceUri("table"));
            sheetName.Value = sheet.TableName;
            sheetNode.Attributes.Append(sheetName);

            SaveColumnDefinition(sheet, sheetNode, ownerDocument);

            SaveRows(sheet, sheetNode, ownerDocument, styleSheet);

            sheetsRootNode.AppendChild(sheetNode);
        }

        private static void SaveColumnDefinition(DataTable sheet, XmlNode sheetNode, XmlDocument ownerDocument)
        {
            XmlNode columnDefinition = ownerDocument.CreateElement("table:table-column", GetNamespaceUri("table"));

            XmlAttribute columnsCount = ownerDocument.CreateAttribute("table:number-columns-repeated", GetNamespaceUri("table"));
            columnsCount.Value = sheet.Columns.Count.ToString(CultureInfo.InvariantCulture);
            columnDefinition.Attributes.Append(columnsCount);

            sheetNode.AppendChild(columnDefinition);
        }

        private static void SaveRows(DataTable sheet, XmlNode sheetNode, XmlDocument ownerDocument, DataTable styleTable= null)
        {
            DataRowCollection rows = sheet.Rows;
            DataRowCollection styledRows = null;

            if (styleTable != null) styledRows = styleTable.Rows;

            for (int i = 0; i < rows.Count; i++)
            {
                XmlNode rowNode = ownerDocument.CreateElement("table:table-row", GetNamespaceUri("table"));

                if(styledRows!= null && styledRows.Count > i) SaveCell(rows[i], rowNode, ownerDocument, styledRows[i]);
                else SaveCell(rows[i], rowNode, ownerDocument);

                sheetNode.AppendChild(rowNode);
            }
        }

        private static void SaveCell(DataRow row, XmlNode rowNode, XmlDocument ownerDocument, DataRow styleRow= null)
        {
            object[] cells = row.ItemArray;
            object[] styledCells = null;
            if (styleRow != null) styledCells = styleRow.ItemArray;

            for (int i = 0; i < cells.Length; i++)
            {
                XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));

                if (row[i] != DBNull.Value)
                {
                    // We save values as text (string)
                    XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
                    valueType.Value = "string";
                    cellNode.Attributes.Append(valueType);                    

                    // Cell style
                    if(styledCells != null)
                    {
                        // If the cell contains the style name
                        if (styleRow.ItemArray.Length > i && styleRow[i] != DBNull.Value)
                        {
                            XmlAttribute cellStyle = ownerDocument.CreateAttribute("table:style-name", GetNamespaceUri("table"));
                            cellStyle.Value = styleRow[i].ToString();
                            cellNode.Attributes.Append(cellStyle);
                        }
                    }

                    XmlElement cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
                    cellValue.InnerText = row[i].ToString();
                    cellNode.AppendChild(cellValue);
                }

                rowNode.AppendChild(cellNode);
            }
        }

        private static void SaveContentXml(ZipFile templateFile, XmlDocument contentXml)
        {
            templateFile.RemoveEntry("content.xml");

            MemoryStream memStream = new MemoryStream();
            contentXml.Save(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            templateFile.AddEntry("content.xml", memStream);
        }

        private static string GetNamespaceUri(string prefix)
        {
            for (int i = 0; i < namespaces.GetLength(0); i++)
            {
                if (namespaces[i, 0] == prefix)
                    return namespaces[i, 1];
            }

            throw new InvalidOperationException("Can't find that namespace URI");
        }
    }
}
