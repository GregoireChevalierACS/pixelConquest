# Avalonia + SkiaSharp — Integration Notes

## Avalonia UI
- Cross-platform UI framework for .NET (Windows, macOS, Linux)
- XAML-based (like WPF) but also supports code-behind
- NuGet: `Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`

## SkiaSharp
- .NET binding for Google's Skia 2D graphics library
- Used by Flutter, Chrome, Android
- NuGet: `SkiaSharp`, `SkiaSharp.Views.Avalonia`

## Key types for pixelConquest
- `SKBitmap` — pixel buffer (read/write individual pixels)
- `SKCanvas` — drawing surface
- `SKColor` — ARGB color (struct)
- `SKPaint` — brush/pen settings
- `SKImageInfo` — bitmap dimensions + color type

## Rendering loop pattern
1. Maintain a `SKBitmap` as the pixel state
2. On each tick: strategies update the bitmap pixels
3. Avalonia `SKCanvasView.InvalidateVisual()` triggers a repaint
4. In `OnPaintSurface`: draw the bitmap to the canvas

## Pixel access
```
// Write a pixel
bitmap.SetPixel(x, y, new SKColor(255, 0, 0));

// Read a pixel  
SKColor color = bitmap.GetPixel(x, y);
```
