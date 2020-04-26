using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace User.API.UnitTest
{
    public class XUnitTest
    {
        private readonly ITestOutputHelper _output;
        public XUnitTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "忽略这个测试")]
        [Trait("Group", "A")]//分组
        public void GroupA()
        {

        }

        [Theory]//执行多次
        [InlineData(0, 99)]
        [InlineData(1, 100)]
        [MemberData(nameof(InternalHealthDamageTestData.TestData), MemberType = typeof(InternalHealthDamageTestData))]//方法获取数据
        [HealthDamageData]//特性获取数据
        [Trait("Group", "B")]//分组
        public void GroupB(int damage, int expectedHealth)
        {
            _output.WriteLine("打印 正在测试 Group B");
            Assert.True(true);
        }
    }

    public class InternalHealthDamageTestData
    {
        private static readonly List<object[]> Data = new List<object[]>
        {
            new object[] {0, 100},
            new object[] {1, 99},
            new object[] {50, 50},
            new object[] {101, 1}
        };

        public static IEnumerable<object[]> TestData => Data;
    }

    public class HealthDamageDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { 0, 100 };
            yield return new object[] { 1, 99 };
            yield return new object[] { 50, 50 };
            yield return new object[] { 101, 1 };
        }
    }
}
