using Base_Bank__Substances.DataBase;
using BaseBankSubstances;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Base_Bank__Substances.DataBase_Interaction
{
    public class DB_LegalEntities
    {
        readonly SqlConnection sqlConnection;

        public DB_LegalEntities()
        {
            sqlConnection = new DB_Connection().sqlConnection;

            sqlConnection.Open();
            //TestBankingSystemCreation.ClientCreate += InsertNewClient;
        }

        public void InsertNewClient(LegalEntity legalEntity)
        {
            SqlConnectionStringBuilder sqlConnectionSB = new SqlConnectionStringBuilder() 
                {
                    DataSource = @"(localdb)\MSSQLLocalDB",
                    InitialCatalog = "XXX DB",
                    IntegratedSecurity = true,
                    Pooling = true
                };

            SqlConnection sqlConnectionTest = new SqlConnection(sqlConnectionSB.ConnectionString);
            sqlConnectionTest.Open();

            //SqlCommand sqlCommandDelete = sqlConnectionTest.CreateCommand();
            //sqlCommandDelete.CommandText = $"TRUNCATE TABLE TableTEST";
            //sqlCommandDelete.ExecuteNonQuery();

            byte accountState = legalEntity.ClientAccount.IsOpen ? (byte)1 : (byte)0;

            SqlCommand sqlCommand = sqlConnectionTest.CreateCommand();

            sqlCommand.CommandText = $"INSERT TableTEST VALUES (" +
                $"'{legalEntity.ID}', " +
                $"'{legalEntity.Name}'," +
                $"'{legalEntity.RegistrationDate:yyyy-MM-dd HH:mm:ss.fff}'," +
                $"'{legalEntity.StateRgistrationNum}'," +
                $"'{legalEntity.StateRegistrationDate:yyyy-MM-dd HH:mm:ss.fff}'," +
                $"'{legalEntity.ClientAddress.CountryName}', " +
                $"'{legalEntity.ClientAddress.CityName}', " +
                $"'{legalEntity.ClientAddress.StreetName}', " +
                $"'{legalEntity.ClientAddress.HouseNumber}'," +
                $"'{legalEntity.ClientAddress.ApartmentNumber}'," +
                $"'{legalEntity.ClientAccount.AccountNumber}', " +
                $"'{legalEntity.ClientAccount.ContractNumber}', " +
                $"'{legalEntity.ClientAccount.ContractDate:yyyy-MM-dd HH:mm:ss.fff}'"; //+
                                                                                       //$"'{legalEntity.ClientAccount.Balans.ToString("0.0000", CultureInfo.InvariantCulture)}'"; //'{accountState}', '{legalEntity.ClientCredit.CreditAmount}', '{legalEntity.ClientCredit.MonthlyInstallment}')";

            //String.Format("{0:0.0000}", legalEntity.ClientAccount.Balans, CultureInfo.InvariantCulture

            //sqlCommand.CommandText = "TRUNCATE TABLE TableTEST";

            sqlCommand.ExecuteNonQuery();

            sqlConnectionTest.Close();




            SqlCommand insertCmd = sqlConnection.CreateCommand();

            string regDate = legalEntity.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss.fff"); 
            string contractDate = legalEntity.ClientAccount.ContractDate.ToString("yyyy-MM-dd HH:mm:ss.fff");


            string test = "XXX";

            insertCmd.CommandText = $"INSERT LegalEntities VALUES ('{test}', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '' )";

            //insertCmd.CommandText = $"INSERT LegalEntities VALUES (" +
            //    $"'{legalEntity.ID}'," +
            //    $"'{legalEntity.Name}'," +
            //    $"'{null}'," +
            //    $"'{legalEntity.StateRgistrationNum}'," +
            //    $"'{legalEntity.StateRegistrationDate}'," +
            //    $"'{legalEntity.ClientAddress.CountryName}'," +
            //    $"'{legalEntity.ClientAddress.CityName}'," +
            //    $"'{legalEntity.ClientAddress.StreetName}'," +
            //    $"'{legalEntity.ClientAddress.HouseNumber}'," +
            //    $"'{legalEntity.ClientAddress.ApartmentNumber}'," +
            //    $"'{legalEntity.ClientAccount.AccountNumber}'," +
            //    $"'{legalEntity.ClientAccount.ContractNumber}'," +
            //    $"'{null}'," +
            //    $"'{legalEntity.ClientAccount.Balans}'," +
            //    $"'{accountState}'," +
            //    $"'{legalEntity.ClientCredit.CreditAmount}'," +
            //    $"'{legalEntity.ClientCredit.MonthlyInstallment}'," +
            //    $"'{legalEntity.ClientCredit.CreditAccountNum}'," +
            //    $"'{legalEntity.ClientCredit.CreditStartDate}'," +
            //    $"'{legalEntity.ClientCredit.CreditPeriod}'," +
            //    $"'{legalEntity.ClientCredit.CreditContractNumber}'," +
            //    $"'{null}')";    //legalEntity.ClientCredit.CreditInterest

            //MessageBox.Show(sqlConnection.State.ToString());

            //return;

            //insertCmd.ExecuteNonQuery();

            //sqlConnection.Close();
        }
    }
}
