using MahApps.Metro.IconPacks;
using System.Windows.Media;

namespace Restless.Panama.Resources
{
    public static class Icons
    {
        public static PackIconMaterial Get(PackIconMaterialKind kind) => new() { Kind = kind, Foreground = Brushes.LightGray };
    }
}