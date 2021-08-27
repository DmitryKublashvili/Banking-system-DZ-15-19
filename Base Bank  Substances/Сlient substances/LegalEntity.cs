using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances
{
    public class LegalEntity : Client, ICloneable
    {
        public LegalEntity(DateTime registrationDate, string name, string iD, Address clientAddress, 
            Account clientAccount, Credit clientCredit, List<TransactionInfo> transactionHistory, 
            bool isSelected, DateTime stateRegistrationDate, string stateRgistrationNum)
            : base(registrationDate, name, iD, clientAddress, clientAccount, transactionHistory, isSelected)
        {
            StateRegistrationDate = stateRegistrationDate;
            StateRgistrationNum = stateRgistrationNum;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public LegalEntity() { }

        /// <summary>
        /// Дата государственной регистрации юр.лица
        /// </summary>
        public DateTime StateRegistrationDate { get; set; }

        /// <summary>
        /// Номер государственной регистрации юр.лица
        /// </summary>
        public string StateRgistrationNum { get; set; }

        /// <summary>
        /// Реализация интерфейса ICloneable
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            LegalEntity legalEntity = (LegalEntity)MemberwiseClone();

            legalEntity.TransactionHistory = new List<TransactionInfo>(TransactionHistory);

            return legalEntity;
        }
    }
}
