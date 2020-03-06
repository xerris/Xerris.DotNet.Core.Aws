namespace Xerris.DotNet.Core.Aws.IoC
{
    public interface ILazyProvider<out TProvider> 
    {
        TProvider Create();
    }
}