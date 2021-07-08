using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DZ_13_Banking_system
{
    class DepositOpen
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="dataContextForMain"></param>
        public DepositOpen(DataContextForMainWindow dataContextForMain)
        {
            AllIndividualClients = new ObservableCollection<Client>();

            GetAllIndividualClients(dataContextForMain);

            DataContextForMain = dataContextForMain;

            AllIndividualClientsReserv = AllIndividualClients;

            window = new DepositOpenWindow();

            window.DataContext = this;

            window.Show();
        }

        private DepositOpenWindow window;

        private DataContextForMainWindow DataContextForMain;

        private decimal DepositSum { get; set; }

        public ObservableCollection<Client> AllIndividualClients { get; set; } 

        private ObservableCollection<Client> AllIndividualClientsReserv { get; set; }

        private static Client selectedClient;        // поле выбранный клиент
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
                        window.tb_ClientName.Text = "";
                        return;
                    }

                    window.tb_ClientName.Text = $" {SelectedClient.Name}";

                    double depositInterest = Deposit.GetDepositInterest(SelectedClient as Individual);

                    if (SelectedClient.GetType() == typeof(VIP_Individual))
                    {
                       // (SelectedClient as VIP_Individual).ClientDeposit.SetDepositInterest(SelectedClient as VIP_Individual);

                        window.tb_DepInterest.Text = string.Format("{0:f1}", depositInterest + " (VIP)");
                    }
                    else if (SelectedClient.GetType() == typeof(Individual))
                    {
                       // (SelectedClient as Individual).ClientDeposit.SetDepositInterest(SelectedClient as Individual);

                        window.tb_DepInterest.Text = string.Format("{0:f1}", depositInterest);
                    }
                    else
                    {
                        MessageBox.Show("Opening a deposit for clients of this type is not provided");
                        return;
                    }
                }
            }
        }


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
        /// Создает список клиентов фильтрацией полного списка физических лиц по введенной пользователем подстроке быстрого поиска. Поиск подстроки производится по имени клиента и номеру текущего счета
        /// </summary>
        private void QuickSearchingData()
        {
            SelectedClient = null;
            window.dg_AllClientsList.ItemsSource = null;
            AllIndividualClients = new ObservableCollection<Client>(AllIndividualClientsReserv.Where(x => x.Name.ToLowerInvariant().Contains(QuicSearchText.ToLowerInvariant())
                                                                                   || x.ClientAccount.AccountNumber.Contains(QuicSearchText.ToLowerInvariant())));

            window.dg_AllClientsList.ItemsSource = AllIndividualClients;
        }


        /// <summary>
        /// Наполняет коллекцию всех физических лиц
        /// </summary>
        /// <param name="dataContextForMain"></param>
        private void GetAllIndividualClients(DataContextForMainWindow dataContextForMain)
        {
            foreach (var item in dataContextForMain.IndividualsForUI)
            {
                if (item.ClientAccount.IsOpen)
                {
                    AllIndividualClients.Add(item);
                }
            }

            foreach (var item in dataContextForMain.VIP_IndividualsForUI)
            {
                if (item.ClientAccount.IsOpen)
                {
                    AllIndividualClients.Add(item);
                }
            }
        }


        public RelayCommand createDepositComand;        // команда, запускающая алгоритм открытия депозита
        /// <summary>
        /// Команада, запускающая алгоритм открытия депозита
        /// </summary>
        public RelayCommand CreateDepositComand
        {
            get
            {
                return createDepositComand ??
                  (createDepositComand = new RelayCommand(obj =>
                  {
                      if (SelectedClient == null) { MessageBox.Show("Please, select the client!"); return; }

                      if (!SelectedClient.ClientAccount.IsOpen) { MessageBox.Show($"{SelectedClient.Name} account is closed. The transaction is impossible"); return; }

                      if (string.IsNullOrWhiteSpace(window.cb_DepositPeriod.Text)) { MessageBox.Show("Please, select the period!"); return; }

                      if (string.IsNullOrWhiteSpace(window.tb_ContractNum.Text)) { MessageBox.Show("Please, enter contract number!"); return; }

                      decimal sum;   // сумма депозита

                      // проверка наличия введенной суммы и ее корректности
                      if (String.IsNullOrWhiteSpace(window.tb_DepSum.Text) || !decimal.TryParse(window.tb_DepSum.Text, out sum))
                      {
                          MessageBox.Show("Please, enter correct sum!");
                          return;
                      }
                      else
                      {
                          DepositSum = sum;   // присвоение свойству значения суммы депозита

                          // если депозит уже есть, то предлагается его пополнить
                          if ((SelectedClient as Individual).ClientDeposit.DepositAmount >0)
                          {
                              if (MessageBox.Show($"The deposit for this client is already open. Do you wish to add the specified amount to the existing deposit?\n", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                              {
                                  (SelectedClient as Individual).ClientDeposit.DepositAmount += DepositSum;
                                  DepositSum = (SelectedClient as Individual).ClientDeposit.DepositAmount;
                                  FinishDepositCreation();
                              }
                              else return;
                          }
                          else
                          {
                              FinishDepositCreation();
                          }
                          MessageBox.Show("Deposit successfully created");
                      }
                  }));
            }
        }


        /// <summary>
        /// Завершает открытие депозита с добавлением записи в историю транзакций
        /// </summary>
        private void FinishDepositCreation()
        {
            (SelectedClient as Individual).ClientDeposit.SetDepositInterest(SelectedClient as Individual);
            (SelectedClient as Individual).ClientDeposit.DepositAmount = DepositSum;
            (SelectedClient as Individual).ClientDeposit.DepositStartDate = DateTime.Now;
            (SelectedClient as Individual).ClientDeposit.DepositPeriod = int.Parse(window.cb_DepositPeriod.Text);
            (SelectedClient as Individual).ClientDeposit.IsCapitalised = (bool)window.chb_Capitalised.IsChecked;
            (SelectedClient as Individual).ClientDeposit.DepositAccountNum = Deposit.GetAccountNumber();

            Deposit.AddDepositOpenTransactoin(SelectedClient as Individual, DateTime.Now);

            window.Close();
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
                      window.tb_ContractNum.Text = $"{Guid.NewGuid().ToString().Substring(5, 11)}";
                  }));
            }
        }



    }
}
