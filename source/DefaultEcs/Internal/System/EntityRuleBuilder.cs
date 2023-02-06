using System;
using System.Reflection;
using DefaultEcs.System;

namespace DefaultEcs.Internal.System
{
    internal static class EntityRuleBuilder
    {

        public static EntityQueryBuilder GetSystemQueryBuilder(World world, Type systemType)
        {
            var setFilterAttributes = systemType.GetCustomAttributes(typeof(ComponentAttribute), true);
            bool isEnabledSet = systemType.GetCustomAttribute<DisabledAttribute>() is null;
            EntityQueryBuilder rawSet = isEnabledSet ? world.GetEntities() : world.GetDisabledEntities();
            foreach (var attribute in setFilterAttributes)
            {
                switch (attribute)
                {
                    case WithAttribute withAttr:
                        {
                            rawSet = rawSet.With(withAttr.ComponentTypes);
                            break;
                        }
                    case WithoutAttribute withoutAttr:
                        {
                            rawSet = rawSet.Without(withoutAttr.ComponentTypes);
                            break;
                        }
                    case WithEitherAttribute withEAttr:
                        {
                            rawSet = rawSet.WithEither(withEAttr.ComponentTypes);
                            break;
                        }
                    case WithoutEitherAttribute withoutEAttr:
                        {
                            rawSet = rawSet.WithoutEither(withoutEAttr.ComponentTypes);
                            break;
                        }
                    case WhenAddedAttribute whenAddAttr:
                        {
                            rawSet = rawSet.WhenAdded(whenAddAttr.ComponentTypes);
                            break;
                        }
                    case WhenAddedEitherAttribute whenAddEAttr:
                        {
                            rawSet = rawSet.WhenAddedEither(whenAddEAttr.ComponentTypes);
                            break;
                        }
                    case WhenChangedAttribute whenChangeAttr:
                        {
                            rawSet = rawSet.WhenChanged(whenChangeAttr.ComponentTypes);
                            break;
                        }
                    case WhenChangedEitherAttribute whenChangeEAttr:
                        {
                            rawSet = rawSet.WhenChangedEither(whenChangeEAttr.ComponentTypes);
                            break;
                        }
                    case WhenRemovedAttribute whenRemoveAttr:
                        {
                            rawSet = rawSet.WhenRemoved(whenRemoveAttr.ComponentTypes);
                            break;
                        }
                    case WhenRemovedEitherAttribute whenRemoveEAttr:
                        {
                            rawSet = rawSet.WhenRemovedEither(whenRemoveEAttr.ComponentTypes);
                            break;
                        }
                    default: continue;
                }
            }
            return rawSet;
        }

    }
}
