using Project.Domain.Seedwork;
using System.Collections.Generic;

namespace Project.Domain.AggregatesModel.ProjectAggregate
{
    /// <summary>
    /// 项目属性
    /// </summary>
    public class ProjectProperty : ValueObject
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Key;
            yield return Text;
            yield return Value;
        }
    }
}
