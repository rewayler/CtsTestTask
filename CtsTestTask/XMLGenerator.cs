using CTSTestApplication;
namespace CtsTestTask
{
    public class XMLGenerator
    {
        public void GenerateTestXml(string path, int count) => (new Tester()).CreateTestFile(path, count);
    }
}