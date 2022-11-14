using Regex.Engine.Rules;

namespace Regex.Engine
{
    public class ContextWrapper : IContext
    {
        #region Fields

        private readonly IContext _context;
        private readonly List<char> _buffer;

        #endregion

        #region Constructors

        public ContextWrapper(IContext context)
        {
            _context = context;
            _buffer = new List<char>(_context.InputLength);
        }

        #endregion

        #region Properties

        public int InputLength => _context.InputLength;

        public char[] Result => _context.Result;

        public char? CurrentChar => _context.CurrentChar;

        public char? NextChar => _context.NextChar;

        public Match? LastCharMatch => _context.LastCharMatch;

        public bool IsFirstChar => _context.IsFirstChar;

        public int CurrentPosition => _context.CurrentPosition;

        #endregion

        #region Public Methods        

        public Checkpoint CreateCheckpoint()
        {
            return _context.CreateCheckpoint();
        }

        public bool Read() => _context.Read();

        public bool CanRead() => _context.CanRead();

        public void ConfirmMatch() 
        { 
            _context.ConfirmMatch();
            UpdateBuffer(_context.CurrentChar);
        }

        public void UpdateResult(char c)
        {
            _context.UpdateResult(c);
        }

        public void DropMatchMemo()
        {
            _buffer.Clear();
        }

        public char[] ReadLastMatch()
        {
            var match = _buffer.ToArray();
            _buffer.Clear();
            return match;
        }

        #endregion

        #region Private Methods

        private void UpdateBuffer(char? c)
        {
            if (c.HasValue)
            {
                _buffer.Add(c.Value);
            }
        }        

        #endregion

    }
}
