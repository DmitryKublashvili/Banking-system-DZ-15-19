using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base_Bank__Substances.DataBase
{

    class DB_Connection
    {
        public SqlConnection sqlConnection { get; private set; }

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;
                                            Initial Catalog=BankSystemDB;
                                            Integrated Security=True;
                                            Connect Timeout=30;
                                            Encrypt=False;
                                            TrustServerCertificate=False;
                                            ApplicationIntent=ReadWrite;
                                            MultiSubnetFailover=False";

        public DB_Connection()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "BankSystemDB",
                IntegratedSecurity = true,
                Pooling = true
            };

            sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
        }
    }
}
