namespace GAS.RuntimeWithECS.Core
{
    public class TurnController
    {
        private long _currentTurn;
        public long CurrentTurn => _currentTurn;
        
        public void ResetTurn()
        {
            _currentTurn = 0;
        }
        
        public void NextTurn()
        {
            _currentTurn++;
        }
        
        public void PreviousTurn()
        {
            _currentTurn--;
        }
        
        public void SetTurn(long turn)
        {
            _currentTurn = turn;
        }
    }
}