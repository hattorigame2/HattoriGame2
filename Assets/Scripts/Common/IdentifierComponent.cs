using Mono.Security;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HattoriGame2
{
    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct Guid
    {
        [SerializeField]
        private uint a;
        [SerializeField]
        private ushort b;
        [SerializeField]
        private ushort c;
        [SerializeField]
        private byte d;
        [SerializeField]
        private byte e;
        [SerializeField]
        private byte f;
        [SerializeField]
        private byte g;
        [SerializeField]
        private byte h;
        [SerializeField]
        private byte i;
        [SerializeField]
        private byte j;
        [SerializeField]
        private byte k;
  
        public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
            this.h = h;
            this.i = i;
            this.j = j;
            this.k = k;
        }

        public Guid(byte[] guidBytes) : this( BitConverter.ToUInt32(guidBytes, 0), BitConverter.ToUInt16(guidBytes, 4), BitConverter.ToUInt16(guidBytes, 6), guidBytes[8], guidBytes[9], guidBytes[10], guidBytes[11], guidBytes[12], guidBytes[13], guidBytes[14], guidBytes[15] )
        {
        }

        public Guid(uint a, ushort b, ushort c, byte[] d) : this ( a, b, c, d[0], d[1], d[2], d[3], d[4], d[5], d[6], d[7])
        {

        }

        public Guid( string guidString )
        {
            guidString = Regex.Replace(guidString, @"[\{\} ]", "").ToLower();

            if (!Regex.IsMatch(guidString, @"[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{16}"))
            {
                this.a = this.b = this.c = this.d = this.e = this.f = this.g = this.h = this.i = this.j = this.k = 0;        
            }
            else
            {
                var items = guidString.Split('-');

                this.a = UInt32.Parse(items[0], System.Globalization.NumberStyles.HexNumber);
                this.b = UInt16.Parse(items[1], System.Globalization.NumberStyles.HexNumber);
                this.c = UInt16.Parse(items[2], System.Globalization.NumberStyles.HexNumber);
                this.d = byte.Parse(items[3].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                this.e = byte.Parse(items[3].Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                this.f = byte.Parse(items[3].Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                this.g = byte.Parse(items[3].Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                this.h = byte.Parse(items[3].Substring(8, 2), System.Globalization.NumberStyles.HexNumber);
                this.i = byte.Parse(items[3].Substring(10, 2), System.Globalization.NumberStyles.HexNumber);
                this.j = byte.Parse(items[3].Substring(12, 2), System.Globalization.NumberStyles.HexNumber);
                this.k = byte.Parse(items[3].Substring(14, 2), System.Globalization.NumberStyles.HexNumber);
            }            
        }

        public static Guid Generate()
        {
            byte[] data = new byte[0x10];

            var RNG = RandomNumberGenerator.Create();
            RNG.GetBytes(data);
            
            return new Guid(data);
        }

        public string ToString( bool uppercase, string delimeter, bool braces )
        {
            var hexFormat = uppercase ? 'X' : 'x';

            return String.Format((braces ? "{{" : "") + "{0:" + hexFormat + "8}" + delimeter + "{1:" + hexFormat + "4}" + delimeter + "{2:" + hexFormat + "4}" + delimeter + "{3:" + hexFormat + "2}{4:" + hexFormat + "2}{5:" + hexFormat + "}{6:" + hexFormat + "2}{7:" + hexFormat + "2}{8:" + hexFormat + "2}{9:" + hexFormat + "2}{10:" + hexFormat + "2}" + (braces ? "}}" : ""), a, b, c, d, e, f, g, h, i, j, k);
        }

        public override string ToString()
        {
            return ToString(true, " - ", true);
        }

        public static bool operator ==( Guid thisGuid, Guid otherGuid )
        {
            return
                thisGuid.a == otherGuid.a &&
                thisGuid.b == otherGuid.b &&
                thisGuid.c == otherGuid.c &&
                thisGuid.d == otherGuid.d &&
                thisGuid.e == otherGuid.e &&
                thisGuid.f == otherGuid.f &&
                thisGuid.g == otherGuid.g &&
                thisGuid.h == otherGuid.h &&
                thisGuid.i == otherGuid.i &&
                thisGuid.j == otherGuid.j &&
                thisGuid.k == otherGuid.k;
        }

        public static bool operator !=(Guid thisGuid, Guid otherGuid)
        {
            return
                thisGuid.a != otherGuid.a ||
                thisGuid.b != otherGuid.b ||
                thisGuid.c != otherGuid.c ||
                thisGuid.d != otherGuid.d ||
                thisGuid.e != otherGuid.e ||
                thisGuid.f != otherGuid.f ||
                thisGuid.g != otherGuid.g ||
                thisGuid.h != otherGuid.h ||
                thisGuid.i != otherGuid.i ||
                thisGuid.j != otherGuid.j ||
                thisGuid.k != otherGuid.k;
        }
    }

    [AddComponentMenu("Hattori Game 2/Common/Identifier")]
    [DisallowMultipleComponent]
    [Serializable]
    public class IdentifierComponent : MonoBehaviour
    {
        [SerializeField]
        public Guid GUID;

       /* [SerializeField]
        public Tag Tag;*/

        public override string ToString()
        {
            return GUID.ToString();
        }
    }
}
