﻿using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Address : IElement<Address>, ISecureElement
    {
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public string HouseNameNumber { get; set; }
        public IEnumerable<string>? AddressLines { get; set; }
        public string? CityTown { get; set; }
        public string? Region { get; set; }
        public string PostalCode { get; set; }
        public bool Active {  get; set; }
        public S Visit<S>(Func<Address, S> visitor)
        {
            return visitor(this);
        }
    }
}
