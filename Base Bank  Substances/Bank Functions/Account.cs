using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseBankSubstances
{
    public class Account : INotifyPropertyChanged, ICloneable
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
        /// <param name="accountNumber"></param>
        /// <param name="contractNumber"></param>
        /// <param name="contractDate"></param>
        /// <param name="balans"></param>
        /// <param name="isOpen"></param>
        public Account(string accountNumber, string contractNumber, DateTime contractDate, decimal balans, bool isOpen)
        {
            AccountNumber = accountNumber;
            ContractNumber = contractNumber;
            ContractDate = contractDate;
            Balans = balans;
            IsOpen = isOpen;
        }

        /// <summary>
        /// Номер текущего счета
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Номер договора на открытие счета
        /// </summary>
        public string ContractNumber { get; set; }

        /// <summary>
        /// Дата договора на открытие счета
        /// </summary>
        public DateTime ContractDate { get; set; }

        private decimal balans;
        /// <summary>
        /// Баланс текущего счета
        /// </summary>
        public decimal Balans
        {
            get { return balans; }
            private set { balans = value; OnPropertyChanged("Balans"); }
        }

        private bool isOpen;
        /// <summary>
        /// Отражает информацию о том, открыт ли текущий счет в данный момент
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                OnPropertyChanged("IsOpen");
            }
        }

        /// <summary>
        /// Базовый номер текущего счета, выдается вновь зарегистрированному клиенту после его инкрементации
        /// </summary>
        public static long BaseAccountNumber { get; private set; } = 301200000000;

        /// <summary>
        /// Увеличивает баланс текущего счета
        /// </summary>
        /// <param name="addingSum"></param>
        public void AddBalans(decimal addingSum)
        {
            balans += addingSum;
        }

        /// <summary>
        /// Уменьшает баланс текущего счета
        /// </summary>
        /// <param name="reducingSum"></param>
        public void ReduceBalans(decimal reducingSum)
        {
            balans -= reducingSum;
        }

        /// <summary>
        /// Устанавливает баланс текущего счета 
        /// </summary>
        /// <param name="sum"></param>
        public void SetBalans(decimal sum)
        {
            // перед установлением нового баланса текущего счета он должен быть равен 0. Все имеющиеся средства или задолженности должны быть переведены на другие счета.
            if (balans != 0)
            {
                MessageBox.Show($"Balans must be equal to zero before a new current account balance can be established. " +
                    $"Currently, the balance is equal to {balans}. " +
                    $"Perform the necessary operations.");
                return;
            }

            balans = sum;
        }

        /// <summary>
        /// Возвращает новый номер текущего счета для регистрируемого клиента
        /// </summary>
        /// <returns></returns>
        public static string GetAccountNumber()
        {
            return (++BaseAccountNumber).ToString(); 
        }

        /// <summary>
        /// Добавляет в историю транзакций запись об открытии текущего счета
        /// </summary>
        /// <param name="client"></param>
        public static void AddTransactionInfoAccountOpen(Client client)
        {
            client.TransactionHistory.Add(new TransactionInfo
            (
                client,
                client.ClientAccount.Balans,
                client.RegistrationDate,
                client.ClientAccount.Balans,
                0,
                "Current account opening",
                "external source",
                client.ClientAccount.AccountNumber
            ));

            AddNewTransaction(client.TransactionHistory[client.TransactionHistory.Count - 1]);
        }


        /// <summary>
        /// Реализация интерфейса ICloneable
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Account account = (Account)MemberwiseClone();
            return account;
        }

        /// <summary>
        /// Происходит при изменении привязанного свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Оповещает об изменении привязанных свойств
        /// </summary>  
        /// <param name="prop"></param>
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
