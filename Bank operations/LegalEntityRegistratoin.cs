using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseBankSubstances
{
    class LegalEntityRegistratoin : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public LegalEntityRegistratoin()
        {
            _window = new LegalEntityRegistrationWindow();

            _window.DataContext = this;

            _window.Show();
        }

        static Random random = new Random();

        /// <summary>
        /// Окно регистрации клиента
        /// </summary>
        private LegalEntityRegistrationWindow _window;

        private DateTime selectedDate;
        /// <summary>
        /// Выбранная в календаре дата 
        /// </summary>
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged("SelectedDate");

                _window.tb_StateRegistrationDate.Text = SelectedDate.ToShortDateString();
            }
        }


        /// <summary>
        /// Завершает регистрацию с вызовом метода проверки заполнения полей формы
        /// </summary>
        private void ClientRegistrationFinish()
        {
            // вызов метода проверки заполнения всех полей формы
            if (!FormCheck()) return;

            // инициализация клиента
            Client newClient = new LegalEntity
                (
                    DateTime.Now,
                    _window.tb_Name.Text,
                    _window.tb_ID.Text,
                    new Address(
                            _window.tb_Country.Text,
                            _window.tb_City.Text,
                            _window.tb_Street.Text,
                            _window.tb_HouseNum.Text,
                            String.IsNullOrWhiteSpace(_window.tb_ApartmentNum.Text) ? "none" : _window.tb_ApartmentNum.Text),
                    new Account(
                            Account.GetAccountNumber(),
                            $"{Guid.NewGuid().ToString().Substring(5, 11)}",
                            DateTime.Now,
                            0,
                            true),
                    new Credit(),
                    new List<TransactionInfo>(),
                    false,
                    SelectedDate,
                    _window.tb_StateRgistrationNum.Text
                );

            newClient.ClientCredit = new Credit();

            Account.AddTransactionInfoAccountOpen(newClient);

            // добавление клиента в коллекцию
            TestBankingSystemCreation.bankSistem.LegalEntities.Add(newClient as LegalEntity);

            MessageBox.Show("Registration successfully completed");
            _window.Close();
        }

        /// <summary>
        /// Проверяет заполнение всех полей в форме
        /// </summary>
        /// <returns></returns>
        private bool FormCheck()
        {
            if (string.IsNullOrWhiteSpace(_window.tb_Name.Text)) { MessageBox.Show("Enter the client's name!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_StateRgistrationNum.Text)) { MessageBox.Show("Enter the client's state registration number!"); return false; }

            if (_window.tb_StateRegistrationDate.Text == " select on the calendar") { MessageBox.Show("Enter the client's state registration date!"); return false; }

            if (SelectedDate >= DateTime.Now) { MessageBox.Show("The client's state registration date must be earlier than the current date!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_ID.Text)) { MessageBox.Show("Fill client ID!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_Country.Text)) { MessageBox.Show("Enter the country where the client is located!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_City.Text)) { MessageBox.Show("Enter the city where the client is located!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_Street.Text)) { MessageBox.Show("Enter the street where the client is located!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_HouseNum.Text)) { MessageBox.Show("Enter the client's house number!"); return false; }

            return true;
        }


        public RelayCommand registrationCompleteComand;
        /// <summary>
        /// Команада, которая при нажатии на соответствующую кнопку вызывает алгоритм завершения регистрации клиента
        /// </summary>
        public RelayCommand RegistrationCompleteComand
        {
            get
            {
                return registrationCompleteComand ??
                  (registrationCompleteComand = new RelayCommand(obj =>
                  {
                      ClientRegistrationFinish();
                  }));
            }
        }


        public RelayCommand autoIDComand;
        /// <summary>
        /// Команада, которая при нажатии на соответствующую кнопку генерирует случайный ID
        /// </summary>
        public RelayCommand AutoIDComand
        {
            get
            {
                return autoIDComand ??
                  (autoIDComand = new RelayCommand(obj =>
                  {
                      _window.tb_ID.Text = Guid.NewGuid().ToString().Substring(0, 30);
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
