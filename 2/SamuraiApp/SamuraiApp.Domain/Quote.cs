namespace SamuraiApp.Domain
{
    public class Quote
    {
        public int Id { get; set; }
        public string Text { get; set; }

        //Reference property back to Samurai
        public Samurai Samurai { get; set; }

        //Foreign Key
        public int SamuraiId { get; set; }
    }
}
