using BaseBankSubstances.Bank_operations;
using DZ_13_Banking_system;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace BaseBankSubstances
{
    public class DataContextForMainWindow : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="mainWindow"></param>
        public DataContextForMainWindow(MainWindow mainWindow)
        {
            // инициализация списков клиентов
            IndividualsForUI = TestBankingSystemCreation.bankSistem.Individuals;

            LegalEntitiesForUI = TestBankingSystemCreation.bankSistem.LegalEntities;

            VIP_IndividualsForUI = TestBankingSystemCreation.bankSistem.Vip_Individuals;

            // инициализация элементов UI
            this.mainWindow = mainWindow;

            this.mainWindow.DataContext = this;

            pageForIndividuals = new PageForIndividuals();

            pageForIndividuals.DataContext = this;

            pageForLegalEntity = new PageForLegalEntity();

            pageForLegalEntity.DataContext = this;

            CurrentPage = pageForIndividuals;

            mainWindow.btn_Individuals.FontWeight = FontWeights.SemiBold;

            IsTestModeActivated = false;

            Client.SelectedClientEvent += SetSelectedClient;
        }

        #region////////////// Поля и свойства \\\\\\\\\\\\\

        public ObservableCollection<Individual> IndividualsForUI { get; set; }

        public ObservableCollection<VIP_Individual> VIP_IndividualsForUI { get; set; }

        public ObservableCollection<LegalEntity> LegalEntitiesForUI { get; set; }

       // public ObservableCollection<Client> AllClients { get; set; }

        public MainWindow mainWindow;

        public Client_Info_Edit_Page individualsInfoEditPage;

        public LegalEntityEditPage legalEntityEditPage;

        public MoneyTransferPage moneyTransferPage;

        private PageForIndividuals pageForIndividuals;

        private PageForLegalEntity pageForLegalEntity;

        private Page currentPage;
        /// <summary>
        /// Тукущая страница UI
        /// </summary>
        public Page CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        public bool isTestModeActivated; // = false;
        /// <summary>
        /// Маркер активации тестового режима
        /// </summary>
        public bool IsTestModeActivated
        {
            get => isTestModeActivated;
            set
            {
                if (isTestModeActivated != value)
                {
                    isTestModeActivated = value;

                    mainWindow.tb_TestModeAlert.Text = isTestModeActivated ? "TEST MODE!" : "";
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
                }
            }
        }
        #endregion

        #region/////////////    Команды   \\\\\\\\\\\\\\\\

        public RelayCommand pageForIndividualsCommand;          // команда, открывающая страницу UI для работы с физическими лицами
        /// <summary>
        /// Команада, открывающая страницу UI для работы с физическими лицами
        /// </summary>
        public RelayCommand PageForIndividualsCommand
        {
            get
            {
                return pageForIndividualsCommand ??
                  (pageForIndividualsCommand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      CurrentPage = pageForIndividuals;

                      // отмена выделения жирным шрифтом любых пунктов главного меню
                      ButtonsFontWeightsClear();

                      // выделение жирным шрифтом выбранного элемента списка главного меню
                      mainWindow.btn_Individuals.FontWeight = FontWeights.SemiBold;
                  }));
            }
        }


        public RelayCommand pageForLegalEntitysCommand;         // команда, открывающая страницу UI для работы с юридическими лицами
        /// <summary>
        /// Команада, открывающая страницу UI для работы с юридическими лицами
        /// </summary>
        public RelayCommand PageForLegalEntitysCommand
        {
            get
            {
                return pageForLegalEntitysCommand ??
                  (pageForLegalEntitysCommand = new RelayCommand(obj =>
                  {
                      CurrentPage = pageForLegalEntity;

                      // отмена выделения жирным шрифтом любых пунктов главного меню
                      ButtonsFontWeightsClear();

                      // выделение жирным шрифтом выбранного элемента списка главного меню
                      mainWindow.btn_LegalEntitys.FontWeight = FontWeights.SemiBold;
                  }));
            }
        }


        public RelayCommand monyTransferComand;    // команда, открывающая страницу для оформления денежного перевода
        /// <summary>
        /// Команада, открывающая страницу для оформления денежного перевода
        /// </summary>
        public RelayCommand MonyTransferComand
        {
            get
            {
                return monyTransferComand ??
                  (monyTransferComand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      // создание экземпляа класса, отвечающего за денежные переводы
                      MonyTransfer monyTransfer = new MonyTransfer(this);

                      // отмена выделения жирным шрифтом любых пунктов главного меню
                      ButtonsFontWeightsClear();

                      // выделение жирным шрифтом выбранного элемента списка главного меню
                      mainWindow.btn_MoneyTransfer.FontWeight = FontWeights.SemiBold;
                  }));
            }
        }


        public RelayCommand creditOpenCommand;     // команда, открывающая страницу для оформления кредита
        /// <summary>
        /// Команада, открывающая страницу для оформления денежного перевода
        /// </summary>
        public RelayCommand CreditOpenCommand
        {
            get
            {
                return creditOpenCommand ??
                  (creditOpenCommand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      // создание экземпляа класса, отвечающего за открытие кредитов
                      CreditOpen creditOpen = new CreditOpen(this);

                      // отмена выделения жирным шрифтом любых пунктов главного меню
                      ButtonsFontWeightsClear();

                      // выделение жирным шрифтом выбранного элемента списка главного меню
                      mainWindow.btn_CreditOpen.FontWeight = FontWeights.SemiBold;
                  }));
            }
        }


        public RelayCommand individualClientRegistrationComand; // команда, открывающая окно регистрации нового клиента (физического лица)
        /// <summary>
        /// Команада, открывающая окно регистрации нового клиента - физического лица
        /// </summary>
        public RelayCommand IndividualClientRegistrationComand
        {
            get
            {
                return individualClientRegistrationComand ??
                  (individualClientRegistrationComand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      // создание экземпляа класса, отвечающего за регистрацию физичеких лиц
                      ClientRegistration clientRegistrator = new ClientRegistration();
                  }));
            }
        }


        public RelayCommand legalEntityRegistrationComand;      // команда, открывающая окно регистрации нового клиента (юридического лица)
        /// <summary>
        /// Команада, открывающая окно регистрации нового клиента (юридического лица)
        /// </summary>
        public RelayCommand LegalEntityRegistrationComand
        {
            get
            {
                return legalEntityRegistrationComand ??
                  (legalEntityRegistrationComand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      // создание экземпляа класса, отвечающего за регистрацию юридических лиц
                      LegalEntityRegistratoin legalEntityRegistrator = new LegalEntityRegistratoin();
                  }));
            }
        }


        public RelayCommand depositOpenComand;                  // команда, открывающая окно оформления депозитов
        /// <summary>
        /// Команада, открывающая окно оформления депозитов
        /// </summary>
        public RelayCommand DepositOpenComand
        {
            get
            {
                return depositOpenComand ??
                  (depositOpenComand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      // создание экземпляа класса, отвечающего за оформление депозитов
                      DepositOpen depositOpen = new DepositOpen(this);

                  }));
            }
        }


        public RelayCommand openClientInfoEditPageComand;       // команда, открывающая страницу просмотра и редактирования клиентов (физических лиц)
        /// <summary>
        /// Команада, открывающая страницу просмотра и редактирования клиентов (физических лиц)
        /// </summary>
        public RelayCommand OpenClientInfoEditPageComand
        {
            get
            {
                return openClientInfoEditPageComand ??
                  (openClientInfoEditPageComand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      // создание экземпляа класса (страницы UI), в которой осуществляется просмотр и редктирование информации о физических лицах
                      individualsInfoEditPage = new Client_Info_Edit_Page();

                      individualsInfoEditPage.DataContext = this;

                      CurrentPage = individualsInfoEditPage;
                  }));
            }
        }


        public RelayCommand openLegalEntityInfoEditPageComand;  // команда, открывающая страницу просмотра и редактирования клиентов (юр. лиц)
        /// <summary>
        /// Команада, открывающая страницу просмотра и редактирования клиентов (юр. лиц)
        /// </summary>
        public RelayCommand OpenLegalEntityInfoEditPageComand
        {
            get
            {
                return openLegalEntityInfoEditPageComand ??
                  (openLegalEntityInfoEditPageComand = new RelayCommand(obj =>
                  {
                      SelectedClient = null;

                      // создание экземпляа класса (страницы UI), в которой осуществляется просмотр и редктирование информации о юр. лицах
                      legalEntityEditPage = new LegalEntityEditPage();

                      legalEntityEditPage.DataContext = this;

                      CurrentPage = legalEntityEditPage;
                  }));
            }
        }


        public RelayCommand clientInfoEditComand;               // команда, открывающая окно просмотра и редактироания профиля ВЫБРАННОГО клиента 
        /// <summary>
        /// Команада, открывающая окно просмотра и редактироания профиля ВЫБРАННОГО клиента
        /// </summary>
        public RelayCommand ClientInfoEditComand
        {
            get
            {
                return clientInfoEditComand ??
                  (clientInfoEditComand = new RelayCommand(obj =>
                  {
                      if (SelectedClient == null)
                      {
                          MessageBox.Show("Select the client, please");
                          return;
                      }

                      if (selectedClient is Individual)
                      {
                          // создание экземпляа класса, обеспечивающего просмотр и редктирование информации о физ. лицах
                          IndividualClientInfo individualClientInfo = new IndividualClientInfo(SelectedClient, IndividualsForUI, VIP_IndividualsForUI);
                      }
                      else
                      {
                          // создание экземпляа класса, обеспечивающего просмотр и редктирование информации о юр. лицах
                          LegalEntityInfo legalEntityInfo = new LegalEntityInfo(SelectedClient);
                      }

                  }));
            }
        }

        

        public RelayCommand operationsJornalCommand;              // команда, открывающая страницу операционного журнала
        /// <summary>
        /// Команада, открывающая страницу операционного журнала
        /// </summary>
        public RelayCommand OperationsJornalCommand
        {
            get
            {
                return operationsJornalCommand ??
                  (operationsJornalCommand = new RelayCommand(obj =>
                  {
                      // создание страницы операционного журнала
                      OperationJornalPage operationJornalPage = new OperationJornalPage();

                      CurrentPage = operationJornalPage;

                      // отмена выделения жирным шрифтом любых пунктов главного меню
                      ButtonsFontWeightsClear();

                      // выделение жирным шрифтом выбранного элемента списка главного меню
                      mainWindow.btn_OperationsJornal.FontWeight = FontWeights.SemiBold;
                  }));
            }
        }


        public RelayCommand testSystemPageCommand;              // команда, запускающая алгоритм закрытия выбранного счета
        /// <summary>
        /// Команада, которая при нажатии на соответствующую кнопку вызывает алгоритм закрытия выбранного счета
        /// </summary>
        public RelayCommand TestSystemPageCommand
        {
            get
            {
                return testSystemPageCommand ??
                  (testSystemPageCommand = new RelayCommand(obj =>
                  {
                      // создание экземпляра класса, отвечающего за тестирование системы
                      TestSystem testSystem = new TestSystem(this);

                      // отмена выделения жирным шрифтом любых пунктов главного меню
                      ButtonsFontWeightsClear();

                      // выделение жирным шрифтом выбранного элемента списка главного меню
                      mainWindow.btn_TestSystem.FontWeight = FontWeights.SemiBold; 
                  }));
            }
        }

        #endregion

        #region///////////// Методы и события \\\\\\\\\\\\\

        /// <summary>
        /// Присваивает свойству SelectedClient ссылку на того клиента, который выбран в UI
        /// </summary>
        /// <param name="client"></param>
        private void SetSelectedClient(Client client)
        {
            SelectedClient = client;
        }

        /// <summary>
        /// Отменяет выделение жирным шрифтом любых пунктов главного меню
        /// </summary>
        private void ButtonsFontWeightsClear()
        {
            mainWindow.btn_Individuals.FontWeight = FontWeights.Normal;
            mainWindow.btn_LegalEntitys.FontWeight = FontWeights.Normal;
            mainWindow.btn_MoneyTransfer.FontWeight = FontWeights.Normal;
            mainWindow.btn_CreditOpen.FontWeight = FontWeights.Normal;
            mainWindow.btn_OperationsJornal.FontWeight = FontWeights.Normal;
            mainWindow.btn_TestSystem.FontWeight = FontWeights.Normal;
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

        #endregion

    }
}
