Label = "Sample Mouse & Keyboard Input"
Description = "Coding sample on how to retreive Mouse & Keyboard data."

function Setup()
	lastMouseAction = "None"
	lastKeyAction = "None"
end

function OnMouseDown(button)
	lastMouseAction = "Mouse Down " .. button;
end

function OnMouseUp(button)
	lastMouseAction = "Mouse Up " .. button;
end

function OnKeyDown(keyEvent)
	lastKeyAction = "KeyDown: "
		.. "Key=" .. NameOf(keyEvent.key)
		.. " Alt=".. (keyEvent.alt and "True" or "False")
		.. " Shift=" .. (keyEvent.shift and "True" or "False")
		.. " Repeated=" .. (keyEvent.repeated and "True" or "False");
end

function OnKeyUp(keyEvent)
	lastKeyAction = "KeyUp: "
		.. "Key=" .. NameOf(keyEvent.key)
		.. " Alt=".. (keyEvent.alt and "True" or "False")
		.. " Shift=" .. (keyEvent.shift and "True" or "False")
		.. " Repeated=" .. (keyEvent.repeated and "True" or "False");
end

function Run()	
	Print(""
		.. "Last Mouse Action: " .. lastMouseAction .. "\n"
		.. "Last Keyboard Action: " .. lastKeyAction
		)
end

function Cleanup()
	Print()
end