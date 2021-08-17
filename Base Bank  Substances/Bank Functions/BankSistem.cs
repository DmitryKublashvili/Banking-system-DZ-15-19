//using DZ_13_Banking_system.Bank_operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBankSubstances
{
    public class BankSistem
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="individuals">Коллекция клиентов - физических лиц (не ВИП)</param>
        /// <param name="vip_Individuals">Коллекция клиентов - ВИП физических лиц</param>
        /// <param name="legalEntities">Коллекция клиентов - юридических лиц</param>
        public BankSistem(ObservableCollection<Individual> individuals, ObservableCollection<VIP_Individual> vip_Individuals, ObservableCollection<LegalEntity> legalEntities)
        {
            Individuals = individuals;
            Vip_Individuals = vip_Individuals;
            LegalEntities = legalEntities;
            GetAllClients();
        }

        /// <summary>
        /// Коллекция клиентов - физических лиц (не ВИП)
        /// </summary>
        public ObservableCollection<Individual> Individuals { get; set; }

        /// <summary>
        /// Коллекция клиентов - ВИП физических лиц
        /// </summary>
        public ObservableCollection<VIP_Individual> Vip_Individuals { get; set; }

        /// <summary>
        /// Коллекция клиентов - юридических лиц
        /// </summary>
        public ObservableCollection<LegalEntity>  LegalEntities { get; set; }

        /// <summary>
        /// Коллекция всех клиентов
        /// </summary>
        public ObservableCollection<Client> AllClients { get; set; } = new ObservableCollection<Client>();

        /// <summary>
        /// Наполняет коллекцию всех клиентов 
        /// </summary>
        public void GetAllClients()
        {
            foreach (var item in Individuals)
            {
                AllClients.Add(item);
            }

            foreach (var item in Vip_Individuals)
            {
                AllClients.Add(item);
            }

            foreach (var item in LegalEntities)
            {
                AllClients.Add(item);
            }
        }

    }
}
