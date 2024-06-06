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
    public string MacrosPath = "";

    [ObservableProperty]
    private ObservableCollection<Macro> _macros = [];

    [ObservableProperty]
    private int _selectedIndex = -1;

    [ObservableProperty]
    private bool _selected = false;

    private readonly FileSystemWatcher fileWatcher;

    private MacrosViewModel()
    {
        Macros = [];
        MacrosPath = Path.Combine(Setup.UserDataPath, "Macros");

        // fallback for vs studio xaml viewer;
        if (!Directory.Exists(MacrosPath))
        {
            MacrosPath = ".";
        }

        fileWatcher = new FileSystemWatcher(MacrosPath)
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

    internal void BuildNavigation()
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
            Console.WriteLine("OPEN {0}", MacrosPath);
            Process.Start("explorer.exe", MacrosPath);
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
        Console.WriteLine("MacrosViewModel UpdateFiles");
        foreach (var macro in Macros)
        {
            macro.Stop();
        }

        var fileEntries = Directory.GetFiles(MacrosPath, "*.lua");
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

    internal void OnEntry()
    {
        Console.WriteLine("OnEntry");
        Selected = true;

        if (Macros.Count == 0)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.RETURN_UP);
        }
    }

    internal void OnExit()
    {
        Console.WriteLine("OnExit");
        Selected = false;
        SelectedIndex = -1;
    }

    internal void OnEntryFromTop()
    {
        Console.WriteLine("OnEntryFromTop");
        SelectedIndex = 0;
    }

    internal void OnEntryFromBottom()
    {
        Console.WriteLine("OnEntryFromBottom");
        SelectedIndex = Macros.Count - 1;
    }

    internal void OnUp()
    {
        Console.WriteLine("OnUp {0}", SelectedIndex);
        if (SelectedIndex <= 0)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.RETURN_UP);
            return;
        }
        SelectedIndex--;
    }

    internal void OnDown()
    {
        Console.WriteLine("OnDown {0}", SelectedIndex);
        if (SelectedIndex >= Macros.Count - 1)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.RETURN_DOWN);
            return;
        }
        SelectedIndex++;
    }

    internal void OnLeft()
    {
        Console.WriteLine("OnLeft");
        Macros[SelectedIndex].OnLeft();
    }
    internal void OnRight()
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
