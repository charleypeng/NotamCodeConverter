/// <summary>
/// NotamCodeDecoder
/// Auther: Peng Lei
/// Date: 2016-1-12
/// This Project is under the terms of LGPL
/// </summary>
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using System.Threading.Tasks;

namespace NotamDecoder
{
    /// <summary>
    /// Notam Code Decoder
    /// </summary>
    public class CodeConverter
    {
        /// <summary>
        /// Database file path
        /// </summary>
        public string DBPath { get; set; } = Directory.GetCurrentDirectory() + @"\CodeData.db";

        /// <summary>
        /// Return the Chinese Character
        /// *Async
        /// </summary>
        /// <param name="scode">4 digi code</param>
        /// <returns>Chinese character</returns>
        public virtual async Task<string> GetCharacterAsync(string scode)
        {
            if (string.IsNullOrWhiteSpace(scode)) return string.Empty;

            var _scode = scode.Trim();
            var conn = new SQLiteAsyncConnection(DBPath);
            string data = string.Empty;

            if (!File.Exists(DBPath)) return scode;

            try
            {
                var lst = await conn.Table<CodeItem>().Where(x => x.Code == _scode).ToListAsync();

                if (lst == null) return scode;

                data = lst.FirstOrDefault().Character;
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return data;
        }

        /// <summary>
        /// To confirm the characters are all ASCSII  numbers
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool isNum(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' || str[i] > '9')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// To confirm wheather it is a Notam Code
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        private bool isNotamCode(string Code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Code)) return false;
                else
                {
                    Code = Code.Trim();
                    if (isNum(Code) && Code.Count() == 4) return true; else return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Remove a given character
        /// </summary>
        /// <param name="StringData">String Data</param>
        /// <param name="Symbol">Symbol for removal</param>
        /// <returns>String</returns>
        private string removeStringSymbol(string StringData, char Symbol)
        {
            if (string.IsNullOrWhiteSpace(StringData)) return StringData;

            if (!StringData.Contains(Symbol.ToString())) return StringData;
            else
            {
                var lst = new List<string>();

                foreach (var item in StringData)
                {
                    if (Symbol != item)
                    {
                        lst.Add(item.ToString());
                    }
                }

                var stringData = string.Empty;
                foreach (var str in lst)
                {
                    stringData = stringData + str;
                }

                return stringData;
            }
        }

        /*
                private bool isFullNotam(string notamString)
                {
                    if (string.IsNullOrWhiteSpace(notamString)) return false;
                    try
                    {
                        notamString = RemoveStringSymbol(notamString, '\n').Trim();
                        if (notamString.Contains("NNNN")) return true;
                        else
                            return false;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                public string DecodeFullNotam(string FullNotamString)
                {
                    if (string.IsNullOrWhiteSpace(FullNotamString)) return FullNotamString;
                    if (!isFullNotam(FullNotamString)) return FullNotamString;
                    try
                    {
                        FullNotamString = FullNotamString.Trim();
                        var strs = FullNotamString.Split('\n');
                        int posA = 0;
                        int posB = 0;
                        FullNotamString.IndexOf('(');
                        for (int i = 0; i < strs.Count(); i++)
                        {
                            if (strs[i].ToUpper().Contains("E)"))
                                posA = i;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
        */

        /// <summary>
        /// Decode the given NOTAM string
        /// only suitable for the Notam E section at present
        /// use it in other phrase may cause abnormal display of NOTAM
        /// </summary>
        /// <param name="NotamString">NOTAM String</param>
        /// <returns>Decoded Notam String for Chinese character</returns>
        public async Task<string> DecodeNotamAsync(string NotamString)
        {
            //TODO add bool to confirm it is NOTAM E section or not.
            string strData = string.Empty;
            if (string.IsNullOrWhiteSpace(NotamString)) return string.Empty;
            try
            {
                string[] strlst = null;
                //to confirm the Notam Contains "\r\n"

                if (NotamString.Contains('\n') || NotamString.Contains('\r'))
                {
                    if (NotamString.Contains('\n')) strlst = NotamString.Split('\n');
                    if (NotamString.Contains('\r')) strlst = NotamString.Split('\r');

                    foreach (var lstItem in strlst)
                    {
                        var strs = lstItem.Split(' ');
                        foreach (var str in strs)
                        {
                            var item = str;
                            if (isNotamCode(str))
                            {
                                item = await GetCharacterAsync(str); // return Chinese character
                            }
                            strData = strData + item + " ";  //form into a line
                        }
                        strData = strData + Environment.NewLine; // form into Notam Text
                    }
                }
                else
                {
                    var strs = NotamString.Split(' ');
                    foreach (var str in strs)
                    {
                        var item = str;
                        if (isNotamCode(item)) item = await GetCharacterAsync(item); // Return Chinese character
                        strData = strData + item + " ";
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("error when decode the Notam code", ex);
            }
            return strData;
        }
    }
}
