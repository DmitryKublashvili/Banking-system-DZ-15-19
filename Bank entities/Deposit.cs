using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DZ_13_Banking_system
{
    class Deposit : ICloneable
    {
        /// <summary>
        /// Добавление новой транзакции в главный операционный журнал 
        /// </summary>
        /// <param name="transactionInfo"></param>
        public delegate void AddTransaction(TransactionInfo transactionInfo);

        /// <summary>
        /// Происходит при добавлении новой транзакции 
        /// </summary>
        public static event AddTransaction AddNewTransaction;


        /// <summary>
        /// Конструтор класса
        /// </summary>
        /// <param name="isCapitalised"></param>
        /// <param name="depositAmount"></param>
        /// <param name="depositAccountNum"></param>
        /// <param name="depositStartDate"></param>
        /// <param name="depositPeriod"></param>
        /// <param name="depositContractNumber"></param>
        /// <param name="depositInterest"></param>
        public Deposit(bool isCapitalised, decimal depositAmount, string depositAccountNum, DateTime depositStartDate,
                        int depositPeriod, string depositContractNumber, double depositInterest)
        {
            IsCapitalised = isCapitalised;
            DepositAmount = depositAmount;
            DepositAccountNum = depositAccountNum;
            DepositStartDate = depositStartDate;
            DepositPeriod = depositPeriod;
            DepositContractNumber = depositContractNumber;
            DepositInterest = depositInterest;
        }

        /// <summary>
        /// Конструтор по умолчанию
        /// </summary>
        public Deposit()
        {

        }

        // базовая процентная ставка по депозиту
        private const double standartDepositInterest = 5;

        // повышающий коэффициент для ВИП клиентов
        private const double VIPcoefficient = 1.2;

        /// <summary>
        /// Индикатор наличия капитализации процентов
        /// </summary>
        public bool IsCapitalised { get; set; } = false;

        /// <summary>
        /// Сумма депозита
        /// </summary>
        public decimal DepositAmount { get; set; }

        /// <summary>
        /// Номер депозитного счета
        /// </summary>
        public string DepositAccountNum { get; set; }

        /// <summary>
        /// Дата открытия депозита
        /// </summary>
        public DateTime DepositStartDate { get; set; }

        /// <summary>
        /// Период депозита в месяцах
        /// </summary>
        public int DepositPeriod { get; set; }

        /// <summary>
        /// Номер депозитного договора
        /// </summary>
        public string DepositContractNumber { get; set; }

        private double depositInterest;
        /// <summary>
        /// Процентня ставка по депозиту
        /// </summary>
        public double DepositInterest
        {
            get { return depositInterest; }
            private set { depositInterest = value; }
        }

        /// <summary>
        /// Базовый номер депозитного счета, выдается клиенту при оформлении депозита
        /// </summary>
        public static long BaseAccountNumber { get; private set; } = 701200000000;

        /// <summary>
        /// Возвращает новый номер депозитного счета при оформлении депозита
        /// </summary>
        /// <returns></returns>
        public static string GetAccountNumber()
        {
            return (++BaseAccountNumber).ToString();
        }


        /// <summary>
        /// Устанавливает процентную ставку по депозиту в зависимости от типа клиента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        public virtual void SetDepositInterest<T>(T client)
            where T : Individual 
        {
            DepositInterest = client.GetType() == typeof(VIP_Individual) ? standartDepositInterest * VIPcoefficient : standartDepositInterest;
        }

        /// <summary>
        /// Возвращает процентную ставку по депозиту для клиента данного типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        public static double GetDepositInterest<T>(T client)
            where T : Individual
        {
            return client.GetType() == typeof(VIP_Individual) ? standartDepositInterest * VIPcoefficient : standartDepositInterest;
        }


        /// <summary>
        /// Добавлят запись об открытии депозита в историю транзакций
        /// </summary>
        public static void AddDepositOpenTransactoin(Individual client, DateTime currentDate)
        {
            string depositInterest = string.Format("{0:f1}", client.ClientDeposit.DepositInterest);

            client.TransactionHistory.Add(new TransactionInfo
            (
                client,
                client.ClientDeposit.DepositAmount,
                currentDate,
                0,
                client.ClientDeposit.DepositAmount,
                $"Crediting the amount {client.ClientDeposit.DepositAmount} \nto the deposit account {client.ClientDeposit.DepositAccountNum}\n" +
                $"on {client.ClientDeposit.DepositPeriod} months, \n of interest {depositInterest}%",
                "other sources",
                client.ClientDeposit.DepositAccountNum
            ));

            AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);
        }


        /// <summary>
        /// Проверяет необходимость проведения операций по депозиту на данную дату для данного клиента и при необходимости вызывает соовтетствующие методы
        /// </summary>
        /// <param name="client"></param>
        public static void AccrualOfInterestOnDeposit(Individual client, DateTime settlementDate)
        {
            // дата окончания депозитного периода клиента 
            DateTime finalDepositDate = client.ClientDeposit.DepositStartDate.AddMonths(client.ClientDeposit.DepositPeriod);

            if (settlementDate >= finalDepositDate) //  если депозитный период закончился то производится последний платеж и сумма депозита переводится на текущий счет клиента
            {
                EnrollmentOfDepositInterest(client, settlementDate);
                DepositClosing(client, settlementDate);
                return;
            }

            // когда число даты открытия депозита и число текущего месяца совпадают
            if (client.ClientDeposit.DepositStartDate.Day == settlementDate.Day)
            {
                EnrollmentOfDepositInterest(client, settlementDate);
                settlementDate = settlementDate.AddDays(1);
                return;
            }

            // если дата оформления депозита больше чем количество дней в месяце то платеж оформляется в последний день месяца 
            if (client.ClientDeposit.DepositStartDate.Day > DateTime.DaysInMonth(settlementDate.Year, settlementDate.Month)
                && settlementDate.Day == DateTime.DaysInMonth(settlementDate.Year, settlementDate.Month))
            {
                EnrollmentOfDepositInterest(client, settlementDate);
                settlementDate = settlementDate.AddDays(1);
                return;
            }
        }

        /// <summary>
        /// Начисляет проценты по дипозиту на конкретную дату с добавлением записи в историю транзакций
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settlementDate"></param>
        public static void EnrollmentOfDepositInterest(Individual client, DateTime settlementDate)
        {
            // ежемесячно начисляемая сумма 
            decimal sum = client.ClientDeposit.DepositAmount * (decimal)client.ClientDeposit.DepositInterest / 12 / 100;

            string depositInterest = string.Format("{0:f1}", client.ClientDeposit.DepositInterest);

            // если депозит без капитализации
            if (!client.ClientDeposit.IsCapitalised)
            {
                client.ClientAccount.AddBalans(sum);   // зачисление суммы процентов на текущий счет клиента

                client.TransactionHistory.Add(new TransactionInfo
                (
                    client,
                    sum,
                    settlementDate,
                    sum,
                    0,
                    $"Accrual of interest {depositInterest}% \non the deposit",
                    "bank",
                    client.ClientAccount.AccountNumber
                ));

                AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);
            }
            else                 // в противном случае ежемесячное начисление для депозита без капитализации
            {
                client.ClientDeposit.DepositAmount += sum;

                client.TransactionHistory.Add(new TransactionInfo
                (
                    client,
                    sum,
                    settlementDate,
                    sum,
                    0,
                    $"Accrual of interest {depositInterest}% \non the capitalised deposit",
                    "bank",
                    client.ClientAccount.AccountNumber
                ));

                AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);
            }
        }


        /// <summary>
        /// Закрывает депозит с добавлением записи в историю транзакций
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settlementDate"></param>
        public static void DepositClosing(Individual client, DateTime settlementDate)
        {
            client.ClientAccount.AddBalans(client.ClientDeposit.DepositAmount);  // перевод суммы депозита на текущий счет клиента

            client.TransactionHistory.Add(new TransactionInfo     // добавление записи в историю транзакций
            (
                client,
                client.ClientDeposit.DepositAmount,
                settlementDate,
                client.ClientDeposit.DepositAmount,
                0,
                "Closing the deposit",
                "bank",
                client.ClientAccount.AccountNumber
            ));

            AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);

            // обнуление данных по депозиту для данного клиента
            client.ClientDeposit = new Deposit();
        }

        /// <summary>
        /// Реализация интерфейса ICloneable
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Deposit deposit = (Deposit)MemberwiseClone();
            return deposit;
        }
    }
}
