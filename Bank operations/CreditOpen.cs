using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseBankSubstances
{
    class CreditOpen
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="dataContextForMain"></param>
        public CreditOpen(DataContextForMainWindow dataContextForMain)
        {
            GetAllClients(dataContextForMain);

            dataContextForMainWindow = dataContextForMain;

            AllClientsReserv = AllClients;

            _creditOpenPage = new CreditOpenPage();

            _creditOpenPage.DataContext = this;

            dataContextForMain.CurrentPage = _creditOpenPage;
        }

        private DataContextForMainWindow dataContextForMainWindow;

        /// <summary>
        /// Страница для открытия кредита 
        /// </summary>
        private CreditOpenPage _creditOpenPage;

        /// <summary>
        /// Коллекция всех клиентов
        /// </summary>
        public ObservableCollection<Client> AllClients { get; set; } = new ObservableCollection<Client>();

        /// <summary>
        /// Коллекция всех клиентов используемая при фильтрации списка при быстром поиске по подстроке и остающаяся неизменной
        /// </summary>
        public ObservableCollection<Client> AllClientsReserv { get; set; }


        private static Client selectedClient;      // поле выбранный клиент
        /// <summary>
        /// Свойство выбранный клиент
        /// </summary>
        public Client SelectedClient
        {
            get { return selectedClient; }
            set
            {
                if (selectedClient != value)
                {
                    selectedClient = value;
                    if (SelectedClient == null)
                    {
                        return;
                    }

                    _creditOpenPage.tb_ClientName.Text = $"{SelectedClient.Name}";

                    // получение процента по кредиту для данного клиента
                    double creditInterest = Credit.GetCreditInterest(SelectedClient);

                    // отображение процента в UI (если установлен ВИП процент - об этом указывается в окне UI)
                    if (SelectedClient.GetType() == typeof(VIP_Individual))
                    {
                        _creditOpenPage.tb_CreditInterest.Text = string.Format("{0:f1}", creditInterest) + " (VIP)";
                    }
                    else _creditOpenPage.tb_CreditInterest.Text = string.Format("{0:f1}", creditInterest);

                }
            }
        }


        private decimal creditSum;  // переменная для хранения суммы кредита

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


        /// <summary>
        /// Создает список клиентов фильтрацией полного списка по введенной пользователем подстроке быстрого поиска. Поиск подстроки производится по имени клиента и номеру текущего счета
        /// </summary>
        private void QuickSearchingData()
        {
            _creditOpenPage.tb_ClientName.Text = "";
            _creditOpenPage.dg_AllClientsList.ItemsSource = null;

            AllClients = new ObservableCollection<Client>(AllClientsReserv.Where(x => x.Name.ToLowerInvariant().Contains(QuicSearchText.ToLowerInvariant()) 
                                                                                   || x.ClientAccount.AccountNumber.Contains(QuicSearchText.ToLowerInvariant())));
            _creditOpenPage.dg_AllClientsList.ItemsSource = AllClients;
        }


        /// <summary>
        /// Заполняет коллекцию всех клиентов
        /// </summary>
        /// <param name="dataContextForMain"></param>
        private void GetAllClients(DataContextForMainWindow dataContextForMain)
        {
            foreach (var item in dataContextForMain.IndividualsForUI)
            {
                if (item.ClientCredit.CreditAmount <= 0 && item.ClientAccount.Balans >= 0)
                {
                    AllClients.Add(item);
                }
            }
            foreach (var item in dataContextForMain.VIP_IndividualsForUI)
            {
                if (item.ClientCredit.CreditAmount <= 0 && item.ClientAccount.Balans >= 0)
                {
                    AllClients.Add(item);
                }
            }
            foreach (var item in dataContextForMain.LegalEntitiesForUI)
            {
                if (item.ClientCredit.CreditAmount <= 0 && item.ClientAccount.Balans >= 0)
                {
                    AllClients.Add(item);
                }
            }
        }



        public RelayCommand createCreditComand;                                        
        /// <summary>
        /// Команада, которая при нажатии на соответствующую кнопку вызывает алгоритм завершения процедуры открытия кредита
        /// </summary>
        public RelayCommand CreateCreditComand
        {
            get
            {
                return createCreditComand ??
                  (createCreditComand = new RelayCommand(obj =>
                  {
                      // если клиент не выбран
                      if (SelectedClient == null) { MessageBox.Show("Please, select the client!"); return; }

                      // если текущий счет клиента закрыт/заблокирован
                      if (!SelectedClient.ClientAccount.IsOpen) { MessageBox.Show($"{SelectedClient.Name} account is closed. The transaction is impossible"); return; }

                      // если не определен кредитный период
                      if (string.IsNullOrWhiteSpace(_creditOpenPage.cb_CreditPeriod.Text)) { MessageBox.Show("Please, select the period!"); return; }

                      // если не введен номер договора
                      if (string.IsNullOrWhiteSpace(_creditOpenPage.tb_ContractNum.Text)) { MessageBox.Show("Please, enter contract number!"); return; }

                      // если сумма кредита не введена или введена не корректно
                      if (String.IsNullOrWhiteSpace(_creditOpenPage.tb_CreditSum.Text) || !decimal.TryParse(_creditOpenPage.tb_CreditSum.Text, out creditSum))
                      {
                          MessageBox.Show("Please, enter correct sum!");
                          return;
                      }

                      FinishCreditCreation();

                      MessageBox.Show("Credit successfully created");

                      // очистка формы кредитования
                      SelectedClient = null;
                      CreditOpen creditOpen = new CreditOpen(dataContextForMainWindow);

                  }));
            }
        }

        /// <summary>
        /// Завершает процедуру оформления кредита с вызовом метода добавления соотвествующей транзакции в историю транзакций
        /// </summary>
        private void FinishCreditCreation()
        {
            // установление процентов по кредиту
            SelectedClient.ClientCredit.SetCreditInterest(SelectedClient);

            // присвоение суммы кредита
            SelectedClient.ClientCredit.CreditAmount = creditSum;

            //присвоение значения дате открытия кредита
            SelectedClient.ClientCredit.CreditStartDate = DateTime.Now;

            // присвоение значения кредитному периоду
            SelectedClient.ClientCredit.CreditPeriod = int.Parse(_creditOpenPage.cb_CreditPeriod.Text);

            // устанавливаем сумму ежемесячного возврата части основного долга
            SelectedClient.ClientCredit.SetMonthlyInstallment();

            // присвоение номера кредитного счета
            SelectedClient.ClientCredit.CreditAccountNum = Credit.GetAccountNumber();

            // создание и добавление транзакции об открытии кредита
            Credit.AddCreditOpenTransaction(SelectedClient, DateTime.Now);
        }


        public RelayCommand autoFillContractNumComand;                                        
        /// <summary>
        /// Команада, которая при нажатии на соответствующую кнопку автоматически генерирует номер договора
        /// </summary>
        public RelayCommand AutoFillContractNumComand
        {
            get
            {
                return autoFillContractNumComand ??
                  (autoFillContractNumComand = new RelayCommand(obj =>
                  {
                      _creditOpenPage.tb_ContractNum.Text = $"{Guid.NewGuid().ToString().Substring(5, 11)}";
                  }));
            }
        }

    }
}
