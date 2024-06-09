Label = "How-to read Mouse & Keyboard"
Description = "Coding sample on how to retreive mouse & keyboard data. Start and use Keyboard and Mouse to see input data."

function Setup()
	lastMouseAction = "None"
	lastKeyAction = "None"
end

function OnMouseDown(button, point)
	lastMouseAction = "MouseDown=" .. button .. " x=" .. point.x .. " y=" .. point.y;
end

function OnMouseUp(button, point)
	lastMouseAction = "MouseUp=" .. button .. " x=" .. point.x .. " y=" .. point.y;
end

function OnKeyDown(keyEvent)
	lastKeyAction = ""
		.. "KeyDown=" .. NameOf(keyEvent.key)
		.. " Alt=".. (keyEvent.alt and "True" or "False")
		.. " Shift=" .. (keyEvent.shift and "True" or "False")
		.. " Repeated=" .. (keyEvent.repeated and "True" or "False");
end

function OnKeyUp(keyEvent)
	lastKeyAction = ""
		.. "KeyUp=" .. NameOf(keyEvent.key)
		.. " Alt=".. (keyEvent.alt and "True" or "False")
		.. " Shift=" .. (keyEvent.shift and "True" or "False")
		.. " Repeated=" .. (keyEvent.repeated and "True" or "False");
end

function Run()	
	Print(""
		.. "Last Mouse Action:\n  • " .. lastMouseAction .. "\n\n"
		.. "Last Keyboard Action:\n  • " .. lastKeyAction
		)
end

function Cleanup()
	Print()
end