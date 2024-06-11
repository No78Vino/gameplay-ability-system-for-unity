using System;
using System.IO;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public sealed class IndentedWriter : IDisposable
    {
        private StreamWriter _streamWriter;

        public IndentedWriter(StreamWriter streamWriter, int indent = 0, int spaces = 4)
        {
            _streamWriter = streamWriter;
            Indent = indent;
            Spaces = spaces;
        }

        public int Spaces { get; set; }

        public int Indent { get; set; }

        public string GetIndentation()
        {
            return new string(' ', Math.Max(0, Indent * Spaces));
        }

        public void Write(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                _streamWriter.Write(text);
            else
                _streamWriter.Write(GetIndentation() + text);
        }

        public void WriteLine(string line = null, bool ignoreIndent = false)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                _streamWriter.WriteLine();
            }
            else
            {
                if (ignoreIndent)
                {
                    _streamWriter.WriteLine(line);
                }
                else
                {
                    _streamWriter.WriteLine(GetIndentation() + line);
                }
            }
        }

        private bool _isDisposed;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                _streamWriter.Dispose();
                _streamWriter = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
#endif