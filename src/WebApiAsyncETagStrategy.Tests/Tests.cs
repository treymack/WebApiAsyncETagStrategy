using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace WebApiAsyncETagStrategy.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Pass()
        {
            true.Should().Be(true);
        }

        [Test]
        public void Fail()
        {
            //true.Should().Be(false);
        }
    }
}
