using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Contact.API.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ContactBook
    {
        public ContactBook()
        {
            Contacts = new List<Contact>();
        }

        public int UserId { get; set; }

        /// <summary>
        /// 联系人列表
        /// </summary>
        public List<Contact> Contacts { get; set; }
    }
}
