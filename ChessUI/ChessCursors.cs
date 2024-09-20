using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ChessUI
{
    public static class ChessCursors
    {
        // Defines a static cursor for white pieces by loading it from a file
        public static readonly Cursor WhiteCursor = LoadCursor("Assets/CursorW.cur");

        // Defines a static cursor for black pieces by loading it from a file
        public static readonly Cursor BlackCursor = LoadCursor("Assets/CursorB.cur");

        // Private method to load a cursor from a specified file path
        private static Cursor LoadCursor(string filePath)
        {
            // Loads a stream from the file path specified and returns a new Cursor
            Stream stream = Application.GetResourceStream(new Uri(filePath, UriKind.Relative)).Stream;
            return new Cursor(stream, true); // 'true' indicates disposal of the stream
        }
    }
}
