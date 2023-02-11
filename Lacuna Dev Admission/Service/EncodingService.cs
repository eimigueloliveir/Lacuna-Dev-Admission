using System.Text;

namespace Lacuna_Dev_Admission.Service
{
    internal class EncodingService
    {
        public string StringBase64ToBinaryString(string inputStrand)
        {
            byte[] valueBytes = Convert.FromBase64String(inputStrand);
            string decodedString = Encoding.UTF8.GetString(valueBytes);
            string binaryString = string.Join("", valueBytes.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
            Dictionary<string, string> dictionary = new() { { "11", "T" }, { "00", "A" },
                                                  { "01", "C" }, { "10", "G" } };

            StringBuilder resultBuilder = new();
            for (int i = 0; i < binaryString.Length; i += 2)
            {
                string pair = binaryString.Substring(i, 2);
                resultBuilder.Append(dictionary[pair]);
            }
            return resultBuilder.ToString();
        }

        public string BinaryStringToString(string inputStrand)
        {
            var dict = new Dictionary<char, string>() { { 'T', "11" }, { 'A', "00" },
                                                  { 'C', "01" }, { 'G', "10" } };

            string b = string.Concat(inputStrand.Select(c => dict[c]));

            byte[] valueBytes = Enumerable.Range(0, b.Length / 8).Select(i => Convert.ToByte(b.Substring(i * 8, 8), 2)).ToArray();
            return Convert.ToBase64String(valueBytes);
        }
    }
}
