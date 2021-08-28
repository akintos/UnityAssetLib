using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityAssetLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bundle = AssetBundleFile.Open("en.bundle"))
            {
                Console.WriteLine(bundle.CompressionType);
            }
        }
    }
}
