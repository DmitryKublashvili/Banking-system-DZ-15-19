using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances
{
    public class TransactionInfo
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="date"></param>
        /// <param name="incomingSum"></param>
        /// <param name="outflowSum"></param>
        /// <param name="balans"></param>
        /// <param name="deposit"></param>
        /// <param name="credit"></param>
        /// <param name="type"></param>
        /// <param name="senderAccountNum"></param>
        /// <param name="reciverAccountNum"></param>
        public TransactionInfo(Client client, decimal transactionAmount, DateTime date, decimal incomingSum, decimal outflowSum, 
                               string type, string senderAccountNum, string reciverAccountNum)
        {
            ClientName = client.Name;
            TransactionAmount = transactionAmount;
            Date = date;
            IncomingSum = incomingSum;
            OutflowSum = outflowSum;
            Balans = client.ClientAccount.Balans;
            Deposit = client is Individual ? (client as Individual).ClientDeposit.DepositAmount : 0;
            Credit = client.ClientCredit.CreditAmount;
            Type = type;
            SenderAccountNum = senderAccountNum;
            ReciverAccountNum = reciverAccountNum;
        }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName { get; private set; }

        /// <summary>
        /// сумма транзакции
        /// </summary>
        public decimal TransactionAmount { get; private set; }

        /// <summary>
        /// Дата транзакции
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма поступивших средств
        /// </summary>
        public decimal IncomingSum { get; set; }

        /// <summary>
        /// Сумма выбывших средств
        /// </summary>
        public decimal OutflowSum { get; set; }

        /// <summary>
        /// Баланс текущего счета
        /// </summary>
        public decimal Balans { get; set; }

        /// <summary>
        /// Сумма депозита
        /// </summary>
        public decimal Deposit { get; set; }

        /// <summary>
        /// Сумма кредита
        /// </summary>
        public decimal Credit { get; set; }

        /// <summary>
        /// Тип банковской операции или ее описание
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Отправитель денежных средств
        /// </summary>
        public string SenderAccountNum { get; set; }

        /// <summary>
        /// Получатель денежных средств
        /// </summary>
        public string ReciverAccountNum { get; set; }
    }
}
