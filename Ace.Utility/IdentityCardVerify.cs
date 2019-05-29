using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ace.Utility
{
    /// <summary>
    /// 身份证验证
    /// </summary>
    public class IdentityCardVerify
    {
        // 身份证前17位每位加权因子
        private static readonly int[] Power = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

        // 身份证第18位校检码
        private static readonly string[] RefNumber = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };

        // 18位二代身份证号码的正则表达式
        public static readonly string RegexIdNo18 = "^"
                                                       + "\\d{6}" // 6位地区码
                                                       + "(18|19|([23]\\d))\\d{2}" // 年YYYY
                                                       + "((0[1-9])|(10|11|12))" // 月MM
                                                       + "(([0-2][1-9])|10|20|30|31)" // 日DD
                                                       + "\\d{3}" // 3位顺序码
                                                       + "[0-9Xx]" // 校验码
                                                       + "$";

        // 15位一代身份证号码的正则表达式
        public static readonly string RegexIdNo15 = "^"
                                                       + "\\d{6}" // 6位地区码
                                                       + "\\d{2}" // 年YYYY
                                                       + "((0[1-9])|(10|11|12))" // 月MM
                                                       + "(([0-2][1-9])|10|20|30|31)" // 日DD
                                                       + "\\d{3}" // 3位顺序码
                                                       + "$";
        /// <summary>
        /// 校验身份证号码
        /// </summary>
        /// <param name="idNo">身份证号码</param>
        /// <returns></returns>
        public static bool CheckIdNo(string idNo)
        {
            if (idNo.Length == 15)
                return Regex.Match(idNo, RegexIdNo15).Success;
            else if (idNo.Length == 18)
                return Regex.Match(idNo, RegexIdNo18).Success;
            else
                return false;
        }

        /// <summary>
        /// 计算身份证的第十八位校验码
        /// </summary>
        /// <param name="idNo">身份证号码</param>
        /// <returns></returns>
        public static string GetCheckCode(string idNo)
        {
            char[] tmp = idNo.ToCharArray();
            int[] cardidArray = new int[tmp.Length - 1];
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                cardidArray[i] = int.Parse(tmp[i] + "");
            }
            return GetCheckCode(cardidArray);
        }

        /// <summary>
        /// 计算身份证的第十八位校验码
        /// </summary>
        /// <param name="idNoArray">身份证号码数字组</param>
        /// <returns></returns>
        public static string GetCheckCode(int[] idNoArray)
        {
            int result = 0;
            for (int i = 0; i < Power.Length; i++)
            {
                result += Power[i] * idNoArray[i];
            }
            return RefNumber[result % 11];
        }

        /// <summary>
        /// 校验身份证第18位是否正确
        /// </summary>
        /// <param name="idNo">身份证号码</param>
        /// <returns></returns>
        public static bool ValidateCheckNumber(string idNo)
        {
            if (idNo.Length != 15 && idNo.Length != 18)
            {
                return false;
            }

            if (idNo.Length == 15)
                idNo = ChangeIdNo15To18(idNo);

            string checkCode = GetCheckCode(idNo);
            string lastNum = idNo.Substring(idNo.Length - 1, 1);
            if (lastNum.Equals("x", StringComparison.OrdinalIgnoreCase))
            {
                lastNum = lastNum.ToUpper();
            }
            if (!checkCode.Equals(lastNum, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 15位一代身份证号码升级18位二代身份证号码
        /// 第一代身份证十五位数升为第二代身份证十八位数的一般规则是：
        /// 第一步，在原十五位数身份证的第六位数后面插入19 ，这样身份证号码即为十七位数；
        /// 第二步，按照国家规定的统一公式计算出第十八位数，作为校验码放在第二代身份证的尾号
        /// </summary>
        /// <param name="idNo">身份证号码</param>
        /// <returns></returns>
        public static string ChangeIdNo15To18(string idNo)
        {
            if (idNo.Length == 15)
            {
                idNo = idNo.Substring(0, 6) + "19" + idNo.Substring(6, idNo.Length - 6);
            }
            //后面补位，达到18位长度
            string tempIdNo = idNo + "x";
            //返回本体码+校验码=完整的身份证号码
            return idNo + GetCheckCode(tempIdNo);
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <param name="idNo">身份证号码</param>
        /// <returns></returns>
        public static string GetSex(string idNo)
        {
            string strSex = string.Empty;
            if (idNo.Length == 18)
            {
                strSex = idNo.Substring(14, 3);
            }
            else if (idNo.Length == 15)
            {
                strSex = idNo.Substring(12, 3);
            }
            else
                return strSex;
            //性别代码为偶数是女性奇数为男性
            return int.Parse(strSex) % 2 == 0 ? "女" : "男";
        }

        /// <summary>
        /// 获取出生年月
        /// </summary>
        /// <param name="idNo">身份证号码</param>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static string GetBirthday(string idNo, string format = "yyyy-MM-dd")
        {
            string birthday = string.Empty;
            if (idNo.Length == 18)
            {
                birthday = idNo.Substring(6, 4) + "-" + idNo.Substring(10, 2) + "-" + idNo.Substring(12, 2);
                return DateTime.Parse(birthday).ToString(format);
            }
            else if (idNo.Length == 15)
            {
                birthday = "19" + idNo.Substring(6, 2) + "-" + idNo.Substring(8, 2) + "-" + idNo.Substring(10, 2);
                return DateTime.Parse(birthday).ToString(format);
            }
            return birthday;
        }

        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthDay">出生日期</param>
        /// <returns></returns>
        public static int GetAge(string birthDay)
        {
            DateTime birthDate = DateTime.Parse(birthDay);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }
    }
}
