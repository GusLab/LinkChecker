using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PunyCode.Helper;

namespace PunyCode
{
    public class PunyCodeConverterImpl : IPunycodeConverter, IDisposable
    {
        private PunyCodeConverterHelper _punyCodeConverterHelper;
        private bool _disposed;

        public PunyCodeConverterImpl()
        {
            _punyCodeConverterHelper = new PunyCodeConverterHelper();
            _disposed = false;
        }

        /// <summary>
        /// Encodes Unicode to Punicode (forces unicode on string)
        /// </summary>
        /// <param name="inputString">input string</param>
        static public string Encode(string inputString)
        {
            // convert to ascii
            var b = Encoding.Unicode.GetBytes(inputString);
            inputString = Encoding.Unicode.GetString(b);
            var outputLenght = (uint)(PunyCodeStatic.MaxInputStringLenght);
            b = new byte[PunyCodeStatic.MaxInputStringLenght];
            var status = PunycodeEncode((uint)inputString.Length, inputString, null, out outputLenght, out b);

            inputString = "";

            if (status != PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusSuccess) return inputString.ToLower();
            for (var i = 0; i < b.Length; i++)
            {
                //fix numbers
                if (b[i] >= 16 && b[i] <= 25)
                {
                    b[i] += 32;
                }
                if (b[i] != 0)
                {
                    inputString += (char)b[i];
                }
            }
            return inputString.ToLower();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _punyCodeConverterHelper.Dispose();
                _punyCodeConverterHelper = null;
            }

            _disposed = true;
        }

        ~PunyCodeConverterImpl()
        {
            Dispose(false);
        }
    }
}
