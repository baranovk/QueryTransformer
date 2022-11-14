namespace Regex.Engine.Rules
{
    public class Checkpoint : IDisposable
    {
        #region Fields

        private bool _commited;
        private bool _disposed;
        private int _position;

        #endregion

        #region Events

        public event EventHandler<EventArgs>? Committed;
        public event EventHandler<EventArgs>? Disposed;

        #endregion

        #region Constructors

        public Checkpoint(int position)
        {
            _position = position;
        }

        #endregion

        #region Properties

        public int Position => _position;

        #endregion

        #region Public Methods

        public void Commit()
        {            
            _commited = true;
            Committed?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            if (_disposed) return;
            if (_commited) return;
            Disposed?.Invoke(this, new EventArgs());
            _disposed = true;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
