using Base_Bank__Substances.DataBase_Interaction;
using BaseBankSubstances.Bank_operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances
{
    class TestBankingSystemCreation
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        static TestBankingSystemCreation()
        {
            //объявление коллекций клиентов
            testIndividuals = new ObservableCollection<Individual>();
            test_VIP_Individuals = new ObservableCollection<VIP_Individual>();
            testLegalEntities = new ObservableCollection<LegalEntity>();

            operationsJornal = new OperationsJornal();

            // наполнение коллекций клиентами
            bankSistem = TestBankSistemCreate(33, 25, 49);
        }

        public static OperationsJornal operationsJornal;

        public static BankSistem bankSistem;

        private static Random random = new Random();

        private static ObservableCollection<Individual> testIndividuals;

        private static ObservableCollection<VIP_Individual> test_VIP_Individuals;

        private static ObservableCollection<LegalEntity> testLegalEntities;

        public static event Action<Client> ClientCreate;

        /// <summary>
        /// Автоматически создает тестовую банковскую базу случайных клиентов  
        /// </summary>
        /// <param name="numOfIndividuals"></param>
        /// <param name="numOfVIP_Individuals"></param>
        /// <param name="numOfLegalEntities"></param>
        /// <returns></returns>
        private static BankSistem TestBankSistemCreate(int numOfIndividuals = 0, int numOfVIP_Individuals = 0, int numOfLegalEntities = 0)
        {

            for (int i = 0; i < numOfIndividuals; i++)
            {
                testIndividuals.Add(GetIndividualClient(typeof(Individual)) as Individual);
            }


            for (int i = 0; i < numOfVIP_Individuals; i++)
            {
                test_VIP_Individuals.Add(GetIndividualClient(typeof(VIP_Individual)) as VIP_Individual);
            }


            for (int i = 0; i < numOfLegalEntities; i++)
            {
                testLegalEntities.Add(GetLegalEntityClient());
            }

            SystemRecalculationOnCurrentDate();

            return new BankSistem(testIndividuals, test_VIP_Individuals, testLegalEntities);
        }


        /// <summary>
        /// Возвращает "случайного", автоматически созданного клиента (юрилическое лицо)
        /// </summary>
        /// <returns></returns>
        private static LegalEntity GetLegalEntityClient()
        {
            DateTime registrationDate = new DateTime(random.Next(2015, 2021), random.Next(1, 13), random.Next(1, 29));

            LegalEntity newClient = new LegalEntity(
                registrationDate,
                GetRandomEntityName(),
                Guid.NewGuid().ToString(),
                new Address("Belarus",
                            "Minsk",
                            "Kolasa str.",
                            $"{random.Next(1, 300)}a",
                            $"{random.Next(1, 500)} - {Guid.NewGuid().ToString()[0]}"),
                new Account(Account.GetAccountNumber(),
                            $"{Guid.NewGuid().ToString().Substring(5, 11)}",
                            registrationDate,
                            random.Next(-100000, 1000000),
                            random.Next(100) % 10 != 0),
                new Credit(),
                new List<TransactionInfo>(),
                false,
                new DateTime(random.Next(1933, 2015), random.Next(1, 13), random.Next(1, 29)),
                random.Next(100000, 999999).ToString());

            newClient.ClientCredit = new Credit();

            // добавление транзакции об открытии текущего счета
            Account.AddTransactionInfoAccountOpen(newClient);

            if (random.Next(9)%2 == 0)
            {
                newClient.ClientCredit = new Credit()
                {
                    CreditAccountNum = Credit.GetAccountNumber(),
                    CreditAmount = random.Next(10000, 1000000),
                    CreditContractNumber = $"{Guid.NewGuid().ToString().Substring(5, 11)}",
                    CreditStartDate = newClient.RegistrationDate.AddDays(random.Next((DateTime.Now - newClient.RegistrationDate).Days)),
                    CreditPeriod = random.Next(0, 10) % 2 == 0 ? 24 : 36
                };
                newClient.ClientCredit.SetCreditInterest(newClient);
                newClient.ClientCredit.SetMonthlyInstallment();
            }

            //ClientCreate?.Invoke(newClient);   // вызов события для добавления клиента в базу данных
            new DB_LegalEntities().InsertNewClient(newClient);

            return newClient;
        }


        /// <summary>
        /// Возвращает "случайного", автоматически созданного клиента (физическое лицо)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Individual GetIndividualClient(Type type)   
        {
            Individual newClient = type == typeof(Individual) ? new Individual() : new VIP_Individual();

            newClient.RegistrationDate = new DateTime(random.Next(2015, 2021), random.Next(1, 13), random.Next(1, 29));
            newClient.Name = GetRandomName();
            newClient.PassportNum = random.Next(100000, 999999).ToString();
            newClient.DateOfBirth = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29));
            newClient.ID = Guid.NewGuid().ToString();
            newClient.TransactionHistory = new List<TransactionInfo>();
            newClient.ClientAddress = new Address(
                "Belarus",
                "Minsk",
                "Kolasa str.",
                $"{random.Next(1, 300)}a",
                $"{random.Next(1, 500)}-{Guid.NewGuid().ToString()[0]}");

            newClient.ClientAccount = new Account(
                Account.GetAccountNumber(),
                $"{Guid.NewGuid().ToString().Substring(5, 11)}",
                newClient.RegistrationDate,
                0,
                random.Next(100) % 10 != 0);

            newClient.ClientAccount.SetBalans(newClient.ClientAccount.IsOpen ? random.Next(-100000, 1000000) : 0);

            newClient.ClientDeposit = new Deposit();
            newClient.ClientCredit = new Credit();

            // добавление транзакции об открытии текущего счета
            Account.AddTransactionInfoAccountOpen(newClient);

            if (newClient.ClientAccount.IsOpen && (random.Next(0,9)%2==0 || random.Next()%3==0))
            {
                newClient.ClientDeposit = new Deposit()
                {
                    DepositAccountNum = Deposit.GetAccountNumber(),
                    DepositAmount = random.Next(100000, 1000000),
                    DepositContractNumber = $"{Guid.NewGuid().ToString().Substring(5, 11)}",
                    DepositStartDate = newClient.RegistrationDate.AddDays(random.Next((DateTime.Now - newClient.RegistrationDate).Days)),
                    DepositPeriod = random.Next(0, 10) % 2 == 0 ? 24 : 36,
                    IsCapitalised = random.Next(99) % 2 == 0
                };
                newClient.ClientDeposit.SetDepositInterest(newClient);
            }

            if (newClient.ClientAccount.IsOpen && (random.Next(0,9) % 2 == 0 || random.Next() % 3 == 0) && newClient.ClientAccount.Balans >= 0)
            {
                newClient.ClientCredit = new Credit()
                {
                    CreditAccountNum = Credit.GetAccountNumber(),
                    CreditAmount = random.Next(10000, 1000000),
                    CreditContractNumber = $"{Guid.NewGuid().ToString().Substring(5, 11)}",
                    CreditStartDate = newClient.ClientDeposit.DepositAmount <= 0 ? newClient.RegistrationDate.AddDays(random.Next((DateTime.Now - newClient.RegistrationDate).Days)) 
                    : newClient.ClientDeposit.DepositStartDate.AddDays(random.Next((DateTime.Now - newClient.ClientDeposit.DepositStartDate).Days)),
                    CreditPeriod = random.Next(0, 10) % 2 == 0 ? 24 : 36
                };
                newClient.ClientCredit.SetCreditInterest(newClient);
                newClient.ClientCredit.SetMonthlyInstallment(); 
            }
            return newClient;
        }


        /// <summary>
        /// Перерасчет банковской системы на текущую дату
        /// </summary>
        private static void SystemRecalculationOnCurrentDate()
        {
            foreach (var client in testIndividuals)
            {
                if (client.ClientCredit.CreditAmount == 0 && client.ClientDeposit.DepositAmount == 0)
                {
                    continue;
                }

                // расчетная дата начинается с даты создания текущео сета (даты договора)
                DateTime settlementDate = client.ClientAccount.ContractDate;

                while (settlementDate <= DateTime.Now && (client.ClientCredit.CreditAmount > 0 || client.ClientDeposit.DepositAmount > 0))
                {
                    // добавление записи об открытии депозита
                    if (settlementDate.Date == client.ClientDeposit.DepositStartDate.Date)
                    {
                        Deposit.AddDepositOpenTransactoin(client, settlementDate);
                    }

                    // добавление записи об открытии кредита
                    if (settlementDate.Date == client.ClientCredit.CreditStartDate.Date)
                    {
                        Credit.AddCreditOpenTransaction(client, settlementDate);
                    }

                    // если кредит есть
                    if (client.ClientCredit.CreditAmount > 0 && client.ClientCredit.CreditStartDate < settlementDate)
                    {
                        Credit.LoanRepaymentCalculation(client, settlementDate);
                    }

                    // если депозит есть
                    if (client.ClientDeposit.DepositAmount > 0 && client.ClientDeposit.DepositStartDate < settlementDate)
                    {
                        Deposit.AccrualOfInterestOnDeposit(client, settlementDate);
                    }

                    settlementDate = settlementDate.AddDays(1);
                }
            }

            foreach (var client in test_VIP_Individuals)
            {
                if (client.ClientCredit.CreditAmount == 0 && client.ClientDeposit.DepositAmount == 0)
                {
                    continue;
                }

                DateTime settlementDate = client.ClientAccount.ContractDate;

                while (settlementDate <= DateTime.Now && (client.ClientCredit.CreditAmount > 0 || client.ClientDeposit.DepositAmount > 0))
                {
                    // добавление записи об открытии депозита
                    if (settlementDate.Date == client.ClientDeposit.DepositStartDate.Date)
                    {
                        Deposit.AddDepositOpenTransactoin(client, settlementDate);
                    }

                    // добавление записи об открытии кредита
                    if (settlementDate.Date == client.ClientCredit.CreditStartDate.Date)
                    {
                        Credit.AddCreditOpenTransaction(client, settlementDate);
                    }

                    // если кредит есть
                    if (client.ClientCredit.CreditAmount > 0 && client.ClientCredit.CreditStartDate < settlementDate)
                    {
                        Credit.LoanRepaymentCalculation(client, settlementDate);
                    }

                    // если депозит есть
                    if (client.ClientDeposit.DepositAmount > 0 && client.ClientDeposit.DepositStartDate < settlementDate)
                    {
                        Deposit.AccrualOfInterestOnDeposit(client, settlementDate);
                    }

                    settlementDate = settlementDate.AddDays(1);
                }
            }

            foreach (var client in testLegalEntities)
            {
                if (client.ClientCredit.CreditAmount <= 0)
                {
                    continue;
                }

                DateTime settlementDate = client.ClientAccount.ContractDate;

                while (settlementDate <= DateTime.Now && client.ClientCredit.CreditAmount > 0)
                {
                    // если расчетная дата и дата открытия кредита совпадают
                    if (settlementDate.Date == client.ClientCredit.CreditStartDate.Date)
                    {
                        Credit.AddCreditOpenTransaction(client, settlementDate);
                    }

                    // если расчетная дата позже даты открытия кредита
                    if (settlementDate > client.ClientCredit.CreditStartDate)
                    {
                        Credit.LoanRepaymentCalculation(client, settlementDate);
                    }
                    settlementDate = settlementDate.AddDays(1);
                }
            }
        }


        /// <summary>
        /// Возвращает случайные фамилию и имя физического лица
        /// </summary>
        /// <returns></returns>
        static string GetRandomName()
        {
            string[] names = new string[]                                    // массив имен
            {
                "Dan", "Kat", "John", "Sobaka", "July","Valentyna","Dimon", "Kot", "Daria", "Nataly", "Zoi", "Mary",
                "Mike", "Sofy", "Bruce"
            };

            string[] sureNames = new string[]                                // массив фамилий
            {
                "Tesla","Portnoy","Yakudza","Sobaken","Smishno","Mioskosi","Vazovski","Dindon","Matroskin", "Lee",
                "Shvarzneiger","Acula","Chubaka"
            };

            return sureNames[random.Next(sureNames.Length)] + " " + names[random.Next(names.Length)];         // возврат случайного имени
        }


        ///// <summary>
        ///// Возвращает случайное название юридического лица
        ///// </summary>
        static string GetRandomEntityName()
        {
            string leters = "QWERTYUIOPASDFGHJKLZXCVBNM";

            int numOfLeters = random.Next(2,5);

            string name = String.Empty;

            for (int i = 0; i < numOfLeters; i++)
            {
                name += leters[random.Next(leters.Length)];
            }

            string[] abbreviation = new string[] { "d.o.o", "Corp.", "Inc.", "Technologis", "Solutions", "Crocodiles", 
                                                   "Tigers", "Groupe", "& Co", "Friends", "Sciences", "Programming", 
                                                   "Tech", "Company", "Software", "engineers", "Connections" };

            return name + $" {abbreviation[random.Next(abbreviation.Length)]}";         // возврат случайного имени
        }

    }
}
