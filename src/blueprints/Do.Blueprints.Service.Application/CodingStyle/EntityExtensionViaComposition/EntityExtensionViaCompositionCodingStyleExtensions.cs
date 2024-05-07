﻿using Do.CodingStyle;
using Do.CodingStyle.EntityExtensionViaComposition;
using Do.Domain.Model;
using System.Diagnostics.CodeAnalysis;

namespace Do;

public static class EntityExtensionViaCompositionCodingStyleExtensions
{
    public static EntityExtensionViaCompositionCodingStyleFeature EntityExtensionViaComposition(this CodingStyleConfigurator _) =>
        new();

    public static bool TryGetEntityTypeFromExtension(this TypeModel type, DomainModel domain, [NotNullWhen(true)] out TypeModel? entityType)
    {
        entityType = default;

        if (!type.TryGetMetadata(out var entityExtensionMetadata)) { return false; }
        if (!entityExtensionMetadata.TryGetSingle<EntityExtensionAttribute>(out var entityExtensionAttribute)) { return false; }

        entityType = domain.Types[entityExtensionAttribute.EntityType];

        return true;
    }
}