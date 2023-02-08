using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna_Dev_Admission.Entity.Service
{
    internal class EncodingService
    {
        public string DecodeBase64Strand(string encodedStrand)
        {
            byte[] data = Convert.FromBase64String(encodedStrand);
            return Encoding.UTF8.GetString(data);
        }
    }
}
