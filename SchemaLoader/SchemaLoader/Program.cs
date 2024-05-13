using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Extracting schema....");

List<KeyValuePair<string, string>> rows = new();
List<TableSchema> dbSchema = new();

using (SqlConnection connection = new SqlConnection("YOUR_CONNECTION_STRING"))
{
    connection.Open();

    // Get the schema
    string sql = @"SELECT SCHEMA_NAME(schema_id) + '.' + o.Name AS 'TableName', c.Name as 'ColumName'
                FROM     sys.columns c 
                         JOIN sys.objects o ON o.object_id = c.object_id 
                WHERE    o.type = 'U' 
                ORDER BY o.Name";

    using (SqlCommand command = new SqlCommand(sql, connection))
    {
        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                rows.Add(new KeyValuePair<string, string>(reader.GetValue(0).ToString(), reader.GetValue(1).ToString()));
            }
        }
    }
}

var groups = rows.GroupBy(x => x.Key);

foreach (var group in groups)
{
    dbSchema.Add(new TableSchema() { TableName = group.Key, Columns = group.Select(x => x.Value).ToList() });
    //use this list
}

Console.WriteLine("Copy the schema below into the Index.cshtml.cs file of the YourOwnData project:");
Console.WriteLine();
Console.WriteLine();

var textLines = new List<string>();

foreach (var table in dbSchema)
{
    var schemaLine = $"- {table.TableName} (";

    foreach (var column in table.Columns)
    {
        schemaLine += column + ", ";
    }

    schemaLine += ")";
    schemaLine = schemaLine.Replace(", )", " )");

    Console.WriteLine(schemaLine);
    textLines.Add(schemaLine);
}

File.WriteAllText(@"Schema.txt", JsonSerializer.Serialize(dbSchema));

public class TableSchema()
{
    public string TableName { get; set; }
    public List<string> Columns { get; set; }
}