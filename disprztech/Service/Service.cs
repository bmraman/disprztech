using Dapper;
using disprztech.Data.Interfaces;
using disprztech.Service.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace disprztech.Service
{
    public class Service<T> : IService<T> where T : class
    {

        private readonly IDbConnectionFactory _dbConnection;
        private readonly string _tableName;


        public Service(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
            _tableName = GetTableName(typeof(T));
        }

        private string GetTableName(Type type)
        {
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            if (tableAttr == null)
                throw new InvalidOperationException($"Missing [Table] attribute on {type.Name}");
            return string.IsNullOrWhiteSpace(tableAttr.Schema)
                ? tableAttr.Name
                : $"{tableAttr.Schema}.{tableAttr.Name}";
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                using var conn = _dbConnection.CreateConnection();
                return await conn.QueryAsync<T>($"SELECT * FROM {_tableName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving data from {_tableName}: {ex.Message}", ex);
            }
         }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                using var conn = _dbConnection.CreateConnection();
                return await conn.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE id = @id", new { id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving id {id} from {_tableName}: {ex.Message}", ex);
            }
        }

        public async Task CreateAsync(T entity)
        {
            try
            {
                using var conn = _dbConnection.CreateConnection();
                var properties = typeof(T).GetProperties().Where(p => p.Name.ToLower() != "id");
                var columnNames = string.Join(", ", properties.Select(p => p.Name.ToLower()));
                var paramNames = string.Join(", ", properties.Select(p => "@" + p.Name));
                var sql = $"INSERT INTO {_tableName} ({columnNames}) VALUES ({paramNames})";

                await conn.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting into {_tableName}: {ex.Message}", ex);
            }
        }

        public async Task BulkInsertAsync(IEnumerable<T> entities, string uniqueColumn)
        {
            try
            {
                using var conn = _dbConnection.CreateConnection();

                var props = typeof(T).GetProperties()
                    .Where(p => !Attribute.IsDefined(p, typeof(NotMappedAttribute)))
                    .Where(p => p.Name.ToLowerInvariant() != "id" && p.GetCustomAttribute<KeyAttribute>() == null)
                    .ToArray();

                var columnNames = props.Select(p => p.Name).ToList();
                var columnList = string.Join(", ", columnNames);

                var sql = new StringBuilder($"INSERT INTO {_tableName} ({columnList}) VALUES ");

                var paramData = new DynamicParameters();
                var rows = new List<string>();
                int index = 0;

                foreach (var entity in entities)
                {
                    var rowParams = new List<string>();
                    foreach (var prop in props)
                    {
                        var paramName = $"{prop.Name}_{index}";
                        rowParams.Add($"@{paramName}");
                        paramData.Add(paramName, prop.GetValue(entity));
                    }
                    rows.Add($"({string.Join(", ", rowParams)})");
                    index++;
                }

                sql.Append(string.Join(", ", rows));

                // Build DO UPDATE SET clause
                var updateAssignments = columnNames
                    .Where(c => !string.Equals(c, uniqueColumn, StringComparison.OrdinalIgnoreCase))
                    .Select(c => $"{c} = EXCLUDED.{c}");

                sql.Append($" ON CONFLICT ({uniqueColumn}) DO UPDATE SET ");
                sql.Append(string.Join(", ", updateAssignments));

                await conn.ExecuteAsync(sql.ToString(), paramData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during bulk insert into {_tableName}: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(int id, T entity)
        {
            try
            {
                using var conn = _dbConnection.CreateConnection();
                var properties = typeof(T).GetProperties()
                    .Where(p => p.Name.ToLower() != "id" && !p.Name.Equals("Created", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Automatically set Updated if the property exists
                var updatedDateProp = properties.FirstOrDefault(p => p.Name.Equals("Updated", StringComparison.OrdinalIgnoreCase));
                if (updatedDateProp != null && updatedDateProp.PropertyType == typeof(DateTime))
                {
                    updatedDateProp.SetValue(entity, DateTime.UtcNow);
                }

                var setClause = string.Join(", ", properties.Select(p => $"{p.Name.ToLower()} = @{p.Name}"));
                var sql = $"UPDATE {_tableName} SET {setClause} WHERE id = @id";

                var parameters = new DynamicParameters(entity);
                parameters.Add("id", id);

                await conn.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating {_tableName} with id {id}: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var conn = _dbConnection.CreateConnection();
                var sql = $"DELETE FROM {_tableName} WHERE id = @id";
                await conn.ExecuteAsync(sql, new { id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting from {_tableName} with id {id}: {ex.Message}", ex);
            }
        }
    }
}
