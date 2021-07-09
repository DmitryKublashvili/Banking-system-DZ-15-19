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
    class TestSystem : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="dataContextForMainWindow"></param>
        public TestSystem(DataContextForMainWindow dataContextForMainWindow)
        {
            // если тестовый режим уже активирован используется ранее созданная страница
            if (dataContextForMainWindow.IsTestModeActivated)
            {
                dataContextForMainWindow.CurrentPage = TestSystemPage;
                return;
            }

            TestSystemPage = new TestSystemPage();
            TestSystemPage.DataContext = this;

            DataContextForMain = dataContextForMainWindow;
            DataContextForMain.CurrentPage = TestSystemPage;

            CopyIndividualsForUI = new ObservableCollection<Individual>();
            CopyVIP_IndividualsForUI = new ObservableCollection<VIP_Individual>();
            CopyLegalEntitiesForUI = new ObservableCollection<LegalEntity>();

            // настройка невидимых/неактивных элементов UI (кнопка деактивации режима тестирования)
            TestSystemPage.btn_TestingComplete.IsEnabled = false;
            TestSystemPage.btn_TestingComplete.Visibility = Visibility.Collapsed;
        }


        private DateTime testDate;
        /// <summary>
        /// Дата, по состоянию на которую осуществляется перерасчет системы
        /// </summary>
        public DateTime TestDate
        {
            get => testDate;
            set
            {
                testDate = value;
                OnPropertyChanged("TestDate");
            }
        }

        public static ObservableCollection<Individual> CopyIndividualsForUI { get; set; } 

        public static ObservableCollection<VIP_Individual> CopyVIP_IndividualsForUI { get; set; } 

        public static ObservableCollection<LegalEntity> CopyLegalEntitiesForUI { get; set; } 

        public static DataContextForMainWindow DataContextForMain { get; set; }

        public static TestSystemPage TestSystemPage { get; set; }


        /// <summary>
        /// Создание резервных коллекций клиентов
        /// </summary>
        private static void CreatingBackupsOfClientLists()
        {
            foreach (var item in DataContextForMain.IndividualsForUI)
            {
                CopyIndividualsForUI.Add((Individual)item.Clone());
            }

            foreach (var item in DataContextForMain.VIP_IndividualsForUI)
            {
                CopyVIP_IndividualsForUI.Add((VIP_Individual)item.Clone());
            }

            foreach (var item in DataContextForMain.LegalEntitiesForUI)
            {
                CopyLegalEntitiesForUI.Add((LegalEntity)item.Clone());
            }
        }


        /// <summary>
        /// Перерасчет системы на выбранную дату
        /// </summary>
        private void SystemRecalculation()
        {
            CreatingBackupsOfClientLists();

            foreach (var client in DataContextForMain.IndividualsForUI)
            {
                // если нет ни кредита ни депозита
                if (client.ClientCredit.CreditAmount == 0 && client.ClientDeposit.DepositAmount == 0)
                {
                    continue;
                }

                // расчетная дата начиная со следующего дня
                DateTime settlementDate = DateTime.Now.AddDays(1);

                while (settlementDate <= TestDate && (client.ClientCredit.CreditAmount > 0 || client.ClientDeposit.DepositAmount > 0))
                {
                    // если кредит есть
                    if (client.ClientCredit.CreditAmount > 0)
                    {
                        Credit.LoanRepaymentCalculation(client, settlementDate);
                    }

                    // если депозит есть
                    if (client.ClientDeposit.DepositAmount > 0)
                    {
                        Deposit.AccrualOfInterestOnDeposit(client, settlementDate);
                    }

                    settlementDate = settlementDate.AddDays(1);
                }
            }

            foreach (var vip_client in DataContextForMain.VIP_IndividualsForUI)
            {
                if (vip_client.ClientCredit.CreditAmount == 0 && vip_client.ClientDeposit.DepositAmount == 0)
                {
                    continue;
                }

                DateTime settlementDate = DateTime.Now.AddDays(1);

                while (settlementDate <= TestDate && (vip_client.ClientCredit.CreditAmount > 0 || vip_client.ClientDeposit.DepositAmount > 0))
                {
                    // если кредит есть
                    if (vip_client.ClientCredit.CreditAmount > 0)
                    {
                        Credit.LoanRepaymentCalculation(vip_client, settlementDate);
                    }

                    // если депозит есть
                    if (vip_client.ClientDeposit.DepositAmount > 0)
                    {
                        Deposit.AccrualOfInterestOnDeposit(vip_client, settlementDate);
                    }

                    settlementDate = settlementDate.AddDays(1);
                }
            }

            foreach (var legalEntity in DataContextForMain.LegalEntitiesForUI)
            {
                if (legalEntity.ClientCredit.CreditAmount == 0)
                {
                    continue;
                }

                DateTime settlementDate = DateTime.Now.AddDays(1);

                while (settlementDate <= TestDate && legalEntity.ClientCredit.CreditAmount > 0)
                {
                    Credit.LoanRepaymentCalculation(legalEntity, settlementDate);
                    settlementDate = settlementDate.AddDays(1);
                }
            }
        }


        /// <summary>
        /// Восстановление системы после тестового режима в первоначальный вид
        /// </summary>
        private static void SystemComeback()
        {
            // очистка базовых коллекций клиентов, в которых производились изменения в тестовом режиме
            DataContextForMain.IndividualsForUI.Clear();
            DataContextForMain.VIP_IndividualsForUI.Clear();
            DataContextForMain.LegalEntitiesForUI.Clear();

            // наполнение базовых коллекций клиентами из резервных копий
            foreach (var item in CopyIndividualsForUI)
            {
                DataContextForMain.IndividualsForUI.Add(item);
            }

            foreach (var item in CopyVIP_IndividualsForUI)
            {
                DataContextForMain.VIP_IndividualsForUI.Add(item);
            }

            foreach (var item in CopyLegalEntitiesForUI)
            {
                DataContextForMain.LegalEntitiesForUI.Add(item);
            }

            // Очистка резервных коллекций
            CopyIndividualsForUI.Clear();
            CopyVIP_IndividualsForUI.Clear();
            CopyLegalEntitiesForUI.Clear();
        }


        public RelayCommand startTestingCommand;                                        
        /// <summary>
        /// Команада, активирующая тестовый режим
        /// </summary>
        public RelayCommand StartTestingCommand
        {
            get
            {
                return startTestingCommand ??
                  (startTestingCommand = new RelayCommand(obj =>
                  {
                      if (TestDate < DateTime.Now)
                      {
                          MessageBox.Show("The test date must be later than the current date");
                          return;
                      }

                      SystemRecalculation();
                      MessageBox.Show("Test mode started");

                      DataContextForMain.IsTestModeActivated = true;

                      // Изменения в UI характеризующие включение тестового режима
                      TestSystemPage.btn_TestingComplete.IsEnabled = true;
                      TestSystemPage.btn_TestingComplete.Visibility = Visibility.Visible;

                      TestSystemPage.btn_StartTesting.IsEnabled = false;
                      TestSystemPage.btn_StartTesting.Visibility = Visibility.Collapsed;
                  }));
            }
        }
        

        public RelayCommand resetTestingCommand;                                        
        /// <summary>
        /// Команада, завершающая тестовый режим
        /// </summary>
        public RelayCommand ResetTestingCommand
        {
            get
            {
                return resetTestingCommand ??
                  (resetTestingCommand = new RelayCommand(obj =>
                  {
                      SystemComeback();
                      MessageBox.Show("Test mode completed");

                      DataContextForMain.IsTestModeActivated = false;

                      // Восстановление элементов UI 
                      TestSystemPage.btn_TestingComplete.IsEnabled = false;
                      TestSystemPage.btn_TestingComplete.Visibility = Visibility.Collapsed;

                      TestSystemPage.btn_StartTesting.IsEnabled = true;
                      TestSystemPage.btn_StartTesting.Visibility = Visibility.Visible;
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
