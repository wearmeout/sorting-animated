using FractionalSorting.Visualization.Data;
using System.Linq;
using Xunit;

namespace FractionalSorting.Visualization.Test.Data
{
    public class ExchangeSortsProviderTest
    {
        [Fact]
        public void ProvideAtLeastOneSort()
        {
            IAlgorithmsProvider provider = new ExchangeSortsProvider();
            int[] list = new[] { 2, 1, 3 };

            var sorts = provider.Create(list);

            Assert.NotEmpty(sorts);
            Assert.Equal(list, sorts.First().Source);
        }
    }
}
