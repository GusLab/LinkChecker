using System;
using System.Linq;
using System.Text;
using PunyCode.Helper;

namespace PunyCode
{
    public sealed class PunyCodeConverterImpl : IPunycodeConverter, IDisposable
    {
        private PunyCodeConverterHelper _punyCodeConverterHelper;
        private bool _disposed;

        public PunyCodeConverterImpl()
        {
            _punyCodeConverterHelper = new PunyCodeConverterHelper();
            _disposed = false;
        }

        public string ForcedUnicodeToPunyCode(string inputString)
        {
            var b = Encoding.Unicode.GetBytes(inputString);
            inputString = Encoding.Unicode.GetString(b);
            var outputLenght = (uint)(PunyCodeStatic.MaxInputStringLenght);
            b = new byte[PunyCodeStatic.MaxInputStringLenght];
            var status = _punyCodeConverterHelper.PunycodeEncode((uint)inputString.Length, inputString, out outputLenght, out b);

            inputString = "";

            if (status != PunyCodeStatic.OperationStatus.Success) return inputString.ToLower();
            for (var i = 0; i < b.Length; i++)
            {
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

        public string ForcedPunyCodeToUnicode(string aInput)
        {
            try
            {
                var b = Encoding.ASCII.GetBytes(aInput);
                var outputLenght = (uint)(PunyCodeStatic.MaxInputStringLenght);
                char[] c;
                var status = _punyCodeConverterHelper.PunycodeDecode((uint)aInput.Length, b, out outputLenght, out c, null);
                aInput = "";
                if (status == PunyCodeStatic.OperationStatus.Success)
                {
                    aInput = c.Where(t => t != 0).Aggregate(aInput, (current, t) => current + t);
                }
            }
            catch (Exception e)
            {
                throw new PunyCodeDecodeException("Decoding from punycode failed for:" + aInput,e);
            }
            return aInput.ToLower();

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
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
