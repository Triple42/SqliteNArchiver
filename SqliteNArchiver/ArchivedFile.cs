using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteNArchiver
{
    public class ArchivedFile
    {
        public required long RowId { get; init; }
        public required string FileName { get; init; }
        public required string Path { get; init; }
        public required byte[] Hash { get; init; }
        public required int PieceLength { get; init; }

        Span<byte> GetData(int offset, int length)
        {
            var b = new byte[length];
            //TODO
            return new Span<byte>(b);
        }
    }
}
