
namespace Crosswords
{
    public class Expansion
    {
        public int Left = 0;
        public int Right = 0;
        public int Up = 0;
        public int Down = 0;

        public int TotalX => Left + Right;
        public int TotalY => Up + Down;
    }
    
}
