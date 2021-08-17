using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances
{
    public abstract class Client : INotifyPropertyChanged, ICloneable
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="registrationDate"></param>
        /// <param name="name"></param>
        /// <param name="iD"></param>
        /// <param name="clientAddress"></param>
        /// <param name="clientAccount"></param>
        /// <param name="clientCredit"></param>
        /// <param name="transactionHistory"></param>
        /// <param name="isSelected"></param>
        protected Client(DateTime registrationDate, string name, string iD, Address clientAddress, Account clientAccount, List<TransactionInfo> transactionHistory, bool isSelected)
        {
            RegistrationDate = registrationDate;
            Name = name;
            ID = iD;
            ClientAddress = clientAddress;
            ClientAccount = clientAccount;
            TransactionHistory = transactionHistory;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        protected Client()
        {

        }

        /// <summary>
        /// Дата регистрации клиента
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        private string name;
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string Name
        {
            get { return name; }
            set 
            { 
                name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// ID клиента
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Адрес клиента
        /// </summary>
        public Address ClientAddress { get; set; }

        /// <summary>
        /// Текущий счет клиента
        /// </summary>
        public Account ClientAccount { get; set; }

        /// <summary>
        /// Кредит клиента
        /// </summary>
        public Credit ClientCredit { get; set; }

        /// <summary>
        /// Список транзакций клиента
        /// </summary>
        public List<TransactionInfo> TransactionHistory { get; set; }

        private bool _isSelected;
        /// <summary>
        /// Свойство (логическая переменная) обозначающее, выбран ли данный департамент в тривью
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    if (_isSelected)
                    {
                        SelectedClientEvent?.Invoke(this); // привязка выбранного элемента к источнику данных для заполнения списка работников выбранного отдела
                    }
                }
            }
        }

        /// <summary>
        /// Происходит при смене/выборе клиента в UI
        /// </summary>
        public static event Action<Client> SelectedClientEvent;

        /// <summary>
        /// Реализация интерфейса ICloneable
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Client client = (Client)MemberwiseClone();

            client.ClientAddress = (Address)ClientAddress.Clone();

            client.ClientAccount = (Account)ClientAccount.Clone();

            client.ClientCredit = (Credit)ClientCredit.Clone();

            return client;
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
