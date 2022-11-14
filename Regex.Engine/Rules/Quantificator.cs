namespace Regex.Engine.Rules
{
    public struct Quantificator
    {
        #region Constructors

        public Quantificator()
        {
            From = 1;
            To = 1;
        }

        public Quantificator(int? from, int? to)
        {
            From = from;
            To = to;
        }

        #endregion

        #region Properties

        public int? From { get; set; }

        public int? To { get; set; }

        #endregion

        #region Public Methods

        public bool Validate(int quantity)
        {
            return (!From.HasValue || quantity >= From.Value) && (!To.HasValue || quantity <= To.Value);
        }

        public bool Satisfy(int quantity)
        {
            return !To.HasValue || To.Value >= quantity;
        }

        #endregion
    }
}
