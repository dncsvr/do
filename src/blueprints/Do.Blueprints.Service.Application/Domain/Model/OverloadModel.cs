﻿namespace Do.Domain.Model;

public record OverloadModel(
    MethodModel Method,
    bool IsPublic,
    bool IsProtected,
    bool IsVirtual,
    TypeModel? ReturnType = default
)
{
    public ModelCollection<ParameterModel> Parameters { get; private set; } = default!;
    public ModelCollection<TypeModel> CustomAttributes { get; private set; } = default!;

    internal void Init(ModelCollection<TypeModel> customAttributes, ModelCollection<ParameterModel> parameters)
    {
        CustomAttributes = customAttributes;
        Parameters = parameters;
    }
}
