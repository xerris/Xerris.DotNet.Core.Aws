namespace Xerris.DotNet.Core.Aws.Test.Model
{
    public class Foo
    {
        public Foo() { }
        
        public Foo(string first, string last, int age = 10)
        {
            FirstName = first;
            LastName = last;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}