using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Contact.API.Models
{
    /// <summary>
    /// 好友申请记录
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ContactApplyRequest
    {
        /// <summary>
        /// 申请人
        /// </summary>
        public int ApplierId { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 0未处理 1已通过
        /// </summary>
        public int Approvaled { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime HandledTime { get; set; }

        /// <summary>
        /// 联系人Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string Name { get; set; }
    }
}
