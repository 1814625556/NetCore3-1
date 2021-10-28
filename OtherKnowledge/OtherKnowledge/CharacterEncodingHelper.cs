using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherKnowledge
{
    public class CharacterEncodingHelper
    {
        /// <summary>
        /// test method
        /// </summary>
        public static void ConsoleTest()
        {
            var buf = CharacterEncodingHelper.CharacterToAscii(" AaBbCcDd123`~/?.,;'");
            var str = CharacterEncodingHelper.AsciiToCharacter(buf);
        }

        /// <summary>
        /// 字符串转ascii
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static byte[] CharacterToAscii(string xmlStr)
        {
            return Encoding.Default.GetBytes(xmlStr);
        }

        public static string AsciiToCharacter(byte[] buf)
        {
            return System.Text.Encoding.ASCII.GetString(buf);
        }
    }
}
