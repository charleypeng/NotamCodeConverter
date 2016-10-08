/// <summary>
/// NotamCodeDecoder
/// Auther: Peng Lei
/// Date: 2016-1-12
/// This Project is under the terms of LGPL
/// </summary>
using SQLite;
namespace NotamDecoder
{
    /// <summary>
    /// CodeModel
    /// </summary>
    public class CodeItem
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Chinese Character
        /// </summary>
        public string Character { get; set; }
    }
}
