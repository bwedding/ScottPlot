﻿namespace ScottPlot;

public interface IMultiplot
{
    #region subplot collection management

    // TODO: collection manager

    /// <summary>
    /// Number of subplots in this multiplot
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Add the given plot to the collection of subplots
    /// </summary>
    void AddPlot(Plot plot);

    /// <summary>
    /// Remove the given plot from the collection of subplots
    /// </summary>
    void RemovePlot(Plot plot);

    /// <summary>
    /// Return the plot at the given index
    /// </summary>
    Plot GetPlot(int index);

    /// <summary>
    /// Return all plots in this multiplot
    /// </summary>
    Plot[] GetPlots();

    #endregion

    #region subplot layout management

    // TODO: LastRender state management

    /// <summary>
    /// This logic is used at render time to place subplots 
    /// within the rectangle containing the entire multiplot figure.
    /// </summary>
    IMultiplotLayout Layout { get; set; }

    /// <summary>
    /// Return the plot beneath the given pixel according to the last render.
    /// Returns null if no render occurred or the pixel is not over a plot.
    /// </summary>
    Plot? GetPlotAtPixel(Pixel pixel);

    #endregion

    public MultiplotSharedAxisManager SharedAxes { get; }

    /// <summary>
    /// Render this multiplot onto the given canvas using a layout
    /// created to fit inside the given rectangle.
    /// </summary>
    void Render(SKCanvas canvas, PixelRect figureRect);
}

public static class IMultiplotExtensions
{
    /// <summary>
    /// Reset this multiplot so it only contains the given plot
    /// </summary>
    public static void Reset(this IMultiplot multiplot, Plot plot)
    {
        multiplot.AddPlots(0);
        multiplot.AddPlot(plot);
    }

    /// <summary>
    /// Reset this multiplot so it only contains the first plot
    /// </summary>
    public static void Reset(this IMultiplot multiplot)
    {
        multiplot.AddPlots(1);
    }

    /// <summary>
    /// Create a new image, render the multiplot onto it, and return it
    /// </summary>
    public static Image Render(this IMultiplot multiplot, int width, int height)
    {
        SKImageInfo imageInfo = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        SKSurface surface = SKSurface.Create(imageInfo);
        PixelRect rect = new(0, width, height, 0);
        multiplot.Render(surface.Canvas, rect);
        return new(surface);
    }

    /// <summary>
    /// Render the multiplot into the clip boundary of the given surface.
    /// </summary>
    public static void Render(this IMultiplot multiplot, SKSurface surface)
    {
        multiplot.Render(surface.Canvas, surface.Canvas.LocalClipBounds.ToPixelRect());
    }

    /// <summary>
    /// Save the multiplot as a PNG image file
    /// </summary>
    public static SavedImageInfo SavePng(this IMultiplot multiplot, string filename, int width = 800, int height = 600)
    {
        return multiplot.Render(width, height).SavePng(filename);
    }

    /// <summary>
    /// Add a single plot to the collection of subplots
    /// </summary>
    public static void AddPlot(this IMultiplot multiplot)
    {
        multiplot.AddPlot(new Plot());
    }

    /// <summary>
    /// Add (or remove) plots until the target number of subplots is achieved
    /// </summary>
    public static void AddPlots(this IMultiplot multiplot, int total)
    {
        while (multiplot.Count < total)
        {
            multiplot.AddPlot();
        }

        while (multiplot.Count > total)
        {
            multiplot.RemovePlot(multiplot.GetPlots().Last());
        }
    }
}
