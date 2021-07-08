using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_13_Banking_system
{
    class Address : ICloneable
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="cityName"></param>
        /// <param name="streetName"></param>
        /// <param name="houseNumber"></param>
        /// <param name="apartmentNumber"></param>
        public Address(string countryName, string cityName, string streetName, string houseNumber, string apartmentNumber)
        {
            CountryName = countryName;
            CityName = cityName;
            StreetName = streetName;
            HouseNumber = houseNumber;
            ApartmentNumber = apartmentNumber;
        }

        public string CountryName { get; set; }

        public string CityName { get; set; }

        public string StreetName { get; set; }

        public string HouseNumber { get; set; }

        public string ApartmentNumber { get; set; }


        /// <summary>
        /// Реализация интерфейса ICloneable
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Address address = (Address)MemberwiseClone();

            return address;
        }
    }
}
