using OpenCvSharp;
using VisionSharp.Models.Category;
using VisionSharp.Models.Detect;
using Xunit.Abstractions;

namespace UnitTest.ModelsTest
{
    public class CategoryTest : AbstractTest
    {
        public CategoryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void TestObjRect()
        {
            var objRect = new ObjRect<VocCategory>(VocCategory.Aeroplane, new Rect());
            PrintObject(objRect);
        }
    }
}