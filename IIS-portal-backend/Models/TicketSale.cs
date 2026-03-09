namespace IIS_portal_backend.Models
{
    public class TicketSale
    {
        public int Id { get; set; }
        public int TicketId { get; set; }        
        public DateTime PurchaseDate { get; set; } 
        public DateTime CompetitionDay { get; set; } 
    }
}
