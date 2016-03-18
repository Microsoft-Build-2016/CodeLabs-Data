// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace PartsUnlimited.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        
        public System.DateTime OrderDate { get; set; }
        
        public string Username { get; set; }
        
        public string Name { get; set; }
        
        public string Address { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        public string PostalCode { get; set; }

        public string Country { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }
        
        public decimal Total { get; set; }
        
        public bool Processed { get; set; }
        
        public List<OrderDetail> OrderDetails { get; set; }
    }
}