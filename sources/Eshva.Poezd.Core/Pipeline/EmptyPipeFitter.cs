namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Pipe fitter producing no steps.
  /// </summary>
  internal class EmptyPipeFitter : IPipeFitter
  {
    /// <inheritdoc />
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class { }
  }
}
