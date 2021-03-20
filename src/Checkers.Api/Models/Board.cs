namespace WSTest.Api.Models
{
    public class Board
    {
        public int Number { get; private set; }

        public Board()
        {
            Number = 0;
        }

        public void IncrementNumber()
        {
            Number++;
        }
    }
}
