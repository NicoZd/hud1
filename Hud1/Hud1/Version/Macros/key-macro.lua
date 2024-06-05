Label = "Key Macro Chrome"
Description = "..."

-- https://learn.microsoft.com/en-us/uwp/api/windows.system.virtualkey

function Setup()
	window = FindWindow("Notepad")
	if window == -1 then
		Print("No open window with title containing 'Notepad'")
		Sleep(1000)
		Stop()
	else
		Print("Found Notepad: " .. window)
		Sleep(1000)
	end
end

function Run()
	Print("Press Down: " .. window)
	KeyDown(window, VK.A)
	Sleep(100)

	Print("Release Down: " .. window)
	KeyUp(window, VK.A)
	Sleep(500)

end

function Cleanup()
end