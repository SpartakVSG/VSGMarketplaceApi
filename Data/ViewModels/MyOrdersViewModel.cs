﻿namespace Data.ViewModels
{
    public class MyOrdersViewModel
    {
        public int Id { get; set; }

        public string ItemCode { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public double OrderPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
    }
}
