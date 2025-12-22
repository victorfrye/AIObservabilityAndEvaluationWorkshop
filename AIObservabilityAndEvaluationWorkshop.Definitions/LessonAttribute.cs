namespace AIObservabilityAndEvaluationWorkshop.Definitions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LessonAttribute(int part, int order, string displayName, bool needsInput = true) : Attribute
{
    public int Part { get; } = part;
    public int Order { get; } = order;
    public string DisplayName { get; } = displayName;
    public bool NeedsInput { get; } = needsInput;
}
