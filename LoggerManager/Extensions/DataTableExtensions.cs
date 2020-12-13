using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A class that contains the extension methods for the data table specifically.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Write a value to cell at a given row and column
        /// </summary>
        /// <typeparam name="T">The value type of the value</typeparam>
        /// <param name="table">Extension method for DataTable</param>
        /// <param name="col">The column of the cell</param>
        /// <param name="row">The row of the cell</param>
        /// <param name="value">The value to write</param>
        public static void SetCellValue<T>(this DataTable table, int col, int row, T value)
        {
            while (table.Rows.Count < row + 1) table.Rows.Add();

            while (table.Columns.Count < col + 1) table.Columns.Add();

            table.Rows[row][col] = value;

            Debug.WriteLine($"Write {value} to ({col}, {row})");
        }

        /// <summary>
        /// Insert a new row to the table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row">The index of the row to insert to.</param>
        public static void InsertRowAt(this DataTable table, int row)
        {
            //while (table.Rows.Count < row + 1) table.Rows.Add();

            DataRow newRow = table.NewRow();

            table.Rows.InsertAt(newRow, row);

            Debug.WriteLine($"Insert {newRow} at {row}.");
        }

        /// <summary>
        /// Convert a class object to a dataset.
        /// </summary>
        /// <param name="classInstance">The class object to be convert. </param>
        /// <returns></returns>
        public static DataSet ToDataset<T>(this T classInstance)
            where T : class, new()
        {
            if (classInstance == null) return null;

            PropertyInfo[] propertyInfos;
            DataTable table = new DataTable();
            DataSet dataset = new DataSet();

            // Set the name of the table 
            table.TableName = classInstance.GetType().Name;

            // Convert object to a table
            (int c, int r) = helper(classInstance);

            // Add the table to a dataset
            dataset.Tables.Add(table);

            // return the dataset
            return dataset;

            // The helper for the recursive functionality
            (int currentCol, int currentRow) helper(object subject, int col = 0, int row = 0, bool recordHeader = true, bool isVertical = false)
            {
                // Get all properties of the subject
                propertyInfos = subject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Loop through all the properties
                foreach (var p in propertyInfos)
                {
                    // If this property contains the skip attribute
                    if (Attribute.IsDefined(p, typeof(SkipAttribute)))
                        // Then skip it.
                        continue;

                    // If this property contains the NewSheet attributes..
                    if (Attribute.IsDefined(p, typeof(NewSheetAttribute)))
                    {
                        // Add the current table to the dataset
                        dataset.Tables.Add(table);

                        // Set the table to a new one
                        table = new DataTable();

                        // Set the name of the table
                        table.TableName = p.Name;

                        // Refresh the current row and col 
                        row = 0; col = 0;

                        // This property name is already the table name so don't have to record it in the file.

                        // Expend the type if this type is not system type
                        (col, row) = helper(p.GetValue(subject), col, row, recordHeader, isVertical);
                    }
                    // Handle List Type
                    else if (p.PropertyType.IsGenericType)
                    {
                        // Add an empty row  before the list except it's the first row.
                        if(row != 0) row++;

                        // Record the Name of the list as list header
                        SetHeader(table, p, col, row, recordHeader);

                        // The generic argument type of the list
                        Type[] listType = p.PropertyType.GetGenericArguments();

                        // Unbox the value object to a list
                        var theList = p.GetValue(subject) as IList;

                        if (theList != null)
                        {
                            // If the list has its list name as first header, increase the row by one
                            if (!Attribute.IsDefined(p, typeof(SkipHeaderAttribute)))  row++;

                            for (int i = 0; i < theList.Count; i++)
                            {
                                // Recursively document the data in the list
                                helper(theList[i], col, row, recordHeader: i == 0, isVertical: true);
                                row++;
                            }

                            row++;
                        }
                        else
                        {
                            if (isVertical)
                                table.SetCellValue(col + 1, row++, "null");
                            else
                                table.SetCellValue(col++, row + 1, "null");
                        }
                    }
                    // Handle none-system types (includes a struct, a class...)
                    else if (p.PropertyType.Namespace != "System")
                    {
                        // Record the name of this special type first then skip a column.
                        SetHeader(table, p, col, row, recordHeader);

                        // Skip a column to log the information.
                        col++;

                        // Expend the type if this type is not system type
                        (col, row) = helper(p.GetValue(subject), col, row, recordHeader, isVertical);

                        // After finishing logging vertically, decrease the column by one to put the subsequent header in the right place
                        if (!isVertical) col--;

                    }
                    // If it's not nested with other properties, add the value
                    else
                    {
                        // Record property name as header
                        SetHeader(table, p, col, row, recordHeader);

                        var val = p.GetValue(subject);

                        if (isVertical)
                        {
                            // Log beneath the header 
                            table.SetCellValue(col++, row + 1, val ?? "null");
                        }
                        else
                        {
                            // Log beside the header
                            table.SetCellValue(col + 1, row++, val ?? "null");
                        }
                    }
                }

                // Return the current col and row
                return (col, row);
            }
        }

        /// <summary>
        /// Convert a class object to a dataset whose cells contains the style name to apply on.
        /// </summary>
        /// <param name="classInstance">The class object to be convert. </param>
        /// <param name="definedStyles">A set of style names that has been defined.</param>
        /// <returns></returns>
        public static DataSet ToDatasetStyle<T>(this T classInstance, HashSet<string> definedStyles)
            where T : class, new()
        {
            if (classInstance == null) return null;

            PropertyInfo[] propertyInfos;
            DataTable table = new DataTable();
            DataSet dataset = new DataSet();

            // Set the name of the table 
            table.TableName = classInstance.GetType().Name;

            // Convert object to a table
            (int c, int r) = helper(classInstance);

            // Add the table to a dataset
            dataset.Tables.Add(table);

            // return the dataset
            return dataset;

            // The helper for the recursive functionality
            (int currentCol, int currentRow) helper(object subject, int col = 0, int row = 0, bool recordHeader = true, bool isVertical = false)
            {
                // Get all properties of the subject
                propertyInfos = subject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Loop through all the properties
                foreach (var p in propertyInfos)
                {
                    // If this property contains the skip attribute
                    if (Attribute.IsDefined(p, typeof(SkipAttribute)))
                        // Then skip it.
                        continue;

                    // If this property contains the NewSheet attributes..
                    if (Attribute.IsDefined(p, typeof(NewSheetAttribute)))
                    {
                        // Add the current table to the dataset
                        dataset.Tables.Add(table);

                        // Set the table to a new one
                        table = new DataTable();

                        // Set the name of the table
                        table.TableName = p.Name;

                        // Refresh the current row and col 
                        row = 0; col = 0;

                        // This property name is already the table name so don't have to record it in the file.

                        // Expend the type if this type is not system type
                        (col, row) = helper(p.GetValue(subject), col, row, recordHeader, isVertical);
                    }
                    // Handle List Type
                    else if (p.PropertyType.IsGenericType)
                    {
                        // Add an empty row  before the list except it's the first row.
                        if (row != 0) row++;

                        // Record header style if needed
                        SetHeaderStyle(table, p, col, row, recordHeader);

                        // The generic argument type of the list
                        Type[] listType = p.PropertyType.GetGenericArguments();

                        // Unbox the value object to a list
                        var theList = p.GetValue(subject) as IList;

                        if (theList != null)
                        {
                            // If the list has its list name as first header, increase the row by one
                            if (!Attribute.IsDefined(p, typeof(SkipHeaderAttribute))) row++;

                            for (int i = 0; i < theList.Count; i++)
                            {
                                // Recursively document the data in the list
                                helper(theList[i], col, row, recordHeader: i == 0, isVertical: true);
                                row++;
                            }

                            row++;
                        }
                    }
                    // Handle none-system types (includes a struct, a class...)
                    else if (p.PropertyType.Namespace != "System")
                    { 
                        // Record header style if needed
                        SetHeaderStyle(table, p, col, row, recordHeader);

                        // Skip a column to log the information.
                        col++;

                        // Expend the type if this type is not system type
                        (col, row) = helper(p.GetValue(subject), col, row, recordHeader, isVertical);

                        // After finishing logging vertically, decrease the column by one to put the subsequent header in the right place
                        if (!isVertical) col--;

                    }
                    // If it's not nested with other properties, add the value
                    else
                    {
                        // Record header style if needed
                        SetHeaderStyle(table, p, col, row, recordHeader);

                        // If the value cell type attribute is applied (override the conditional attribute)
                        if(Attribute.IsDefined(p, typeof(ValueCellStyle)))
                        {
                            var valueCellAttribute = (ValueCellStyle)p.GetCustomAttribute(typeof(ValueCellStyle));

                            // If the value cell attribute is found (override the conditionAttribute)
                            if (valueCellAttribute != null)
                            {
                                if (isVertical)
                                    // Log beneath the header 
                                    table.SetCellValue(col, row + 1, valueCellAttribute.StyleName);
                                else
                                    // Log beside the header
                                    table.SetCellValue(col + 1, row, valueCellAttribute.StyleName);
                            }
                        }
                        // If the conditional attribute is applied
                        else if (Attribute.IsDefined(p, typeof(ConditionCellStyleAttribute)))
                        {
                            // Get the custom attirbute
                            var conditionAttribute = (ConditionCellStyleAttribute)p.GetCustomAttribute(typeof(ConditionCellStyleAttribute));

                            // Find the boolean property
                            var bp = subject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(prop => prop.Name == conditionAttribute.BooleanProperty);

                            // If boolean property is found
                            if (bp != null)
                            {
                                string stylename = "";

                                // Determine the condition
                                if ((bool)bp.GetValue(subject)) stylename = conditionAttribute.StyleNameIfTrue;
                                else stylename = conditionAttribute.StyleNameIfFalse;

                                if (isVertical)
                                    // Log beneath the header 
                                    table.SetCellValue(col, row + 1, stylename);
                                else
                                    // Log beside the header
                                    table.SetCellValue(col + 1, row, stylename);
                            }

                        }


                        if (isVertical) col++;
                        else row++;
                        
                    }
                }

                // Return the current col and row
                return (col, row);
            }
        }

        /// <summary>
        /// Get the specified table if exist. Return null if the table name doesn't exist.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="tableName">The name of the table.</param>
        /// <returns></returns>
        public static DataTable GetTable(this DataTableCollection tables, string tableName)
        {
            if (tables.Contains(tableName))
            {
                foreach (DataTable t in tables) if (t.TableName == tableName) return t;
            }
            return null;
        }

        /// <summary>
        /// Save a <see cref="DataTable"/> to a text file. 
        /// </summary>
        /// <param name="dt">The data table to be converted.</param>
        /// <param name="filename">The output filename.</param>
        /// <param name="displayColumnNames">Display column names.</param>
        public static void SaveToTextFile(this DataTable dt, string filename, bool displayColumnNames = false)
        {
            int[] maxLengths = new int[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                maxLengths[i] = dt.Columns[i].ColumnName.Length;

                foreach (DataRow row in dt.Rows)
                {
                    if (!row.IsNull(i))
                    {
                        int length = row[i].ToString().Length;

                        if (length > maxLengths[i])
                        {
                            maxLengths[i] = length;
                        }
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                if (displayColumnNames)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sw.Write(dt.Columns[i].ColumnName.PadRight(maxLengths[i] + 2));
                    }

                    sw.WriteLine();
                }

                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (!row.IsNull(i)) sw.Write(row[i].ToString().PadRight(maxLengths[i] + 2));
                        else sw.Write(new string(' ', maxLengths[i] + 2));
                    }

                    sw.WriteLine();
                }

                sw.Close();
            }
        }

        /// <summary>
        /// Save a <see cref="DataTable"/> to a csv file. 
        /// </summary>
        /// <param name="dt">The data table to be converted.</param>
        /// <param name="filename">The output filename.</param>
        /// <param name="displayColumnNames">Display column names.</param>
        public static void SaveToCsvFile(this DataTable dt, string filename, bool displayColumnNames = false)
        {
            int[] maxLengths = new int[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                maxLengths[i] = dt.Columns[i].ColumnName.Length;

                foreach (DataRow row in dt.Rows)
                {
                    if (!row.IsNull(i))
                    {
                        int length = row[i].ToString().Length;

                        if (length > maxLengths[i])
                        {
                            maxLengths[i] = length;
                        }
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                if (displayColumnNames)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sw.Write(dt.Columns[i].ColumnName);
                        if (i != dt.Columns.Count - 1) sw.Write(",");
                    }

                    sw.WriteLine();
                }

                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (!row.IsNull(i)) sw.Write(row[i].ToString());
                        else sw.Write(new string(' ', maxLengths[i] + 2));
                        if (i != dt.Columns.Count - 1) sw.Write(",");
                    }

                    sw.WriteLine();
                }

                sw.Close();
            }
        }


        #region Private Methodes

        private static void SetHeader(DataTable table, PropertyInfo subject, int col, int row,  bool setHeader = true)
        {
            // Skip the header?
            if (Attribute.IsDefined(subject, typeof(SkipHeaderAttribute))) return;
            // Rename?
            else if(Attribute.IsDefined(subject, typeof(HeaderNameAttribute)))
            {
                // Get the attribute
                var attr = (HeaderNameAttribute)subject.GetCustomAttribute(typeof(HeaderNameAttribute));

                // Set the name of the table 
                if(setHeader) table.SetCellValue(col, row, attr.HeaderName);
            }
            // No attribute applied
            else
            {
                // Set the name of the table               
                if (setHeader) table.SetCellValue(col, row, subject.Name);
            }

        }

        private static void SetHeaderStyle(DataTable table, PropertyInfo subject, int col, int row, bool setHeader = true)
        {
            // Skip the header?
            if (Attribute.IsDefined(subject, typeof(SkipHeaderAttribute))) return;
            // Apply header style attribute?
            else if (Attribute.IsDefined(subject, typeof(HeaderStyleAttribute)))
            {
                // Get the header attribute (only the first one)
                var header = (HeaderStyleAttribute)subject.GetCustomAttribute(typeof(HeaderStyleAttribute));
                // Set the style name to the cell
                if(setHeader) table.SetCellValue(col, row, header.StyleName);
            }

        }

        #endregion
    }
}
