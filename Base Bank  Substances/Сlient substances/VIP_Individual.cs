using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances
{
    public class VIP_Individual : Individual
    {
        /// <summary>
        /// Конструтор класса
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
        public VIP_Individual(DateTime registrationDate, string name, string iD, Address clientAddress, Account clientAccount,
        Credit clientCredit, List<TransactionInfo> transactionHistory, bool isSelected, DateTime dateOfBirth,
        string passportNum)

        : base(registrationDate, name, iD, clientAddress, clientAccount,
        clientCredit, transactionHistory, isSelected, dateOfBirth,
        passportNum)
        {

        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public VIP_Individual()
        {

        }


    }
}
