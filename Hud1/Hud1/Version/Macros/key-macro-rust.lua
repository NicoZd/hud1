Label = "Key Macro Rust"
Description = "..."

-- https://learn.microsoft.com/en-us/uwp/api/windows.system.virtualkey

function Setup()
	window = FindWindow("Rust")
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

	Print("Press Down: " .. window)
	KeyDown(window, VK.D)
	Sleep(100)

	Print("Release Down: " .. window)
	KeyUp(window, VK.D)
	Sleep(500)

end

function Cleanup()
end