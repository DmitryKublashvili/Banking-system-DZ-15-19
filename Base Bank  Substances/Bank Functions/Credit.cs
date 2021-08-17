using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances
{
    public class Credit : ICloneable
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
        /// Конструктор класса
        /// </summary>
        /// <param name="creditAmount"></param>
        /// <param name="monthlyInstallment"></param>
        /// <param name="creditAccountNum"></param>
        /// <param name="creditStartDate"></param>
        /// <param name="creditPeriod"></param>
        /// <param name="creditContractNumber"></param>
        /// <param name="creditInterest"></param>
        public Credit(decimal creditAmount, decimal monthlyInstallment, string creditAccountNum, DateTime creditStartDate, 
            int creditPeriod, string creditContractNumber, double creditInterest)
        {
            CreditAmount = creditAmount;
            this.monthlyInstallment = monthlyInstallment;
            CreditAccountNum = creditAccountNum;
            CreditStartDate = creditStartDate;
            CreditPeriod = creditPeriod;
            CreditContractNumber = creditContractNumber;
            CreditInterest = creditInterest;
        }

        /// <summary>
        /// Конструтор по умолчанию
        /// </summary>
        public Credit()
        {

        }

        /// <summary>
        /// стандартная процентная ставка по кредиту
        /// </summary>
        private const double standartCreditInterest = 10;

        /// <summary>
        /// ВИП коэффициент, понижающий стандартную процентную ставку для ВИП
        /// </summary>
        private const double VIPcoefficient = 1.2;

        /// <summary>
        /// Сумма кредита
        /// </summary>
        public decimal CreditAmount { get; set; }

        /// <summary>
        /// Сумма ежемесячного возврата кредита
        /// </summary>
        public decimal monthlyInstallment { get; set; }

        /// <summary>
        /// Номер кредитного счета
        /// </summary>
        public string CreditAccountNum { get; set; }

        /// <summary>
        /// Дата открытия кредита
        /// </summary>
        public DateTime CreditStartDate { get; set; }

        /// <summary>
        /// Период кредитования
        /// </summary>
        public int CreditPeriod { get; set; }

        /// <summary>
        /// Номер кредитного договора
        /// </summary>
        public string CreditContractNumber { get; set; }

        private double creditInterest;
        /// <summary>
        /// Кредитная процентная ставка
        /// </summary>
        public double CreditInterest
        {
            get { return creditInterest; }
            private set { creditInterest = value; }
        }

        /// <summary>
        /// Базовый номер кредитного счета, выдается клиенту при оформлении кредита
        /// </summary>
        public static long BaseAccountNumber { get; private set; } = 901200000000;

        /// <summary>
        /// Устанавливает сумму ежемесячного возврата части основного долга
        /// </summary>
        public void SetMonthlyInstallment()
        {
            monthlyInstallment = CreditAmount / CreditPeriod;
        }

        /// <summary>
        /// Устанавливает кредитную процентную ставку
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        public void SetCreditInterest<T>(T client)
                                where T : Client
        {
            CreditInterest = client.GetType() == typeof(VIP_Individual) ? standartCreditInterest / VIPcoefficient : standartCreditInterest;
        }


        /// <summary>
        /// Возвращает кредитную процентную ставку для данного типа клиента 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        public static double GetCreditInterest<T>(T client)
                                where T : Client
        {
            return client.GetType() == typeof(VIP_Individual) ? standartCreditInterest / VIPcoefficient : standartCreditInterest;
        }


        /// <summary>
        /// Добавляет запись об откртии кредита в историю транзакций
        /// </summary>
        public static void AddCreditOpenTransaction(Client client, DateTime currentDate)
        {
            string creditInterest = string.Format("{0:f1}", client.ClientCredit.CreditInterest);

            client.TransactionHistory.Add(new TransactionInfo
            (
                client,
                client.ClientCredit.CreditAmount,
                currentDate,
                client.ClientCredit.CreditAmount,
                0,
                $"Crediting the amount {client.ClientCredit.CreditAmount}\nto the credit account {client.ClientCredit.CreditAccountNum}\n" +
                $"on {client.ClientCredit.CreditPeriod} months, \nof interest {creditInterest}%",
                "bank",
                client.ClientCredit.CreditAccountNum
            ));

            AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);
        }


        /// <summary>
        /// Проверяет необходимость проведения кредитных операций для данного клиента на данную дату и при необходимости вызывает соответствующие методы
        /// </summary>
        /// <param name="client"></param>
        public static void LoanRepaymentCalculation(Client client, DateTime settlementDate)
        {
            // дата окончания кредитного периода клиента 
            DateTime finalCreditDate = client.ClientCredit.CreditStartDate.AddMonths(client.ClientCredit.CreditPeriod);

            //  если кредитный период закончился то производится последний платеж и сумма кредита списывается с текущего счета клиента
            if (settlementDate.Date >= finalCreditDate.Date)
            {
                MakingTheLoanPaymentOnTheSettlementDate(client, settlementDate);
                CreditClosing(client, settlementDate);
                return;
            }

            // когда число даты открытия депозита и число текущего месяца совпадают
            if (client.ClientCredit.CreditStartDate.Day == settlementDate.Day)
            {
                MakingTheLoanPaymentOnTheSettlementDate(client, settlementDate);
                settlementDate = settlementDate.AddDays(1);
                return;
            }

            // если дата выдачи кредита больше чем количество дней в месяце то платеж оформляется в последний день месяца 
            if (client.ClientCredit.CreditStartDate.Day > DateTime.DaysInMonth(settlementDate.Year, settlementDate.Month)
                && settlementDate.Day == DateTime.DaysInMonth(settlementDate.Year, settlementDate.Month))
            {
                MakingTheLoanPaymentOnTheSettlementDate(client, settlementDate);
                settlementDate = settlementDate.AddDays(1);
                return;
            }
        }

        /// <summary>
        /// Осуществляет платеж по кредиту с добавлением записи в историю транзакций
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settlementDate"></param>
        private static void MakingTheLoanPaymentOnTheSettlementDate(Client client, DateTime settlementDate)
        {
            // сумма начисленных процентов
            decimal sumOfInterest = client.ClientCredit.CreditAmount * (decimal)client.ClientCredit.CreditInterest / 12 / 100;

            // частичный возврат кредита
            decimal partOfTheCredit = client.ClientCredit.monthlyInstallment;

            // общая сумма платежа
            decimal loanPayment = sumOfInterest + partOfTheCredit;

            // списание суммы процентов с баланса текущего счета клиента
            client.ClientAccount.ReduceBalans(loanPayment);

            // уменьшение размера кредита на сумму его возврата
            client.ClientCredit.CreditAmount -= partOfTheCredit;

            string creditInterest = string.Format("{0:f1}", client.ClientCredit.CreditInterest);

            client.TransactionHistory.Add(new TransactionInfo
            (
                client,
                loanPayment,
                settlementDate,
                0,
                loanPayment,
                $"Withholding of interest {creditInterest}%\n on the loan and part of the credit",
                client.ClientAccount.AccountNumber,
                "bank"
            ));

            AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);
        }

        /// <summary>
        /// Закрывает кредит с добавлением записи в историю транзакций
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settlementDate"></param>
        private static void CreditClosing(Client client, DateTime settlementDate)
        {
            client.ClientAccount.ReduceBalans(client.ClientCredit.CreditAmount);  // списание остатка кредита с текущего счета клиента

            client.TransactionHistory.Add(new TransactionInfo     // добавление записи в историю транзакций
            (
                client,
                0,
                settlementDate,
                0,
                client.ClientCredit.CreditAmount,
                "Closing the credit",
                client.ClientAccount.AccountNumber,
                "bank"
            ));

            AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);

            // обнуление данных по кредиту для данного клиента
            client.ClientCredit = new Credit();
        }


        /// <summary>
        /// Возвращает новый номер кредитного счета при оформлении кредита
        /// </summary>
        /// <returns></returns>
        public static string GetAccountNumber()
        {
            return (++BaseAccountNumber).ToString();
        }


        /// <summary>
        /// Реализация интерфейса ICloneable
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Credit credit = (Credit)MemberwiseClone();
            return credit;
        }
    }
}
