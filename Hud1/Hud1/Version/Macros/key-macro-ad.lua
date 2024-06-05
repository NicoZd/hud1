Label = "Key Macro"
Description = "Repeat pressing and releasing A and D"

-- https://learn.microsoft.com/en-us/uwp/api/windows.system.virtualkey

function Run()
	Print("Press A")
	KeyDown(VK.A)
	Sleep(100)

	Print("Release A")
	KeyUp(VK.A)

	Print("Press D")
	KeyDown(VK.D)
	Sleep(100)

	Print("Release D")
	KeyUp(VK.D)
end

function OnMouseDown()  
	Stop()
end