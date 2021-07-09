using DZ_13_Banking_system;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BaseBankSubstances
{
    public class MonyTransfer : INotifyPropertyChanged
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
        /// <param name="dataContextForMain"></param>
        public MonyTransfer(DataContextForMainWindow dataContextForMain)
        {
            AllClients = new ObservableCollection<Client>();

            GetAllClients(dataContextForMain);

            DataContextForMain = dataContextForMain;

            AllClientsReserv = AllClients;

            DataContextForMain.moneyTransferPage = new MoneyTransferPage();

            MoneyTransferPage = DataContextForMain.moneyTransferPage;

            DataContextForMain.CurrentPage = MoneyTransferPage;

            MoneyTransferPage.DataContext = this;
        }


        /// <summary>
        /// Страница UI отвечающая за осуществление денежных переводов
        /// </summary>
        private static MoneyTransferPage MoneyTransferPage;

        /// <summary>
        /// Экземпляр класса выступающего контекстом для главного окна
        /// </summary>
        public static DataContextForMainWindow DataContextForMain { get; set; }

        /// <summary>
        /// Коллекция всех клиентов с незаблокированными счетами
        /// </summary>
        public ObservableCollection<Client> AllClients { get; set; }

        /// <summary>
        /// Коллекция всех клиентов используемая при фильтрации списка при быстром поиске по подстроке и остающаяся неизменной
        /// </summary>
        public ObservableCollection<Client> AllClientsReserv { get; set; }

        /// <summary>
        /// Отправитель перевода
        /// </summary>
        public static Client Sender { get; set; }

        /// <summary>
        /// Получатель перевода
        /// </summary>
        public static Client Reciver { get; set; }

        /// <summary>
        /// Сумма перевода
        /// </summary>
        public decimal transferSum = 0;

        private string quicSearchText;
        /// <summary>
        /// Текст (подстрока), вводимый пользователем для быстрого поиска клиента в таблице UI
        /// </summary>
        public string QuicSearchText
        {
            get { return quicSearchText; }
            set
            {
                if (quicSearchText != value)
                {
                    quicSearchText = value;
                    QuickSearchingData();
                }
            }
        }

        private static Client selectedClient;
        /// <summary>
        /// Свойство выбранный клиент
        /// </summary>
        public static Client SelectedClient
        {
            get { return selectedClient; }
            set
            {
                if (selectedClient != value)
                {
                    selectedClient = value;
                    SetSenderOrReciver();
                }
            }
        }


        /// <summary>
        /// Создает список клиентов фильтрацией полного списка по введенной пользователем подстроке быстрого поиска. Поиск подстроки производится по имени клиента и номеру текущего счета
        /// </summary>
        private void QuickSearchingData()
        {
            MoneyTransferPage.dg_AllClientsList.ItemsSource = null;

            AllClients = new ObservableCollection<Client>(AllClientsReserv
                .Where(x => x.Name.ToLowerInvariant().Contains(QuicSearchText.ToLowerInvariant())
                || x.ClientAccount.AccountNumber.Contains(QuicSearchText.ToLowerInvariant())));

            MoneyTransferPage.dg_AllClientsList.ItemsSource = AllClients;
        }


        /// <summary>
        /// Устанавливает отправителя или получателя перевода и производит соответствующие изменения в UI 
        /// </summary>
        private static void SetSenderOrReciver()
        {
            if (SelectedClient==null)
            {
                return;
            }

            if ((bool)MoneyTransferPage.rb_Sender.IsChecked)
            {
                MoneyTransferPage.tb_MonySender.Text = SelectedClient.Name;
                MoneyTransferPage.tb_MonySender.Foreground = Brushes.Black;
                Sender = SelectedClient;
            }
            else
            {
                MoneyTransferPage.tb_MonyReciver.Text = SelectedClient.Name;
                MoneyTransferPage.tb_MonyReciver.Foreground = Brushes.Black;
                Reciver = SelectedClient;
            }
        }


        /// <summary>
        /// Наполняет коллекцию всех клиентов с открытыми счетами (для клиентов с закрытыми счетами перевод средств не доступен)
        /// </summary>
        /// <param name="dataContextForMain"></param>
        private void GetAllClients(DataContextForMainWindow dataContextForMain)
        {
            foreach (var item in dataContextForMain.IndividualsForUI)
            {
                if (item.ClientAccount.IsOpen)
                {
                    AllClients.Add(item);
                }
            }

            foreach (var item in dataContextForMain.VIP_IndividualsForUI)
            {
                if (item.ClientAccount.IsOpen)
                {
                    AllClients.Add(item);
                }
            }

            foreach (var item in dataContextForMain.LegalEntitiesForUI)
            {
                if (item.ClientAccount.IsOpen)
                {
                    AllClients.Add(item);
                }
            }
        }


        public RelayCommand sendMonyComand;                                        
        /// <summary>
        /// Команада, запускающая алгоритм отправления денежного перевода
        /// </summary>
        public RelayCommand SendMonyComand
        {
            get
            {
                return sendMonyComand ??
                  (sendMonyComand = new RelayCommand(obj =>
                  {
                      if (Check())
                      {
                          SendMonyAndClearForm(transferSum);
                      }
                      else return;

                  }));
            }
        }


        /// <summary>
        /// Проверяет корректность заполнения формы
        /// </summary>
        /// <returns></returns>
        private bool Check()
        {

            if (Sender == null) { MessageBox.Show("Please, select the sender!"); return false; }

            if (Reciver == null) { MessageBox.Show("Please, select the reciver!"); return false; }

            if (Sender == Reciver) { MessageBox.Show("A transfer within one account is not possible!"); return false; }

            if (!Sender.ClientAccount.IsOpen) { MessageBox.Show($"{Sender.Name} account is closed. The transaction is impossible"); return false; }

            if (!Reciver.ClientAccount.IsOpen) { MessageBox.Show($"{Reciver.Name} account is closed. The transaction is impossible"); return false; }

            if (String.IsNullOrWhiteSpace(MoneyTransferPage.tb_amountTransferMony.Text) || !decimal.TryParse(MoneyTransferPage.tb_amountTransferMony.Text, out transferSum))
            {
                MessageBox.Show("Please, enter correct sum to transfer!");
                return false;
            }

            try
            {
                if (transferSum > Sender.ClientAccount.Balans)
                {
                    throw new NotEnoughMoneyException($"Insufficient funds in the account. \nThe amount of the current sender's account is {Sender.ClientAccount.Balans}.\nTransfer inpossible.");
                }
            }
            catch (NotEnoughMoneyException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            if (MessageBox.Show($"Confirm the transfer of funds:\nSender: {Sender.Name} \n" +
                $"Reciver: {Reciver.Name} \nSum: {transferSum} ", "Question", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Отправляет денежный перевод с оформлением записей в историях транзакций
        /// </summary>
        /// <param name="sum"></param>
        private void SendMonyAndClearForm(decimal sum)
        {
            Sender.ClientAccount.ReduceBalans(sum);
            Reciver.ClientAccount.AddBalans(sum);
            Sender.TransactionHistory.Add(new TransactionInfo
            (
                Sender,
                sum,
                DateTime.Now,
                0,
                sum,
                $"Money transfer \nfrom {Sender.Name} \nto {Reciver.Name}",
                Sender.ClientAccount.AccountNumber,
                Reciver.ClientAccount.AccountNumber
            ));

            AddNewTransaction(Sender.TransactionHistory[Sender.TransactionHistory.Count - 1]);

            Reciver.TransactionHistory.Add(new TransactionInfo
            (
                Reciver,
                sum,
                DateTime.Now,
                sum,
                0,
                $"Money transfer from \n{Sender.Name}",
                Sender.ClientAccount.AccountNumber,
                Reciver.ClientAccount.AccountNumber
            ));

            MessageBox.Show("The transfer was successful");

            SelectedClient = null;
            MonyTransfer monyTransfer = new MonyTransfer(DataContextForMain);
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
