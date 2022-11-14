using Regex.Engine.Rules;

namespace Regex.Engine
{
    /// <summary>
    /// Represents context for current text transformation
    /// </summary>
    public interface IContext
    {
        #region Properties

        // We can read transformation result from here
        char[] Result { get; }

        // We move through input character by character
        // CurrentPosition shows current pointer position inside input string
        int CurrentPosition { get; }

        // Current character inside input string at which pointer is pointing
        char? CurrentChar { get; }

        // Next char that will be processed 
        char? NextChar { get; }

        // Shows last character match that was confirmed by pattern rule 
        Match? LastCharMatch { get; }

        // Length of the input string that is being processed
        int InputLength { get; } 

        // Is context pointing to the first character in input string?
        bool IsFirstChar { get; }        

        #endregion

        // Try to move to next character in the input string and return false
        // if we outside of the input's bounds
        bool Read();

        // Check if we have any character ahead CurrentPosition 
        bool CanRead();

        // Pattern rules can call this method, so we can update LastCharMatch 
        void ConfirmMatch();

        // Put char to result buffer
        void UpdateResult(char c);

        // We can create checkpoints while parsing input.
        // So we can return pointer to checkpoint position if match attempt 
        // was unsuccessful.
        Checkpoint CreateCheckpoint();
    }
}
