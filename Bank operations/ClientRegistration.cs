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
    class ClientRegistration : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public ClientRegistration()
        {
            _window = new ClientRegistrationWindow();

            _window.DataContext = this;

            _window.Show();
        }

        static Random random = new Random();

        /// <summary>
        /// Окно регистрации клиента
        /// </summary>
        public ClientRegistrationWindow _window;

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

                // заполнение поля "дата рождения" в UI
                _window.tb_BirthDate.Text = SelectedDate.ToShortDateString();

                // определение возраста клиента
                int age = (int)(SelectedDate - DateTime.Now).TotalDays / 365 * (-1);

                // если возраст менее 16 лет значение возраста в форме окрашивается в красный
                if (age < 16)
                {
                    _window.tb_Age.Foreground = Brushes.Red;     
                }
                else _window.tb_Age.Foreground = Brushes.Black;

                _window.tb_Age.Text = ((int)(SelectedDate - DateTime.Now).TotalDays/365*(-1)).ToString();
            }
        }


        /// <summary>
        /// Завершает регистрацию с вызовом метода проверки заполнения полей формы
        /// </summary>
        private void ClientRegistrationFinish()
        {
            Client newClient;

            // определение типа создаваемого клиента
            if (!String.IsNullOrEmpty(_window.cb_ClientType.Text))
            {
                switch (_window.cb_ClientType.Text)
                {
                    case "Individual": newClient = new Individual(); break;
                    case "VIP Individual": newClient = new VIP_Individual(); break;
                    default: return;
                }
            }
            else { MessageBox.Show("Enter client type!"); return; }

            // вызов метода проверки заполнения всех полей формы
            if (!FormCheck()) return;

            // инициализация клиента
            newClient.RegistrationDate = DateTime.Now;
            newClient.Name = _window.tb_Surname.Text + " " + _window.tb_Name.Text;
            newClient.ID = _window.tb_ID.Text;
            (newClient as Individual).PassportNum = _window.tb_PassportNum.Text;
            (newClient as Individual).DateOfBirth = SelectedDate;
            newClient.ClientAddress = new Address
            (
                _window.tb_Country.Text,
                _window.tb_City.Text,
                _window.tb_Street.Text,
                _window.tb_HouseNum.Text,
                String.IsNullOrWhiteSpace(_window.tb_ApartmentNum.Text) ? String.Empty : _window.tb_ApartmentNum.Text
            );
            newClient.ClientAccount = new Account
            (
                Account.GetAccountNumber(),
                $"{Guid.NewGuid().ToString().Substring(5, 11)}",
                DateTime.Now,
                0,
                true
            );
            newClient.TransactionHistory = new List<TransactionInfo>();
            (newClient as Individual).ClientDeposit = new Deposit();
            newClient.ClientCredit = new Credit();

            Account.AddTransactionInfoAccountOpen(newClient);

            // добавление клиента в коллекцию в зависимости от его типа (ВИП или не ВИП)
            if (newClient.GetType() == typeof(Individual))
            {
                TestBankingSystemCreation.bankSistem.Individuals.Add(newClient as Individual);
            }
            else TestBankingSystemCreation.bankSistem.Vip_Individuals.Add(newClient as VIP_Individual);

            MessageBox.Show("Registration successfully completed");
            _window.Close();
        }

        /// <summary>
        /// Проверяет заполнение всех полей в форме
        /// </summary>
        /// <returns></returns>
        private bool FormCheck()
        {
            if (string.IsNullOrWhiteSpace(_window.tb_ID.Text)) { MessageBox.Show("Fill in the client ID!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_Age.Text)) { MessageBox.Show("Select the client's date of birth in the calendar!"); return false; }

            if (int.Parse(_window.tb_Age.Text) < 16) { MessageBox.Show("The client's age cannot be less than 16 years"); return false; }

            if ((bool)(!_window.rb_Female.IsChecked & !_window.rb_Male.IsChecked)) { MessageBox.Show("Choose the client's gender!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_PassportNum.Text)) { MessageBox.Show("Enter the client's passport number!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_Name.Text)) { MessageBox.Show("Enter the client's name!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_Surname.Text)) { MessageBox.Show("Enter the client's last name!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_Country.Text)) { MessageBox.Show("Enter client's country of residence!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_City.Text)) { MessageBox.Show("Enter the client's city of residence!"); return false; }

            if (string.IsNullOrWhiteSpace(_window.tb_Street.Text)) { MessageBox.Show("Enter the customer's street of residence!"); return false; }

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
                      _window.tb_ID.Text = Guid.NewGuid().ToString().Substring(0,30);
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
