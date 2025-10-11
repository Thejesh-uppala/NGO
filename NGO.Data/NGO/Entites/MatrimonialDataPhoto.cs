using System.ComponentModel.DataAnnotations;

namespace NGO.Data
{
    public partial class MatrimonialDataPhoto
    {
        [Key]
        public int Id { get; set; }

        public int MatrimonailId { get; set; }
        public string PhotoType { get; set; }
        public string PhotoPath { get; set; }

        public virtual MatrimonialData MatrimonialData { get; set; } = null!;
    }

}


