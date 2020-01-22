using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using N2.Engine;
using N2.Security;

namespace N2.Definitions
{
    public static class DefinitionExtensions
    {
        public static IEnumerable<ItemDefinition> AllowedBelow(this IEnumerable<ItemDefinition> allDefinitions, ItemDefinition parentDefinition, ContentItem parentItem, ContentItem childItem, IDefinitionManager definitions)
        {
            foreach (var definition in allDefinitions)
            {
                if (IsAllowed(childItem, definition, parentItem, parentDefinition, definitions))
                    yield return definition;
            }
        }

        public static IEnumerable<TemplateDefinition> AllowedBelow(this IEnumerable<TemplateDefinition> allTemplates, ItemDefinition parentDefinition, ContentItem parentItem, IDefinitionManager definitions)
        {
            if (allTemplates == null) yield break;

            foreach (var template in allTemplates)
            {
                if (IsAllowed(null, template.Definition, parentItem, parentDefinition, definitions))
                    yield return template;
            }
        }

        public static PropertyDefinition GetOrCreate(this IDictionary<string, PropertyDefinition> properties, string name, Type propertyType)
        {
            PropertyDefinition property;
            if (!properties.TryGetValue(name, out property))
                properties[name] = property = new PropertyDefinition(name, propertyType);
            return property;
        }

        public static bool IsThrowable(this ItemDefinition definition)
        {
            return definition.GetCustomAttributes<ThrowableAttribute>().Select(va => va.Throwable).FirstOrDefault() != AllowInTrash.No;
        }

        public static bool IsVersionable(this ItemDefinition definition)
        {
            return definition.GetCustomAttributes<VersionableAttribute>().Select(va => va.Versionable).FirstOrDefault() != AllowVersions.No;
        }

        public static IEnumerable<TemplateDefinition> WhereAllowed(this IEnumerable<TemplateDefinition> allTemplates, ContentItem parentItem, string zoneName, IPrincipal user, IDefinitionManager definitions, ISecurityManager security)
        {
            return allTemplates.AllowedBelow(definitions.GetDefinition(parentItem), parentItem, definitions)
                .Where(t => t.Definition.IsAllowedInZone(zoneName) && security.IsAuthorized(t.Definition, user, parentItem));
        }

        public static IEnumerable<T> WhereAuthorized<T>(this IEnumerable<T> allSecurable, ISecurityManager security, IPrincipal user, ContentItem parentItem) where T : ISecurableBase
        {
            return allSecurable.Where(s => security.IsAuthorized(s, user, parentItem));
        }

        internal static T TryClone<T>(this T obj)
        {
            if (obj is ICloneable)
                return (T)(obj as ICloneable).Clone();
            else
                return obj;
        }

        private static bool IsAllowed(ContentItem childItem, ItemDefinition childDefinition, ContentItem parentItem, ItemDefinition parentDefinition, IDefinitionManager definitions)
        {
            var ctx = new AllowedDefinitionQuery { Parent = parentItem, ParentDefinition = parentDefinition, Child = childItem, ChildDefinition = childDefinition, Definitions = definitions };
            var filters = parentDefinition.AllowedChildFilters.Union(childDefinition.AllowedParentFilters).ToList();
            try
            {
                if (filters.Any(f => f.IsAllowed(ctx) == AllowedDefinitionResult.Allow))
                    // filter specificly allows -> allow
                    return true;
                else if (!filters.Any(f => f.IsAllowed(ctx) == AllowedDefinitionResult.Deny))
                    // no filter denies -> allow
                    return true;

                // no filter allowed, but some filter denied -> deny
                return false;
            }
            catch(NullReferenceException)
            {
                Logger.ErrorFormat("IsAllowed > childItem: {0}", childItem);
                Logger.ErrorFormat("IsAllowed > childDefinition: {0}", childDefinition);
                Logger.ErrorFormat("IsAllowed > parentItem: {0}", parentItem);
                Logger.ErrorFormat("IsAllowed > parentDefinition: {0}", parentDefinition);
                Logger.ErrorFormat("IsAllowed > definitions: {0}", definitions);

                throw;
            }
        }
    }
}
