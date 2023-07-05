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
                for (int i = 0; i < table.Columns.Count; i++)
                {
					table.Columns[i].DataType = typeof(string);
				}
            }

            // imports all rows.
            foreach (DataTable table in ds.Tables)
            {
                var targetTable = newDs.Tables[table.TableName];
                foreach (DataRow row in table.Rows)
                {
                    var newRow = targetTable.NewRow();

					for (int i = 0; i < table.Columns.Count; i++)
					{
						newRow[i] = row[i];
					}
                    targetTable.Rows.Add(newRow);
                }
            }

            return newDs;
        }
    }
}
