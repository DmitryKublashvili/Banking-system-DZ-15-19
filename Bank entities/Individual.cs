using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_13_Banking_system
{
    class Individual : Client, ICloneable
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="registrationDate"></param>
        /// <param name="name"></param>
        /// <param name="iD"></param>
        /// <param name="clientAddress"></param>
        /// <param name="clientAccount"></param>
        /// <param name="clientCredit"></param>
        /// <param name="transactionHistory"></param>
        /// <param name="isSelected"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="passportNum"></param>
        /// <param name="clientDeposit"></param>
        public Individual(DateTime registrationDate, string name, string iD, Address clientAddress, Account clientAccount, 
            Credit clientCredit, List<TransactionInfo> transactionHistory, bool isSelected, DateTime dateOfBirth, 
            string passportNum) 
            : base(registrationDate, name, iD, clientAddress, clientAccount, transactionHistory, isSelected)
        {
            DateOfBirth = dateOfBirth;
            PassportNum = passportNum;
           // ClientDeposit = new Deposit();
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Individual()
        {

        }

        /// <summary>
        /// Дата рождения клиента
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Номер паспорта клиента
        /// </summary>
        public string PassportNum { get; set; }

        /// <summary>
        /// Депозит клиента
        /// </summary>
        public Deposit ClientDeposit { get; set; }

        /// <summary>
        /// Реализация интерфейса ICloneable
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Individual individual = (Individual)MemberwiseClone();

            individual.ClientDeposit = (Deposit)ClientDeposit.Clone();

            individual.TransactionHistory = new List<TransactionInfo>(TransactionHistory);

            return individual;
        }







    }
}
