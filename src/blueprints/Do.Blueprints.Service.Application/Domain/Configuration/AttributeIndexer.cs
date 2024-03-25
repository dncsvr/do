﻿using Do.Domain.Model;

namespace Do.Domain.Configuration;

public class AttributeIndexer : ModelIndexerBase
{
    public static AttributeIndexer For<T>() where T : Attribute
        => new(typeof(T));

    readonly Type _type;

    AttributeIndexer(Type type) =>
        _type = type;

    protected override string IndexId =>
        TypeModel.IdFrom(_type);

    protected override bool AppliesTo(IModel model) =>
        model.CustomAttributes.ContainsKey(_type);
}
