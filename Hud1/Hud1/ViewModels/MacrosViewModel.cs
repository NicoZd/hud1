﻿using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Hud1.ViewModels;

public partial class MacrosViewModel : ObservableObject
{
    public static readonly MacrosViewModel Instance = new();

    [ObservableProperty]
    private ObservableCollection<Macro> _macros = [];

    [ObservableProperty]
    private int _selectedIndex = -1;

    [ObservableProperty]
    private bool _selected = false;

    private readonly FileSystemWatcher fileWatcher;

    public string macrosPath = "";

    private MacrosViewModel()
    {
        Macros = [];

        macrosPath = Path.Combine(Setup.VersionPath, "Macros");

        // fallback for vs studio xaml viewer;
        if (!Directory.Exists(macrosPath))
        {
            macrosPath = ".";
        }

        fileWatcher = new FileSystemWatcher(macrosPath)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName
        };
        fileWatcher.Changed += OnDirectoryChanged;
        fileWatcher.Created += OnDirectoryChanged;
        fileWatcher.Deleted += OnDirectoryChanged;
        fileWatcher.Renamed += OnDirectoryChanged;
        fileWatcher.EnableRaisingEvents = true;

        UpdateFiles();
    }

    public void BuildNavigation()
    {
        var Configure = HudViewModel.Instance.Configure;

        Configure(NavigationStates.MACROS)
            .OnEntryFrom(NavigationTriggers.UP, OnEntryFromBottom)
            .OnEntryFrom(NavigationTriggers.DOWN, OnEntryFromTop)
            .OnEntry(OnEntry)
            .OnExit(OnExit)
            .InternalTransition(NavigationTriggers.LEFT, OnLeft)
            .InternalTransition(NavigationTriggers.RIGHT, OnRight)
            .InternalTransition(NavigationTriggers.UP, OnUp)
            .InternalTransition(NavigationTriggers.DOWN, OnDown)
            .Permit(NavigationTriggers.RETURN_UP, NavigationStates.MENU_MACRO)
            .Permit(NavigationTriggers.RETURN_DOWN, NavigationStates.MACROS_FOLDER);

        NavigationStates.MACROS_FOLDER.RightAction = () =>
        {
            Console.WriteLine("OPEN {0}", macrosPath);
            Process.Start("explorer.exe", macrosPath);
        };

        Configure(NavigationStates.MACROS_FOLDER)
            .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.MACROS_FOLDER.ExecuteRight);

        HudViewModel.Instance.MakeNav(NavigationStates.MENU_MACRO, NavigationStates.MACRO_VISIBLE,
            [NavigationStates.MACROS, NavigationStates.MACROS_FOLDER]);
    }

    private void OnDirectoryChanged(object sender, FileSystemEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(new Action(UpdateFiles));
    }

    private void UpdateFiles()
    {
        Console.WriteLine("UpdateFiles");
        foreach (var macro in Macros)
            macro.Running = false;

        var fileEntries = Directory.GetFiles(macrosPath, "*.lua");
        var temp = new ObservableCollection<Macro>();
        foreach (var fileEntry in fileEntries)
        {
            temp.Add(new Macro(fileEntry, this));
        }

        Macros.Clear();
        foreach (var m in temp.OrderBy(m => m.Label))
        {
            Macros.Add(m);
        }

        var lastIndex = SelectedIndex;
        SelectedIndex = -1;
        SelectedIndex = lastIndex;
    }

    partial void OnSelectedIndexChanged(int value)
    {
        Console.WriteLine("OnSelectedIndexChanged", value);

        for (var i = 0; i < Macros.Count; i++)
        {
            var macro = Macros[i];
            macro.Selected = i == value;
            if (i == value)
            {
                NavigationStates.MACROS.Hint = macro.Description;
            }
        }
    }

    public void OnEntry()
    {
        Console.WriteLine("OnEntry");
        Selected = true;

        if (Macros.Count == 0)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.RETURN_UP);
        }
    }

    public void OnExit()
    {
        Console.WriteLine("OnExit");
        Selected = false;
        SelectedIndex = -1;
    }

    public void OnEntryFromTop()
    {
        Console.WriteLine("OnEntryFromTop");
        SelectedIndex = 0;
    }

    public void OnEntryFromBottom()
    {
        Console.WriteLine("OnEntryFromBottom");
        SelectedIndex = Macros.Count - 1;
    }

    public void OnUp()
    {
        Console.WriteLine("OnUp {0}", SelectedIndex);
        if (SelectedIndex <= 0)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.RETURN_UP);
            return;
        }
        SelectedIndex--;
    }

    public void OnDown()
    {
        Console.WriteLine("OnDown {0}", SelectedIndex);
        if (SelectedIndex >= Macros.Count - 1)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.RETURN_DOWN);
            return;
        }
        SelectedIndex++;
    }

    public void OnLeft()
    {
        Console.WriteLine("OnLeft");
        Macros[SelectedIndex].OnLeft();
    }
    public void OnRight()
    {
        Console.WriteLine("OnRight");
        Macros[SelectedIndex].OnRight();
    }

    internal void SelectMacro(Macro macro)
    {
        HudViewModel.Instance.SelectNavigationState(NavigationStates.MACROS);
        var index = Macros.IndexOf(macro);
        if (index >= 0)
        {
            SelectedIndex = index;
        }
    }
}
