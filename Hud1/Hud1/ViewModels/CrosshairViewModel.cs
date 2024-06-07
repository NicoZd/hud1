﻿using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Controls;
using Hud1.Helpers;
using Hud1.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.System;

namespace Hud1.ViewModels;

public partial class CrosshairViewModel : ObservableObject
{
    public static readonly CrosshairViewModel Instance = new();

    [ObservableProperty]
    private Image _crosshairImage = new();

    private Dictionary<string, Func<int, Brush, bool, Drawing>> FormRenderFunctions = [];
    private Dictionary<string, Brush> ColorOptions = [];

    private CrosshairViewModel()
    {


        VirtualKeyboardHook.KeyDown += HandleKeyDown;
    }

    private void HandleKeyDown(KeyEvent keyEvent)
    {
        if (!keyEvent.repeated && keyEvent.alt && keyEvent.shift && keyEvent.key is VirtualKey.C)
        {
            keyEvent.block = true;
            NavigationStates.CROSSHAIR_ENABLED.SelectionBoolean = !NavigationStates.CROSSHAIR_ENABLED.SelectionBoolean;
        }
    }
    internal void BuildNavigation()
    {
        FormRenderFunctions = [];
        ColorOptions = [];

        var Configure = HudViewModel.Instance.Configure;

        NavigationStates.CROSSHAIR_ENABLED.SelectionBoolean = UserConfig.Current.CrosshairEnabled;
        NavigationStates.CROSSHAIR_ENABLED.LeftAction = NavigationStates.CROSSHAIR_ENABLED.BooleanLeft;
        NavigationStates.CROSSHAIR_ENABLED.RightAction = NavigationStates.CROSSHAIR_ENABLED.BooleanRight;
        Configure(NavigationStates.CROSSHAIR_ENABLED)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_ENABLED.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_ENABLED.ExecuteRight);

        NavigationStates.CROSSHAIR_MONITOR.SelectionLabel = UserConfig.Current.CrosshairDisplay;
        ChangeDisplay(0)();
        NavigationStates.CROSSHAIR_MONITOR.LeftAction = ChangeDisplay(-1);
        NavigationStates.CROSSHAIR_MONITOR.RightAction = ChangeDisplay(1);
        Configure(NavigationStates.CROSSHAIR_MONITOR)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_MONITOR.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_MONITOR.ExecuteRight);

        NavigationStates.CROSSHAIR_FORM.SelectionLabel = UserConfig.Current.CrosshairForm;
        NavigationStates.CROSSHAIR_FORM.Options = [new Option("Dot"), new Option("Ring"), new Option("Cross"), new Option("Diagonal"), new Option("3 Dots")];
        NavigationStates.CROSSHAIR_FORM.SelectOption();
        NavigationStates.CROSSHAIR_FORM.LeftAction = NavigationStates.CROSSHAIR_FORM.OptionLeft;
        NavigationStates.CROSSHAIR_FORM.RightAction = NavigationStates.CROSSHAIR_FORM.OptionRight;
        Configure(NavigationStates.CROSSHAIR_FORM)
          .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_FORM.ExecuteLeft)
          .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_FORM.ExecuteRight);
        FormRenderFunctions = new Dictionary<string, Func<int, Brush, bool, Drawing>>
        {
            { "Dot", CrosshairForms.RenderDot },
            { "Ring", CrosshairForms.RenderRing },
            { "Cross", CrosshairForms.RenderCross },
            { "Diagonal", CrosshairForms.RenderDiagonal },
            { "3 Dots", CrosshairForms.ThreeDots },
        };

        NavigationStates.CROSSHAIR_COLOR.SelectionLabel = UserConfig.Current.CrosshairColor;
        NavigationStates.CROSSHAIR_COLOR.Spacing = 0;
        NavigationStates.CROSSHAIR_COLOR.Options = [

            new Option("#00C000"),
            new Option("#00FF00"),
            new Option("#B2FF00"),
            new Option("#FFFF00"),
            new Option("#FFAC00"),

            new Option("#FF3100"),
            new Option("#D10133"),
            new Option("#FF00FF"),
            new Option("#C000FF"),
            new Option("#0000FF"),

            new Option("#0080FF"),
            new Option("#00FFFF"),
            new Option("#FFFFFF"),
            ];
        NavigationStates.CROSSHAIR_COLOR.SelectOption();
        NavigationStates.CROSSHAIR_COLOR.LeftAction = NavigationStates.CROSSHAIR_COLOR.OptionLeft;
        NavigationStates.CROSSHAIR_COLOR.RightAction = NavigationStates.CROSSHAIR_COLOR.OptionRight;
        Configure(NavigationStates.CROSSHAIR_COLOR)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_COLOR.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_COLOR.ExecuteRight);

        foreach (var option in NavigationStates.CROSSHAIR_COLOR.Options)
        {
            ColorOptions.Add(option.Value, (SolidColorBrush)new BrushConverter().ConvertFromString(option.Value)!);
        }

        NavigationStates.CROSSHAIR_OPACITY.SelectionLabel = UserConfig.Current.CrosshairOpacity;
        NavigationStates.CROSSHAIR_OPACITY.LeftAction = ChangeOpacity(-0.1);
        NavigationStates.CROSSHAIR_OPACITY.RightAction = ChangeOpacity(0.1);
        Configure(NavigationStates.CROSSHAIR_OPACITY)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_OPACITY.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_OPACITY.ExecuteRight);

        NavigationStates.CROSSHAIR_SIZE.SelectionLabel = UserConfig.Current.CrosshairSize;
        NavigationStates.CROSSHAIR_SIZE.Options = [new Option("1"), new Option("2"), new Option("3"), new Option("4"), new Option("5")];
        NavigationStates.CROSSHAIR_SIZE.SelectOption();
        NavigationStates.CROSSHAIR_SIZE.LeftAction = NavigationStates.CROSSHAIR_SIZE.OptionLeft;
        NavigationStates.CROSSHAIR_SIZE.RightAction = NavigationStates.CROSSHAIR_SIZE.OptionRight;
        Configure(NavigationStates.CROSSHAIR_SIZE)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_SIZE.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_SIZE.ExecuteRight);

        NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean = UserConfig.Current.CrosshairOutline;
        NavigationStates.CROSSHAIR_OUTLINE.LeftAction = NavigationStates.CROSSHAIR_OUTLINE.BooleanLeft;
        NavigationStates.CROSSHAIR_OUTLINE.RightAction = NavigationStates.CROSSHAIR_OUTLINE.BooleanRight;
        Configure(NavigationStates.CROSSHAIR_OUTLINE)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CROSSHAIR_OUTLINE.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CROSSHAIR_OUTLINE.ExecuteRight);

        HudViewModel.Instance.MakeNav(NavigationStates.MENU_CROSSHAIR, NavigationStates.CROSSHAIR_VISIBLE,
            [
            NavigationStates.CROSSHAIR_ENABLED,
            NavigationStates.CROSSHAIR_MONITOR,
            NavigationStates.CROSSHAIR_FORM,
            NavigationStates.CROSSHAIR_COLOR,
            NavigationStates.CROSSHAIR_OPACITY,
            NavigationStates.CROSSHAIR_SIZE,
            NavigationStates.CROSSHAIR_OUTLINE,
            ]);

        Redraw();
    }

    internal Action ChangeDisplay(int dir)
    {
        return () =>
        {
            var current = int.Parse(NavigationStates.CROSSHAIR_MONITOR.SelectionLabel);
            var next = Math.Min(Math.Max(current + dir, 0), Monitors.All.Count - 1);
            Debug.Print($"CrosshairViewModel ChangeDisplay {current} => {next}");
            NavigationStates.CROSSHAIR_MONITOR.SelectionLabel = "" + next;
        };
    }

    private Action ChangeOpacity(double dir)
    {
        return () =>
        {
            var current = double.Parse(NavigationStates.CROSSHAIR_OPACITY.SelectionLabel);
            var next = Math.Min(Math.Max(current + dir, 0.1), 1);
            NavigationStates.CROSSHAIR_OPACITY.SelectionLabel = "" + (Math.Round(next * 10) / 10);
        };
    }

    internal void Redraw()
    {
        // Debug.Print("Enabled {0}", NavigationStates.CROSSHAIR_ENABLED.SelectionBoolean);
        // Debug.Print("Form {0}", NavigationStates.CROSSHAIR_FORM.SelectionLabel);
        // Debug.Print("Size {0}", NavigationStates.CROSSHAIR_SIZE.SelectionLabel);
        // Debug.Print("Color {0}", NavigationStates.CROSSHAIR_COLOR.SelectionLabel);
        // Debug.Print("Outline {0}", NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean);

        var scale = int.Parse(NavigationStates.CROSSHAIR_SIZE.SelectionLabel);
        var color = ColorOptions[NavigationStates.CROSSHAIR_COLOR.SelectionLabel];

        if (!FormRenderFunctions.ContainsKey(NavigationStates.CROSSHAIR_FORM.SelectionLabel))
        {
            Debug.Print("Form is null {0}", NavigationStates.CROSSHAIR_FORM.SelectionLabel);
            return;
        }

        var formFunction = FormRenderFunctions[NavigationStates.CROSSHAIR_FORM.SelectionLabel];
        var geometryDrawing = GetGeometryDrawing(scale, color, formFunction);

        DrawingImage drawingImage = new(geometryDrawing);
        drawingImage.Freeze();

        DPIAwareImage image = new()
        {
            Visibility = NavigationStates.CROSSHAIR_ENABLED.SelectionBoolean ? Visibility.Visible : Visibility.Hidden,
            Source = drawingImage,
            Stretch = Stretch.None,
        };

        CrosshairImage = image;

        UpdateAllOptions(scale, color, formFunction);
    }

    private void UpdateAllOptions(int scale, Brush color, Func<int, Brush, bool, Drawing> formFunction)
    {
        foreach (var option in NavigationStates.CROSSHAIR_FORM.Options)
        {
            var optionFormFunction = FormRenderFunctions[option.Value];
            var brush = new SolidColorBrush(((SolidColorBrush)Application.Current.FindResource("BrushSolidSuperBright")).Color);
            var geometryDrawing = GetGeometryDrawing(5, brush, optionFormFunction);

            DrawingImage drawingImage = new(geometryDrawing);
            drawingImage.Freeze();

            DPIAwareImage image = new()
            {
                Source = drawingImage,
                Stretch = Stretch.None,
                Margin = new Thickness(4, 0, 4, 0)
            };

            option.Image = image;
        }

        foreach (var option in NavigationStates.CROSSHAIR_COLOR.Options)
        {
            var colorOption = ColorOptions[option.Value];

            GeometryGroup areaGroup = new();
            areaGroup.Children.Add(new RectangleGeometry(new Rect(-10, -10, 20, 20)));

            GeometryDrawing foregroundDrawing = new() { Geometry = areaGroup, Brush = colorOption };

            DrawingImage drawingImage = new(foregroundDrawing);
            drawingImage.Freeze();

            Image image = new()
            {
                Source = drawingImage,
                Stretch = Stretch.None,
                Margin = new Thickness(0, 2, 0, 0)
            };

            option.Image = image;
        }

        foreach (var option in NavigationStates.CROSSHAIR_SIZE.Options)
        {
            var sizeOption = int.Parse(option.Value);
            var geometryDrawing = GetGeometryDrawing(sizeOption, color, formFunction);

            DrawingImage drawingImage = new(geometryDrawing);
            drawingImage.Freeze();

            DPIAwareImage image = new()
            {
                Source = drawingImage,
                Stretch = Stretch.None,
                Margin = new Thickness(4, 0, 4, 0)
            };

            option.Image = image;
        }

    }

    private static Drawing GetGeometryDrawing(int scale, Brush color, Func<int, Brush, bool, Drawing> optionFormFunction)
    {
        var drawingWithFixedSize = (DrawingGroup)optionFormFunction(scale, color, NavigationStates.CROSSHAIR_OUTLINE.SelectionBoolean);

        GeometryGroup areaGroup = new();
        areaGroup.Children.Add(new RectangleGeometry(new Rect(-12, -12, 24, 24)));

        drawingWithFixedSize.Children.Insert(0, new GeometryDrawing() { Geometry = areaGroup, Brush = Brushes.Transparent });
        //drawingWithFixedSize.Children.Insert(0, new GeometryDrawing() { Geometry = areaGroup, Brush = (Brush)new BrushConverter().ConvertFromString("#33ff0000") });


        drawingWithFixedSize.Opacity = double.Parse(NavigationStates.CROSSHAIR_OPACITY.SelectionLabel);

        return drawingWithFixedSize;
    }
}