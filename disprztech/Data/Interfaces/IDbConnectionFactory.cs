using System.Data;

namespace disprztech.Data.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
