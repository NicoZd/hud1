Label = "Press left mouse"
Description = "Press the left mouse button once."

function Setup()    
	MouseDown()
end

function OnMouseDown()  
	Stop()
end

function Cleanup()
	MouseUp()
end