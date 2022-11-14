using Regex.Engine.Exceptions;
using Regex.Engine.Rules;

namespace Regex.Engine
{
    public class Context : IContext
    {
        #region Fields

        private int _currentCharIndex;
        private Match? _lastMatch;
        private readonly char[] _input;
        private readonly List<char> _result;
        private readonly Stack<Checkpoint> _checkpoints = new();

        #endregion

        #region Constructors

        public Context(char[] input)
        {
            _input = input;
            _currentCharIndex = -1;
            _result = new List<char>(_input.Length);
        }

        #endregion

        #region Properties

        public int InputLength => _input.Length;

        public char[] Result => _result.ToArray();

        public char? CurrentChar => 0 <= _currentCharIndex && _currentCharIndex < _input.Length ? _input[_currentCharIndex] : null;

        public char? NextChar => CanRead() ? _input[_currentCharIndex + 1] : null;

        public Match? LastCharMatch => _lastMatch;

        public bool IsFirstChar => 0 == _currentCharIndex;

        public int CurrentPosition => _currentCharIndex;

        #endregion

        #region Public Methods

        public bool Read()
        {
            return ++_currentCharIndex < _input.Length;
        }

        public bool CanRead()
        {
            return _currentCharIndex + 1 < _input.Length;
        }

        public void ConfirmMatch()
        {
            if (!CurrentChar.HasValue) 
                throw new ContextException($"Can not confirm match on position {CurrentPosition} because current char is null.");

            if (_lastMatch is not null)
            {
                _lastMatch.Set(CurrentPosition, CurrentChar!.Value);
            }
            else
            {
                _lastMatch = new Match(CurrentPosition, CurrentChar!.Value);
            }
        }

        public void UpdateResult(char c)
        {
            _result.Add(c);
        }

        public Checkpoint CreateCheckpoint()
        {
            var checkpoint = new Checkpoint(_currentCharIndex);

            checkpoint.Committed += OnCheckpointCommitted;
            checkpoint.Disposed += OnCheckpointDisposed;

            _checkpoints.Push(checkpoint);
            return checkpoint;
        }

        #endregion

        #region Private Methods

        private void OnCheckpointCommitted(object? sender, EventArgs args)
        {
            if (_checkpoints.Any())
            {
                var checkpoint = _checkpoints.Pop();

                if ((sender as Checkpoint)!.Position != checkpoint.Position) 
                    throw new Exception("Trying to commit checkpoint that doesn't not match last created checkpoint");

                DisposeCheckpoint(checkpoint);
            }
        }

        private void OnCheckpointDisposed(object? sender, EventArgs args)
        {
            if (_checkpoints.Any())
            {
                var checkpoint = _checkpoints.Pop();

                if ((sender as Checkpoint)!.Position != checkpoint.Position)
                    throw new Exception("Trying to dispose checkpoint that doesn't not match last created checkpoint");

                _currentCharIndex = checkpoint.Position;
                DisposeCheckpoint(checkpoint);
            }
        }

        private void DisposeCheckpoint(Checkpoint checkpoint)
        {
            checkpoint.Committed -= OnCheckpointCommitted;
            checkpoint.Disposed -= OnCheckpointDisposed;
        }

        #endregion
    }
}
