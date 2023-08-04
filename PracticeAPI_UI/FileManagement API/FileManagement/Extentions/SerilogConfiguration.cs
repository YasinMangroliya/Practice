using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using System.Collections.ObjectModel;
using System.Data;
using Serilog.Core;
using Model;

namespace FileManagement.Extentions
{
    public static class SerilogConfiguration
    {
        public static Logger SerilogConfig(this WebApplicationBuilder webApplicationBuilder, string connectionString)
        {
            var customColumnOption = new ColumnOptions();
            customColumnOption.AdditionalColumns = new Collection<SqlColumn>()
                         {
                             new SqlColumn { DataType = SqlDbType.NVarChar, ColumnName = ErrorDetailsEnum.ExceptionType, AllowNull = true,DataLength=500 },
                             new SqlColumn { DataType = SqlDbType.NVarChar, ColumnName = ErrorDetailsEnum.EndPoint, AllowNull = true,DataLength=500 },
                             new SqlColumn { DataType = SqlDbType.NVarChar, ColumnName = ErrorDetailsEnum.Path, AllowNull = true,DataLength=500 },
                             new SqlColumn { DataType = SqlDbType.Int, ColumnName = ErrorDetailsEnum.StatusCode, AllowNull = true},
                             new SqlColumn { DataType = SqlDbType.NVarChar, ColumnName = ErrorDetailsEnum.StatusMessage, AllowNull = true,DataLength=500 },
                             new SqlColumn { DataType = SqlDbType.BigInt, ColumnName = ErrorDetailsEnum.UserId, AllowNull = true },
                         };
            customColumnOption.Store.Remove(StandardColumn.MessageTemplate);
            customColumnOption.Store.Remove(StandardColumn.Properties);

            var logger = new LoggerConfiguration()
               .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Error)
               .WriteTo.MSSqlServer(connectionString,
                                    new MSSqlServerSinkOptions
                                    {
                                        TableName = "Logs",
                                        SchemaName = "dbo",
                                        AutoCreateSqlTable = true,
                                    }, columnOptions: customColumnOption, restrictedToMinimumLevel: LogEventLevel.Error)
                                    .CreateLogger();
            return logger;
        }
    }
}
