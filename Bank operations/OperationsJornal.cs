using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances.Bank_operations
{
    class OperationsJornal : INotifyPropertyChanged
    {
        /// <summary>
        /// Статический конструктор класса
        /// </summary>
        static OperationsJornal()
        {
            MainOperationsJornal = new ObservableCollection<TransactionInfo>();

            // подписки на события
            Account.AddNewTransaction += TransactionAddToMainJornal;
            Credit.AddNewTransaction += TransactionAddToMainJornal;
            Deposit.AddNewTransaction += TransactionAddToMainJornal;
            LegalEntityInfo.AddNewTransaction += TransactionAddToMainJornal;
            IndividualClientInfo.AddNewTransaction += TransactionAddToMainJornal;
            MonyTransfer.AddNewTransaction += TransactionAddToMainJornal;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public OperationsJornal()
        {

        }

        private static int operationsCount;
        /// <summary>
        /// Количество операций
        /// </summary>
        public static int OperationsCount
        {
            get { return operationsCount; }
            set 
            { 
                operationsCount = value;
            }
        }


        private static ObservableCollection<TransactionInfo> mainOperationsJornal;
        /// <summary>
        /// Главный операционный журнад
        /// </summary>
        public static ObservableCollection<TransactionInfo> MainOperationsJornal
        {
            get { return mainOperationsJornal; }
            set { mainOperationsJornal = value; }
        }

        /// <summary>
        /// Добавляет запись о транзакции в главный операционный журнал
        /// </summary>
        /// <param name="transactionInfo"></param>
        private static void TransactionAddToMainJornal(TransactionInfo transactionInfo)
        {
            MainOperationsJornal.Add(transactionInfo);
            MainOperationsJornal = new ObservableCollection<TransactionInfo>(MainOperationsJornal.OrderByDescending(x => x.Date));
            OperationsCount = MainOperationsJornal.Count();
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
