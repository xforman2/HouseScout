using System;

namespace HouseScout.model
{
    public class Estate
    {
        public int Id { get; set; }
        public int Api { get; set; }
        public string ApiId { get; set; }
        public int Address { get; set; }
        public double Price { get; set; }
        public double EnergyPrice { get; set; }
        public string Link { get; set; }
        public double Surface { get; set; }
        public int EstateType { get; set; }
        public int OfferType { get; set; }

        public Estate()
        {
            var random = new Random();

            Id = random.Next(1, 1000);
            Api = random.Next(1, 10);
            ApiId = Guid.NewGuid().ToString();
            Address = random.Next(1000, 9999);
            Price = Math.Round(random.NextDouble() * 1000000, 2);
            EnergyPrice = Math.Round(random.NextDouble() * 1000, 2);
            Link = $"https://example.com/estate/{Id}";
            Surface = Math.Round(random.NextDouble() * 1000, 2);
            EstateType = random.Next(1, 5);
            OfferType = random.Next(1, 3);
        }
    }
}