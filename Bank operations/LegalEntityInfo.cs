using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DZ_13_Banking_system
{
    class LegalEntityInfo : INotifyPropertyChanged
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
        /// <param name="SelectedClient"></param>
        public LegalEntityInfo(Client SelectedClient)
        {
            this.SelectedClient = SelectedClient as LegalEntity;

            Window = new LegalEntityInfo_Window();

            Window.DataContext = this;

            FormFilling();

            Window.Show();
        }

        public LegalEntityInfo_Window Window { get; set; }

        public LegalEntity SelectedClient { get; set; }

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
        /// Заполняет форму профиля клиента в UI
        /// </summary>
        private void FormFilling()
        {
            IsOpen = SelectedClient.ClientAccount.IsOpen;
            Window.tblock_Name.Text = SelectedClient.Name.ToUpperInvariant();
            Window.tb_Name.Text = SelectedClient.Name;
            Window.tb_AccountNum.Text = SelectedClient.ClientAccount.AccountNumber;
            Window.tb_ApartmentNum.Text = SelectedClient.ClientAddress.ApartmentNumber;
            Window.tb_Balans.Text = String.Format("{0:f2}", SelectedClient.ClientAccount.Balans);
            Window.tb_StateRegistrationDate.Text = SelectedClient.StateRegistrationDate.ToShortDateString();
            Window.tb_City.Text = SelectedClient.ClientAddress.CityName;
            Window.tb_Country.Text = SelectedClient.ClientAddress.CountryName;
            Window.tb_HouseNum.Text = SelectedClient.ClientAddress.HouseNumber;
            Window.tb_ID.Text = SelectedClient.ID;
            Window.tb_StateRgistrationNum.Text = (SelectedClient).StateRgistrationNum;
            Window.tb_Street.Text = SelectedClient.ClientAddress.StreetName;
            Window.dg_TransactionHistory.ItemsSource = SelectedClient.TransactionHistory;

            Window.btn_SaveChanges.IsEnabled = false;
            Window.btn_SaveChanges.Visibility = Visibility.Collapsed;

            Window.btn_Edit.IsEnabled = true;
            Window.btn_Edit.Visibility = Visibility.Visible;
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

                    AddNewTransaction(SelectedClient.TransactionHistory[SelectedClient.TransactionHistory.Count - 1]);

                    Window.dg_TransactionHistory.ItemsSource = null;
                    Window.dg_TransactionHistory.ItemsSource = SelectedClient.TransactionHistory;
                }
                else return;
            }
            else return;
        }


        /// <summary>
        /// Активирует режим редактирования профиля клиента в UI
        /// </summary>
        private void ClientInfoEdit()
        {
            Window.tb_StateRegistrationDate.Background = Brushes.AntiqueWhite;
            Window.tb_Balans.Background = Brushes.AntiqueWhite;
            Window.tb_AccountNum.Background = Brushes.AntiqueWhite;
            Window.tb_ID.Background = Brushes.AntiqueWhite;
            Window.tb_StateRgistrationNum.Background = Brushes.AntiqueWhite;


            Window.tb_ApartmentNum.IsReadOnly = false;
            Window.tb_City.IsReadOnly = false;
            Window.tb_Country.IsReadOnly = false;
            Window.tb_HouseNum.IsReadOnly = false;
            Window.tb_Name.IsReadOnly = false;
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

            SelectedClient.ClientAddress.CountryName = Window.tb_Country.Text;
            SelectedClient.ClientAddress.CityName = Window.tb_City.Text;
            SelectedClient.ClientAddress.StreetName = Window.tb_Street.Text;
            SelectedClient.ClientAddress.HouseNumber = Window.tb_HouseNum.Text;
            SelectedClient.ClientAddress.ApartmentNumber = Window.tb_ApartmentNum.Text;

            Window.tb_StateRgistrationNum.Background = Brushes.White;
            Window.tb_StateRegistrationDate.Background = Brushes.White;
            Window.tb_Balans.Background = Brushes.White;
            Window.tb_AccountNum.Background = Brushes.White;
            Window.tb_ID.Background = Brushes.White;

            FormFilling();

            MessageBox.Show("Сhanges have been saved");
        }


        public RelayCommand editComand;                                        
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


        public RelayCommand saveComand;                                        
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
