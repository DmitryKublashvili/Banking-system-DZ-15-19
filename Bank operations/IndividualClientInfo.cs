using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaseBankSubstances
{
    public class IndividualClientInfo : INotifyPropertyChanged
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
        /// <param name="client"></param>
        /// <param name="IndividualsForUI"></param>
        /// <param name="VIP_IndividualsForUI"></param>
        public IndividualClientInfo(Client client, ObservableCollection<Individual> IndividualsForUI, ObservableCollection<VIP_Individual> VIP_IndividualsForUI)
        {
            SelectedClient = client;

            this.VIP_IndividualsForUI = VIP_IndividualsForUI;

            this.IndividualsForUI = IndividualsForUI;

            Window = new IndividualClientInfo_Window();

            Window.DataContext = this;

            FormFilling();

            Window.Show();
        }

        public IndividualClientInfo_Window Window { get; set; }

        public Client SelectedClient { get; set; }

        private ObservableCollection<VIP_Individual> VIP_IndividualsForUI;

        private ObservableCollection<Individual> IndividualsForUI;

        private bool isVIP;
        /// <summary>
        /// Свойство, отражающее обладает ли клиент ВИП-статусом
        /// </summary>
        public bool IsVIP
        {
            get { return isVIP; }
            set 
            { 
                isVIP = value;
                OnPropertyChanged("IsVIP");

                ChangeVIPStatus();
            }
        }

        private bool isOpen;
        /// <summary>
        /// Свойство, отражающее открыт или закрыт текущий счет клиента
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                OnPropertyChanged("IsOpen");

                OpenOrCloseAccount();
            }
        }


        /// <summary>
        /// Возвращает копию принятого в качестве аргумента клиента, но с противоположным статусом привелегированности
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        private Individual GetNewClient<T>(T client)
            where T : Individual
        {
            Individual newClient = client.GetType() == typeof(Individual) ? new VIP_Individual() : new Individual();

            newClient.Name = SelectedClient.Name;
            newClient.DateOfBirth = (SelectedClient as Individual).DateOfBirth;
            newClient.ID = SelectedClient.ID;
            newClient.PassportNum = (SelectedClient as Individual).PassportNum;
            newClient.RegistrationDate = SelectedClient.RegistrationDate;
            newClient.TransactionHistory = SelectedClient.TransactionHistory;
            newClient.ClientAddress = SelectedClient.ClientAddress;
            newClient.ClientDeposit = (SelectedClient as Individual).ClientDeposit;
            newClient.ClientAccount = SelectedClient.ClientAccount;
            newClient.ClientDeposit.SetDepositInterest(newClient);
            newClient.ClientCredit = SelectedClient.ClientCredit;
            newClient.ClientCredit.SetCreditInterest(newClient);
            newClient.IsSelected = true;

            return newClient;
        }

        /// <summary>
        /// Изменяет статус привелегированности клиента (ВИП/ не ВИП) на противоположный 
        /// </summary>
        private void ChangeVIPStatus()
        {
            if (IsVIP && SelectedClient.GetType() == typeof(Individual))
            {
                if (MessageBox.Show($"Are you sure you want to make {SelectedClient.Name} a VIP-client?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    VIP_Individual vIP_Individual = (VIP_Individual)GetNewClient(SelectedClient as Individual);

                    vIP_Individual.TransactionHistory.Add(new TransactionInfo 
                        (
                            SelectedClient,
                            0,
                            DateTime.Now,
                            0,
                            0,
                            "Assigning VIP status to a client.",
                            "",
                            ""
                        ));

                    VIP_IndividualsForUI.Add(vIP_Individual);

                    IndividualsForUI.Remove(SelectedClient as Individual);

                    SelectedClient = vIP_Individual;

                    Window.dg_TransactionHistory.ItemsSource = null;
                    Window.dg_TransactionHistory.ItemsSource = SelectedClient.TransactionHistory;
                }
                else
                {
                    Window.chb_VIP.IsChecked = false;
                    return;
                }
            }
            else if (!IsVIP && SelectedClient.GetType() == typeof(VIP_Individual))
            {
                if (MessageBox.Show($"Are you sure you want to deprive {SelectedClient.Name} of his VIP-status?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Individual individual = GetNewClient(SelectedClient as Individual);

                    individual.TransactionHistory.Add(new TransactionInfo
                    (
                         SelectedClient,
                         0,
                         DateTime.Now,
                         0,
                         0,
                         "Depriving a client of VIP status.",
                         "",
                         ""
                    ));

                    IndividualsForUI.Add(individual);

                    VIP_IndividualsForUI.Remove(SelectedClient as VIP_Individual);

                    SelectedClient = individual;

                    Window.dg_TransactionHistory.ItemsSource = null;
                    Window.dg_TransactionHistory.ItemsSource = SelectedClient.TransactionHistory;
                }
                else
                {
                    Window.chb_VIP.IsChecked = true;
                    return;
                }
            }
            else return;
        }


        /// <summary>
        /// Открывает или закрывает текущий счет клиента (изменяет данный параметр на противоположный)
        /// </summary>
        private void OpenOrCloseAccount()
        {
            if (IsOpen != SelectedClient.ClientAccount.IsOpen)
            {
                string openOrCloseWord = SelectedClient.ClientAccount.IsOpen ? "close" : "open";

                if (MessageBox.Show($"Are you sure you want to {openOrCloseWord} account?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // изменение значения параметра на противоположное
                    SelectedClient.ClientAccount.IsOpen = !SelectedClient.ClientAccount.IsOpen;

                    // сообщение для истории транзакций
                    string message = SelectedClient.ClientAccount.IsOpen ? "Unblocking the client's current account" : "Blocking / closing the client's current account";

                    SelectedClient.TransactionHistory.Add(new TransactionInfo
                    (
                         SelectedClient,
                         0,
                         DateTime.Now,
                         0,
                         0,
                         message,
                         "",
                         ""
                    ));

                    Window.dg_TransactionHistory.ItemsSource = null;
                    Window.dg_TransactionHistory.ItemsSource = SelectedClient.TransactionHistory;
                }
                else return;
            }
            else return;
        }


        /// <summary>
        /// Заполняет форму профиля клиента в UI
        /// </summary>
        private void FormFilling()
        {
            IsVIP = SelectedClient is VIP_Individual;
            IsOpen = SelectedClient.ClientAccount.IsOpen;
            Window.tblock_Name.Text = SelectedClient.Name.ToUpperInvariant();
            Window.tb_Name.Text = SelectedClient.Name;
            Window.tb_AccountNum.Text = SelectedClient.ClientAccount.AccountNumber;
            Window.tb_Age.Text = ((int)((DateTime.Now - (SelectedClient as Individual).DateOfBirth).TotalDays / 365)).ToString();
            Window.tb_ApartmentNum.Text = SelectedClient.ClientAddress.ApartmentNumber;
            Window.tb_Balans.Text = string.Format("{0:f2}", SelectedClient.ClientAccount.Balans);
            Window.tb_BirthDate.Text = (SelectedClient as Individual).DateOfBirth.ToShortDateString();
            Window.tb_City.Text = SelectedClient.ClientAddress.CityName;
            Window.tb_Country.Text = SelectedClient.ClientAddress.CountryName;
            Window.tb_HouseNum.Text = SelectedClient.ClientAddress.HouseNumber;
            Window.tb_ID.Text = SelectedClient.ID;
            Window.tb_PassportNum.Text = (SelectedClient as Individual).PassportNum;
            Window.tb_Street.Text = SelectedClient.ClientAddress.StreetName;
            Window.dg_TransactionHistory.ItemsSource = SelectedClient.TransactionHistory;

            Window.btn_SaveChanges.IsEnabled = false;
            Window.btn_SaveChanges.Visibility = Visibility.Collapsed;

            Window.btn_Edit.IsEnabled = true;
            Window.btn_Edit.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Активирует режим редактирования профиля клиента в UI
        /// </summary>
        private void ClientInfoEdit()
        {
            Window.tb_Age.Background = Brushes.AntiqueWhite;
            Window.tb_BirthDate.Background = Brushes.AntiqueWhite;
            Window.tb_Balans.Background = Brushes.AntiqueWhite;
            Window.tb_AccountNum.Background = Brushes.AntiqueWhite;
            Window.tb_ID.Background = Brushes.AntiqueWhite;

            Window.tb_ApartmentNum.IsReadOnly = false;
            Window.tb_City.IsReadOnly = false;
            Window.tb_Country.IsReadOnly = false;
            Window.tb_HouseNum.IsReadOnly = false;
            Window.tb_Name.IsReadOnly = false;
            Window.tb_PassportNum.IsReadOnly = false;
            Window.tb_Street.IsReadOnly = false;

            Window.btn_Edit.IsEnabled = false;
            Window.btn_Edit.Visibility = Visibility.Collapsed;

            Window.btn_SaveChanges.IsEnabled = true;
            Window.btn_SaveChanges.Visibility = Visibility.Visible;

            MessageBox.Show("Edit mode is enabled");
        }


        /// <summary>
        /// Сохраняет изменения в профиле
        /// </summary>
        private void ClientInfoSaveChanges()
        {
            SelectedClient.Name = Window.tb_Name.Text;
            (SelectedClient as Individual).PassportNum = Window.tb_PassportNum.Text;

            SelectedClient.ClientAddress.CountryName = Window.tb_Country.Text;
            SelectedClient.ClientAddress.CityName = Window.tb_City.Text;
            SelectedClient.ClientAddress.StreetName = Window.tb_Street.Text;
            SelectedClient.ClientAddress.HouseNumber = Window.tb_HouseNum.Text;
            SelectedClient.ClientAddress.ApartmentNumber = Window.tb_ApartmentNum.Text;

            Window.tb_Age.Background = Brushes.White;
            Window.tb_BirthDate.Background = Brushes.White;
            Window.tb_Balans.Background = Brushes.White;
            Window.tb_AccountNum.Background = Brushes.White;
            Window.tb_ID.Background = Brushes.White;

            FormFilling();

            MessageBox.Show("Сhanges have been saved");
        }


        public RelayCommand editComand;         // команда, запускающая режим редактирования профиля
        /// <summary>
        /// Команада, запускающая режим редактирования профиля
        /// </summary>
        public RelayCommand EditComand
        {
            get
            {
                return editComand ??
                  (editComand = new RelayCommand(obj =>
                  {
                      ClientInfoEdit();
                  }));
            }
        }


        public RelayCommand saveComand;     // команда, сохраняющая изменения в профиле клиента
        /// <summary>
        /// Команада, сохраняющая изменения в профиле клиента
        /// </summary>
        public RelayCommand SaveComand
        {
            get
            {
                return saveComand ??
                  (saveComand = new RelayCommand(obj =>
                  {
                      ClientInfoSaveChanges();
                  }));
            }
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
