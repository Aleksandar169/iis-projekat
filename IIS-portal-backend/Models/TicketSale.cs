using System.ComponentModel.DataAnnotations;

public class TicketSale
{
    [Key] // Primarni ključ (autoincrement)
    public int Id { get; set; }

    public int TicketId { get; set; } // ID karte iz glavnog sistema (A1)

    public DateTime PurchaseDate { get; set; } // Kada je kupljena

    public DateTime CompetitionDay { get; set; } // Za koji konkretan dan trke važi ovaj red
}
