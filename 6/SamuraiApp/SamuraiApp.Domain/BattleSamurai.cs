using System;

namespace SamuraiApp.Domain
{
    public class BattleSamurai
    {
        public int SamuraiId { get; set; }
        public int BattleId { get; set; }
        
        //Pay-load
        public DateTime DateJoined { get; set; }
    }
}
