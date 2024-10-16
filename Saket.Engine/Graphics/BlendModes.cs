﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics;


// From photoshops explanations


public enum BlendMode
{
    /// <summary>
    /// Edits or paints each pixel to make it the result color. This is the default mode. (Normal mode is called Threshold when you’re working with a bitmapped or indexed-color image.)
    /// </summary>
    Normal,
    /// <summary>
    /// Looks at the color information in each channel and selects the base or blend color—whichever is darker—as the result color. Pixels lighter than the blend color are replaced, and pixels darker than the blend color do not change.
    /// </summary>
    Darken,
    /// <summary>
    /// Looks at the color information in each channel and multiplies the base color by the blend color. The result color is always a darker color. Multiplying any color with black produces black. Multiplying any color with white leaves the color unchanged. When you’re painting with a color other than black or white, successive strokes with a painting tool produce progressively darker colors. The effect is similar to drawing on the image with multiple marking pens.
    /// </summary>
    Multiply,
    /// <summary>
    /// Looks at the color information in each channel and darkens the base color to reflect the blend color by increasing the contrast between the two. Blending with white produces no change.
    /// </summary>
    ColorBurn,
    /// <summary>
    /// Looks at the color information in each channel and selects the base or blend color—whichever is lighter—as the result color. Pixels darker than the blend color are replaced, and pixels lighter than the blend color do not change.
    /// </summary>
    Lighten,
    /// <summary>
    /// Looks at each channel’s color information and multiplies the inverse of the blend and base colors. The result color is always a lighter color. Screening with black leaves the color unchanged. Screening with white produces white. The effect is similar to projecting multiple photographic slides on top of each other.
    /// </summary>
    Screen,
    /// <summary>
    /// Looks at the color information in each channel and brightens the base color to reflect the blend color by decreasing contrast between the two. Blending with black produces no change.
    /// </summary>
    ColorDodge,
    /// <summary>
    /// Multiplies or screens the colors, depending on the base color. Patterns or colors overlay the existing pixels while preserving the highlights and shadows of the base color. The base color is not replaced, but mixed with the blend color to reflect the lightness or darkness of the original color.
    /// </summary>
    Overlay,
    /// <summary>
    /// Darkens or lightens the colors, depending on the blend color. The effect is similar to shining a diffused spotlight on the image. If the blend color (light source) is lighter than 50% gray, the image is lightened as if it were dodged. If the blend color is darker than 50% gray, the image is darkened as if it were burned in. Painting with pure black or white produces a distinctly darker or lighter area, but does not result in pure black or white.
    /// </summary>
    SoftLight,
    /// <summary>
    /// Multiplies or screens the colors, depending on the blend color. The effect is similar to shining a harsh spotlight on the image. If the blend color (light source) is lighter than 50% gray, the image is lightened, as if it were screened. This is useful for adding highlights to an image. If the blend color is darker than 50% gray, the image is darkened, as if it were multiplied. This is useful for adding shadows to an image. Painting with pure black or white results in pure black or white.
    /// </summary>
    HardLight,
    /// <summary>
    /// Looks at the color information in each channel and subtracts either the blend color from the base color or the base color from the blend color, depending on which has the greater brightness value. Blending with white inverts the base color values; blending with black produces no change.
    /// </summary>
    Difference,
    /// <summary>
    /// Creates an effect similar to but lower in contrast than the Difference mode. Blending with white inverts the base color values. Blending with black produces no change.
    /// </summary>
    Exclusion,
    /// <summary>
    /// Looks at the color information in each channel and subtracts the blend color from the base color. In 8- and 16-bit images, any resulting negative values are clipped to zero.
    /// </summary>
    Subtract,
    /// <summary>
    /// Looks at the color information in each channel and divides the blend color from the base color.
    /// </summary>
    Divide,
    /// <summary>
    /// Creates a result color with the luminance and saturation of the base color and the hue of the blend color.
    /// </summary>
    Hue,
    /// <summary>
    /// Creates a result color with the luminance and hue of the base color and the saturation of the blend color. Painting with this mode in an area with no (0) saturation (gray) causes no change.
    /// </summary>
    Saturation,
    /// <summary>
    /// Creates a result color with the luminance of the base color and the hue and saturation of the blend color. This preserves the gray levels in the image and is useful for coloring monochrome images and for tinting color images.
    /// </summary>
    Color,
    /// <summary>
    /// Creates a result color with the hue and saturation of the base color and the luminance of the blend color. This mode creates the inverse effect of Color mode.
    /// </summary>
    Luminosity ,
    /// <summary>
    /// Looks at the color information in each channel and brightens the base color to reflect the blend color by increasing the brightness. Blending with black produces no change.
    /// </summary>
    Addition ,
    Invert,
    Overwrite,
}

public class BlendModes
{
    public static Color Blend(Color source, Color dest, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Normal: return NormalWithAlpha(source, dest);
            case BlendMode.Multiply: return MultiplyWithAlpha(source, dest);
            case BlendMode.Screen: return ScreenWithAlpha(source, dest);
            case BlendMode.Overlay: return OverlayWithAlpha(source, dest);
            case BlendMode.Darken: return DarkenWithAlpha(source, dest);
            case BlendMode.Lighten: return LightenWithAlpha(source, dest);
            case BlendMode.Difference: return DifferenceWithAlpha(source, dest);
            case BlendMode.Exclusion: return ExclusionWithAlpha(source, dest);
            case BlendMode.Addition: return AdditiveWithAlpha(source, dest);
            case BlendMode.Subtract: return SubtractWithAlpha(source, dest);
            case BlendMode.Divide: return DivideWithAlpha(source, dest);
            case BlendMode.HardLight: return HardLightWithAlpha(source, dest);
            case BlendMode.SoftLight: return SoftLightWithAlpha(source, dest);
            case BlendMode.Invert: return InvertWithAlpha(source, dest);
            case BlendMode.Overwrite: return source;
            default: return NormalWithAlpha(source, dest);
        }
    }
    /// <summary>
    /// Normal blend mode with alpha support.
    /// </summary>
    public static Color NormalWithAlpha(Color source, Color target)
    {
        return AlphaBlend(source, target);
    }

    /// <summary>
    /// Multiply blend mode with alpha support.
    /// </summary>
    public static Color MultiplyWithAlpha(Color source, Color target)
    {
        int r = (source.R * target.R) / 255;
        int g = (source.G * target.G) / 255;
        int b = (source.B * target.B) / 255;
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Screen blend mode with alpha support.
    /// </summary>
    public static Color ScreenWithAlpha(Color source, Color target)
    {
        int r = 255 - ((255 - source.R) * (255 - target.R)) / 255;
        int g = 255 - ((255 - source.G) * (255 - target.G)) / 255;
        int b = 255 - ((255 - source.B) * (255 - target.B)) / 255;
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Overlay blend mode with alpha support.
    /// </summary>
    public static Color OverlayWithAlpha(Color source, Color target)
    {
        int r = (target.R < 128) ? (2 * source.R * target.R) / 255 : 255 - (2 * (255 - source.R) * (255 - target.R)) / 255;
        int g = (target.G < 128) ? (2 * source.G * target.G) / 255 : 255 - (2 * (255 - source.G) * (255 - target.G)) / 255;
        int b = (target.B < 128) ? (2 * source.B * target.B) / 255 : 255 - (2 * (255 - source.B) * (255 - target.B)) / 255;
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Darken blend mode with alpha support.
    /// </summary>
    public static Color DarkenWithAlpha(Color source, Color target)
    {
        int r = Math.Min(source.R, target.R);
        int g = Math.Min(source.G, target.G);
        int b = Math.Min(source.B, target.B);
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Lighten blend mode with alpha support.
    /// </summary>
    public static Color LightenWithAlpha(Color source, Color target)
    {
        int r = Math.Max(source.R, target.R);
        int g = Math.Max(source.G, target.G);
        int b = Math.Max(source.B, target.B);
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Difference blend mode with alpha support.
    /// </summary>
    public static Color DifferenceWithAlpha(Color source, Color target)
    {
        int r = Math.Abs(source.R - target.R);
        int g = Math.Abs(source.G - target.G);
        int b = Math.Abs(source.B - target.B);
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Exclusion blend mode with alpha support.
    /// </summary>
    public static Color ExclusionWithAlpha(Color source, Color target)
    {
        int r = source.R + target.R - 2 * source.R * target.R / 255;
        int g = source.G + target.G - 2 * source.G * target.G / 255;
        int b = source.B + target.B - 2 * source.B * target.B / 255;
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Additive blend mode with alpha support.
    /// </summary>
    public static Color AdditiveWithAlpha(Color source, Color target)
    {
        int r = Math.Min(255, source.R + target.R);
        int g = Math.Min(255, source.G + target.G);
        int b = Math.Min(255, source.B + target.B);
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Subtract blend mode with alpha support.
    /// </summary>
    public static Color SubtractWithAlpha(Color source, Color target)
    {
        int r = Math.Max(0, source.R - target.R);
        int g = Math.Max(0, source.G - target.G);
        int b = Math.Max(0, source.B - target.B);
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Divide blend mode with alpha support.
    /// </summary>
    public static Color DivideWithAlpha(Color source, Color target)
    {
        int r = target.R == 0 ? 255 : Math.Min(255, (source.R * 255) / target.R);
        int g = target.G == 0 ? 255 : Math.Min(255, (source.G * 255) / target.G);
        int b = target.B == 0 ? 255 : Math.Min(255, (source.B * 255) / target.B);
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Hard light blend mode with alpha support.
    /// </summary>
    public static Color HardLightWithAlpha(Color source, Color target)
    {
        int r = (source.R < 128) ? (2 * source.R * target.R) / 255 : 255 - (2 * (255 - source.R) * (255 - target.R)) / 255;
        int g = (source.G < 128) ? (2 * source.G * target.G) / 255 : 255 - (2 * (255 - source.G) * (255 - target.G)) / 255;
        int b = (source.B < 128) ? (2 * source.B * target.B) / 255 : 255 - (2 * (255 - source.B) * (255 - target.B)) / 255;
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }

    /// <summary>
    /// Soft light blend mode with alpha support.
    /// </summary>
    public static Color SoftLightWithAlpha(Color source, Color target)
    {
        int r = (int)((target.R < 128) ? (2 * source.R * target.R / 255) + (source.R * source.R * (255 - 2 * target.R) / 255) / 255
                                 : (Math.Sqrt(source.R / 255.0) * (2 * target.R - 255)) + 2 * source.R * (255 - target.R) / 255);
        int g = (int)((target.G < 128) ? (2 * source.G * target.G / 255) + (source.G * source.G * (255 - 2 * target.G) / 255) / 255
                                 : (Math.Sqrt(source.G / 255.0) * (2 * target.G - 255)) + 2 * source.G * (255 - target.G) / 255);
        int b = (int)((target.B < 128) ? (2 * source.B * target.B / 255) + (source.B * source.B * (255 - 2 * target.B) / 255) / 255
                                 : (Math.Sqrt(source.B / 255.0) * (2 * target.B - 255)) + 2 * source.B * (255 - target.B) / 255);
        return AlphaBlend(new Color(r, g, b, source.A), target);
    }
    /// <summary>
    /// Invert blend mode with alpha support.
    /// </summary>
    public static Color InvertWithAlpha(Color source, Color target)
    {
        // Invert the source RGB values
        int invertedR = 255 - source.R;
        int invertedG = 255 - source.G;
        int invertedB = 255 - source.B;

        // Create the inverted color
        Color invertedSource = new Color(invertedR, invertedG, invertedB, source.A);

        // Perform alpha blending between the inverted source and the target
        return AlphaBlend(invertedSource, target);
    }
    /// <summary>
    /// Performs alpha blending between the source and target colors.
    /// </summary>
    private static Color AlphaBlend(Color source, Color target)
    {
        float srcAlpha = source.A / 255f; // Normalize source alpha [0, 1]
        float invAlpha = 1f - srcAlpha;

        // Blend each color channel
        int r = (int)((source.R * srcAlpha) + (target.R * invAlpha));
        int g = (int)((source.G * srcAlpha) + (target.G * invAlpha));
        int b = (int)((source.B * srcAlpha) + (target.B * invAlpha));

        // Calculate resulting alpha
        int a = (int)((source.A * srcAlpha) + (target.A * invAlpha));

        return new Color(r, g, b, a);
    }

    #region Without ALpha
    // Normal blend mode
    public static Color Normal(Color source, Color dest)
    {
        return source;
    }

    // Multiply blend mode
    public static Color Multiply(Color source, Color dest)
    {
        int r = (source.R * dest.R) / 255;
        int g = (source.G * dest.G) / 255;
        int b = (source.B * dest.B) / 255;
        return new Color(r, g, b, source.A);
    }

    // Screen blend mode
    public static Color Screen(Color source, Color dest)
    {
        int r = 255 - ((255 - source.R) * (255 - dest.R)) / 255;
        int g = 255 - ((255 - source.G) * (255 - dest.G)) / 255;
        int b = 255 - ((255 - source.B) * (255 - dest.B)) / 255;
        return new Color(r, g, b, source.A);
    }

    // Overlay blend mode
    public static Color Overlay(Color source, Color dest)
    {
        int r = (dest.R < 128) ? (2 * source.R * dest.R) / 255 : 255 - (2 * (255 - source.R) * (255 - dest.R)) / 255;
        int g = (dest.G < 128) ? (2 * source.G * dest.G) / 255 : 255 - (2 * (255 - source.G) * (255 - dest.G)) / 255;
        int b = (dest.B < 128) ? (2 * source.B * dest.B) / 255 : 255 - (2 * (255 - source.B) * (255 - dest.B)) / 255;
        return new Color(r, g, b, source.A);
    }

    // Darken blend mode
    public static Color Darken(Color source, Color dest)
    {
        int r = Math.Min(source.R, dest.R);
        int g = Math.Min(source.G, dest.G);
        int b = Math.Min(source.B, dest.B);
        return new Color(r, g, b, source.A);
    }

    // Lighten blend mode
    public static Color Lighten(Color source, Color dest)
    {
        int r = Math.Max(source.R, dest.R);
        int g = Math.Max(source.G, dest.G);
        int b = Math.Max(source.B, dest.B);
        return new Color(r, g, b, source.A);
    }

    // Difference blend mode
    public static Color Difference(Color source, Color dest)
    {
        int r = Math.Abs(source.R - dest.R);
        int g = Math.Abs(source.G - dest.G);
        int b = Math.Abs(source.B - dest.B);
        return new Color(r, g, b, source.A);
    }

    // Exclusion blend mode
    public static Color Exclusion(Color source, Color dest)
    {
        int r = source.R + dest.R - 2 * source.R * dest.R / 255;
        int g = source.G + dest.G - 2 * source.G * dest.G / 255;
        int b = source.B + dest.B - 2 * source.B * dest.B / 255;
        return new Color(r, g, b, source.A);
    }

    // Additive blend mode
    public static Color Additive(Color source, Color dest)
    {
        int r = Math.Min(255, source.R + dest.R);
        int g = Math.Min(255, source.G + dest.G);
        int b = Math.Min(255, source.B + dest.B);
        return new Color(r, g, b, source.A);
    }

    // Subtract blend mode
    public static Color Subtract(Color source, Color dest)
    {
        int r = Math.Max(0, source.R - dest.R);
        int g = Math.Max(0, source.G - dest.G);
        int b = Math.Max(0, source.B - dest.B);
        return new Color(r, g, b, source.A);
    }

    // Divide blend mode
    public static Color Divide(Color source, Color dest)
    {
        int r = dest.R == 0 ? 255 : Math.Min(255, (source.R * 255) / dest.R);
        int g = dest.G == 0 ? 255 : Math.Min(255, (source.G * 255) / dest.G);
        int b = dest.B == 0 ? 255 : Math.Min(255, (source.B * 255) / dest.B);
        return new Color(r, g, b, source.A);
    }

    // Hard Light blend mode
    public static Color HardLight(Color source, Color dest)
    {
        int r = (source.R < 128) ? (2 * source.R * dest.R) / 255 : 255 - (2 * (255 - source.R) * (255 - dest.R)) / 255;
        int g = (source.G < 128) ? (2 * source.G * dest.G) / 255 : 255 - (2 * (255 - source.G) * (255 - dest.G)) / 255;
        int b = (source.B < 128) ? (2 * source.B * dest.B) / 255 : 255 - (2 * (255 - source.B) * (255 - dest.B)) / 255;
        return new Color(r, g, b, source.A);
    }

    // Soft Light blend mode
    public static Color SoftLight(Color source, Color dest)
    {
        int r = (int)((dest.R < 128) ? (2 * source.R * dest.R / 255) + (source.R * source.R * (255 - 2 * dest.R) / 255) / 255
                               : (Math.Sqrt(source.R / 255.0) * (2 * dest.R - 255)) + 2 * source.R * (255 - dest.R) / 255);
        int g = (int)((dest.G < 128) ? (2 * source.G * dest.G / 255) + (source.G * source.G * (255 - 2 * dest.G) / 255) / 255
                               : (Math.Sqrt(source.G / 255.0) * (2 * dest.G - 255)) + 2 * source.G * (255 - dest.G) / 255);
        int b = (int)((dest.B < 128) ? (2 * source.B * dest.B / 255) + (source.B * source.B * (255 - 2 * dest.B) / 255) / 255
                               : (Math.Sqrt(source.B / 255.0) * (2 * dest.B - 255)) + 2 * source.B * (255 - dest.B) / 255);
        return new Color(r, g, b, source.A);
    }
    #endregion
}
