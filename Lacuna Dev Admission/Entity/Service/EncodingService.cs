using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna_Dev_Admission.Entity.Service
{
    internal class EncodingService
    {
        public string StringToBinaryBase64(string inputStrand)
        {
            byte[] binaryData = Convert.FromBase64String(inputStrand);
            StringBuilder decodedStrand = new();
            for (int i = 0; i < binaryData.Length; i++)
            {
                for (int j = 6; j >= 0; j -= 2)
                {
                    int nucleobase = (binaryData[i] >> j) & 0b11;
                    switch (nucleobase)
                    {
                        case 0b00:
                            decodedStrand.Append("A");
                            break;
                        case 0b01:
                            decodedStrand.Append("C");
                            break;
                        case 0b10:
                            decodedStrand.Append("G");
                            break;
                        case 0b11:
                            decodedStrand.Append("T");
                            break;
                    }
                }
            }
            return decodedStrand.ToString();
        }
    }
}
