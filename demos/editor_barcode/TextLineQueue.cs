using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demos
{
    /// <summary>
    /// Read text file and read first line and remove it
    /// </summary>
    public sealed class TextLineQueueLite
    {
        private readonly string _path;

        public TextLineQueueLite(string path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public bool TryDequeue(out string line, int bufferSize = 64 * 1024)
        {
            line = null;

            if (!File.Exists(_path))
                return false;

            // 독점 잠금: 처리 중에만 외부 편집 차단. 처리 끝나면 즉시 해제됨.
            using var fs = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            if (fs.Length == 0)
                return false;

            // 1) BOM(UTF-8) 확인
            long readPos = 0;
            byte[] bom = new byte[3];
            int read = fs.Read(bom, 0, 3);

            if (read == 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
            {
                readPos = 3;          // BOM 건너뜀
            }
            else
            {
                fs.Position = 0;      // BOM 아님 → 처음으로 되돌림
                readPos = 0;
            }

            // 2) 첫 줄 경계(\n) 탐색 + 첫 줄 바이트 수집
            using var firstLineBuf = new MemoryStream(capacity: 256);
            int b;
            bool foundNewLine = false;
            while ((b = fs.ReadByte()) != -1)
            {
                readPos++;
                if (b == (byte)'\n')
                {
                    foundNewLine = true;
                    break;
                }
                firstLineBuf.WriteByte((byte)b);
            }

            // 파일이 비어있거나(첫 바이트부터 EOF) 줄 없음(마지막 줄 끝에 \n 없음)도 허용:
            // - foundNewLine == false 이면 파일 전체가 첫 줄이 됨
            if (firstLineBuf.Length == 0 && !foundNewLine)
            {
                // 내용이 전혀 없으면 false
                if (fs.Length == 0) return false;
            }

            // 3) 반환 문자열 만들기 (CRLF 처리)
            var raw = firstLineBuf.ToArray();
            if (raw.Length > 0 && raw.Last() == (byte)'\r') // CR 제거
            {
                Array.Resize(ref raw, raw.Length - 1);
            }
            line = Encoding.UTF8.GetString(raw);

            // 4) 나머지 내용을 파일 앞쪽으로 "블록 이동" (in-place)
            //    readPos 는 현재 "첫 줄 + 개행" 이후 바이트 시작 위치
            //    (개행이 없으면 파일 끝이므로 아래 루프는 0바이트만 처리)
            long writePos = (readPos >= fs.Length) ? 0 : 0; // 항상 0부터 덮어씀
            byte[] buf = new byte[Math.Max(4 * 1024, bufferSize)];
            long srcPos = readPos;

            while (srcPos < fs.Length)
            {
                fs.Position = srcPos;
                int n = fs.Read(buf, 0, buf.Length);
                if (n <= 0) break;
                srcPos += n;

                fs.Position = writePos;
                fs.Write(buf, 0, n);
                writePos += n;
            }

            // 5) 새 길이로 잘라서 첫 줄 제거 완료
            fs.SetLength(writePos);

            return true;
        }
    }

}