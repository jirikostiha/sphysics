using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Checks of the spans that describe the bodies of a system, one item per body. </summary>
internal static class SystemSpans
{
    /// <summary> Throws unless every span of the bodies has as many items as the first one. </summary>
    /// <param name="length"> Length of the first span, the one the others are measured against. </param>
    /// <param name="others"> Lengths of the other spans. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RequireLengths(int length, params ReadOnlySpan<int> others)
    {
        foreach (int other in others)
        {
            if (other != length)
                throw new ArgumentException("Every span of the bodies must have as many items as there are bodies.");
        }
    }
}
