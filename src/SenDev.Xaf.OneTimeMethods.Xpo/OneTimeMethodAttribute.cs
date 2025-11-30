namespace SenDev.Xaf.OneTimeMethods;

[AttributeUsage(AttributeTargets.Method)]
public sealed class OneTimeMethodAttribute : Attribute
{

    public OneTimeMethodAttribute()
    {
        
    }

    public OneTimeMethodAttribute(OneTimeMethodExecutionSequence sequence)
    {
        Sequence = sequence;
    }

    public OneTimeMethodExecutionSequence Sequence { get; set; } = OneTimeMethodExecutionSequence.AfterSchemaUpdate;
    
    public Type? ConnectionType { get; set; }
}
