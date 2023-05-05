using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Extensiones
{
    public static class DataSetExtensions
    {
        public static DataSet ToAllStringFields(this DataSet ds)
        {
            // Clone function -> does not copy the data, but just the structure.
            var newDs = ds.Clone();
            foreach (DataTable table in newDs.Tables)
            {
                table.Columns[0].DataType = typeof(string);
                table.Columns[1].DataType = typeof(string);
                table.Columns[2].DataType = typeof(string);
                table.Columns[3].DataType = typeof(string);
                table.Columns[4].DataType = typeof(string);
                table.Columns[5].DataType = typeof(string);
                table.Columns[6].DataType = typeof(string);
                table.Columns[7].DataType = typeof(string);
                table.Columns[8].DataType = typeof(string);
                table.Columns[9].DataType = typeof(string);
                table.Columns[10].DataType = typeof(string);
                table.Columns[11].DataType = typeof(string);
                table.Columns[12].DataType = typeof(string);
                table.Columns[13].DataType = typeof(string);
                table.Columns[14].DataType = typeof(string);
                table.Columns[15].DataType = typeof(string);
                table.Columns[16].DataType = typeof(string);
                table.Columns[17].DataType = typeof(string);
                table.Columns[18].DataType = typeof(string);
                table.Columns[19].DataType = typeof(string);
                table.Columns[20].DataType = typeof(string);
                table.Columns[21].DataType = typeof(string);
            }

            // imports all rows.
            foreach (DataTable table in ds.Tables)
            {
                var targetTable = newDs.Tables[table.TableName];
                foreach (DataRow row in table.Rows)
                {
                    var newRow = targetTable.NewRow();
                    newRow[0] = row[0];
                    newRow[1] = row[1];
                    newRow[2] = row[2];
                    newRow[3] =  row[3];
                    newRow[4] =  row[4];
                    newRow[5] = row[5];
                    newRow[6] = row[6];
                    newRow[7] = row[7];
                    newRow[8] = row[8];
                    newRow[9] = row[9];
                    newRow[10] = row[10];
                    newRow[11] = row[11];
                    newRow[12] = row[12];
                    newRow[13] = row[13];
                    newRow[14] = row[14];
                    newRow[15] = row[15];
                    newRow[16] = row[16];
                    newRow[17] = row[17];
                    newRow[18] = row[18];
                    newRow[19] = row[19];
                    newRow[20] = row[20];
                    newRow[21] = row[21];
                    targetTable.Rows.Add(newRow);
                }
            }

            return newDs;
        }
    }
}
